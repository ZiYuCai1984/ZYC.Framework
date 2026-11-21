import { useState } from 'react'
import type { HostGreeting } from './host-services'
import { hostService, isHosted } from './webview.ts'

const greetingService = hostService('GreetingService')

function App() {
  const [count, setCount] = useState(0)
  const [greeting, setGreeting] = useState<HostGreeting>()
  const [error, setError] = useState<string>()

  async function greet() {
    try {
      setGreeting(await greetingService.greetAsync('React'))
      setError(undefined)
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : String(reason))
    }
  }

  return (
    <main className="app">
      <h1>__PROJECT_NAME__</h1>
      <p className="muted">React running in a ZYC.Framework WebView2 tab.</p>

      <section className="card">
        <button type="button" onClick={() => setCount((value) => value + 1)}>
          Count is {count}
        </button>
        <p className="muted">
          Edit <code>ClientApp/src/App.tsx</code>. With <code>npm run dev</code> running, changes
          show up without restarting the app.
        </p>
      </section>

      <section className="card">
        <button type="button" disabled={!isHosted} onClick={greet}>
          Call host
        </button>
        <p className="muted">
          {isHosted
            ? (error ?? greeting?.message ?? 'Calls IGreetingService in C# through the typed web bridge.')
            : 'Open this page inside the app to call the C# host.'}
        </p>
      </section>
    </main>
  )
}

export default App
