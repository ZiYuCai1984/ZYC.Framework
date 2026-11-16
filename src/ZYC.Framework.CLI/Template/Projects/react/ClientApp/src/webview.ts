// Typed wrapper around the WebView2 bridge (window.chrome.webview).
// The C# side is OnWebMessageReceived in UI/__PROJECT_SHORT_NAME__View.cs.

export interface HostMessage {
  type: string
  payload?: string
}

interface WebViewBridge {
  postMessage(message: unknown): void
  addEventListener(type: 'message', listener: (event: MessageEvent) => void): void
  removeEventListener(type: 'message', listener: (event: MessageEvent) => void): void
}

declare global {
  interface Window {
    chrome?: { webview?: WebViewBridge }
  }
}

const bridge = window.chrome?.webview

/** True inside the WebView2 host; false when the page is opened in a normal browser. */
export const isHosted = bridge !== undefined

export function postToHost(message: HostMessage): void {
  bridge?.postMessage(message)
}

/** Subscribes to messages from the host. Returns a function that unsubscribes. */
export function onHostMessage(handler: (message: HostMessage) => void): () => void {
  if (!bridge) {
    return () => {}
  }

  const listener = (event: MessageEvent) => handler(event.data as HostMessage)
  bridge.addEventListener('message', listener)
  return () => bridge.removeEventListener('message', listener)
}
