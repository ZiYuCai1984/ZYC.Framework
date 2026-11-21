// Typed calls from the page to C# services through the WebView2 bridge (window.chrome.webview).
// The C# side is WebBridge, called from OnWebMessageReceived in UI/__PROJECT_SHORT_NAME__View.cs.
// The service types come from host-services.d.ts, which the build generates from the [ExposeToWeb] interfaces.

import type { HostServices } from './host-services'

interface WebViewBridge {
  postMessage(message: unknown): void
  addEventListener(type: 'message', listener: (event: MessageEvent) => void): void
}

declare global {
  interface Window {
    chrome?: { webview?: WebViewBridge }
  }
}

interface BridgeReply {
  bridge: 'result' | 'error'
  id: number
  result?: unknown
  error?: string
}

interface PendingCall {
  resolve: (value: unknown) => void
  reject: (reason: Error) => void
}

const bridge = window.chrome?.webview
const pendingCalls = new Map<number, PendingCall>()
let nextCallId = 1

/** True inside the WebView2 host; false when the page is opened in a normal browser. */
export const isHosted = bridge !== undefined

bridge?.addEventListener('message', (event) => {
  const reply = event.data as Partial<BridgeReply> | null
  if (!reply || typeof reply.id !== 'number' || (reply.bridge !== 'result' && reply.bridge !== 'error')) {
    return
  }

  const call = pendingCalls.get(reply.id)
  if (!call) {
    return
  }

  pendingCalls.delete(reply.id)
  if (reply.bridge === 'error') {
    call.reject(new Error(reply.error ?? 'The host call failed.'))
  } else {
    call.resolve(reply.result)
  }
})

/** Calls a method of an [ExposeToWeb] C# service by name. Prefer the typed hostService() below. */
export function callHost(service: string, method: string, args: unknown[]): Promise<unknown> {
  const webview = bridge
  if (!webview) {
    return Promise.reject(new Error('This page is not running inside the WebView2 host.'))
  }

  const id = nextCallId++
  return new Promise((resolve, reject) => {
    pendingCalls.set(id, { resolve, reject })
    webview.postMessage({ bridge: 'call', id, service, method, args })
  })
}

/**
 * Returns a typed proxy for an [ExposeToWeb] C# service, for example
 * `await hostService('GreetingService').greetAsync('React')`.
 */
export function hostService<K extends keyof HostServices>(name: K): HostServices[K] {
  return new Proxy(
    {},
    {
      // `then` stays undefined so the proxy is never mistaken for a promise.
      get: (_target, method) =>
        typeof method === 'string' && method !== 'then'
          ? (...args: unknown[]) => callHost(String(name), method, args)
          : undefined,
    },
  ) as HostServices[K]
}
