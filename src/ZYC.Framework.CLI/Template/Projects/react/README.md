# __PROJECT_NAME__

A ZYC.Framework host project whose tab UI is a React app (Vite + TypeScript) running in WebView2.

## Requirements

- .NET 10 SDK
- Node.js 20.19+ or 22.12+ (required by Vite)
- Microsoft Edge WebView2 Runtime

## Layout

| Path | Purpose |
|------|---------|
| `Module.cs` | Registers the view as a tab. |
| `UI/__PROJECT_SHORT_NAME__View.cs` | WebView2 host. Serves the built React app from `https://__PROJECT_WEB_NAME__.example/` and answers messages from the page. |
| `ClientApp/` | React front end. `src/webview.ts` wraps the `window.chrome.webview` bridge. |

## Build and run

Build and start the project as usual. The build runs `npm install` (when `package.json` changes) and `npm run build` (when front-end files change) in `ClientApp`, then copies `ClientApp/dist` to the `ClientApp` folder in the output directory.

Pass `-p:SkipClientAppBuild=true` to skip the front-end build.

## Hot reload while developing

```bash
cd ClientApp
npm run dev
```

Then start a Debug build of the app. While the Vite dev server is running on `http://localhost:5173`, the view loads it instead of the built files, so changes to `ClientApp/src` appear immediately. Release builds always use the built files.

## Talking to the host

The page sends messages with `postToHost({ type: 'ping' })` and receives replies through `onHostMessage`. On the C# side, `OnWebMessageReceived` in `UI/__PROJECT_SHORT_NAME__View.cs` handles them and replies with `CoreWebView2.PostWebMessageAsJson`. Add your own message types in both places.
