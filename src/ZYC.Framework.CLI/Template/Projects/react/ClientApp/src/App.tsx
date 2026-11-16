import { useEffect, useState } from 'react'
import { isHosted, onHostMessage, postToHost } from './webview.ts'

function App() {
  const [count, setCount] = useState(0)
  const [reply, setReply] = useState<string>()

  useEffect(
    () =>
      onHostMessage((message) => {
        if (message.type === 'pong') {
          setReply(message.payload)
        }
      }),
    [],
  )

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
        <button type="button" disabled={!isHosted} onClick={() => postToHost({ type: 'ping' })}>
          Ping host
        </button>
        <p className="muted">
          {isHosted
            ? (reply ?? 'Sends a message to the C# view and shows its reply.')
            : 'Open this page inside the app to talk to the C# host.'}
        </p>
      </section>
    </main>
  )
}

export default App
