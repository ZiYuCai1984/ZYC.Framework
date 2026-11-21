# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added a typed web bridge for pages hosted in WebView2. Mark an interface with `[ExposeToWeb]` (`ZYC.Framework.Abstractions.Web`) and register its implementation in the DI container; `WebBridge` (`ZYC.Framework.WebView2`) then dispatches the calls a page sends with `postMessage` to that service and returns the result or the error message. Arguments and results are camelCase JSON with enums as strings, `Task`/`ValueTask` results are awaited, and a `CancellationToken` parameter receives `CancellationToken.None`. The service name defaults to the interface name without its leading `I`; set `[ExposeToWeb(Name = "...")]` to change it. Overloaded methods are not supported.
* Added the `zyc web-types` command, which generates TypeScript declarations for the `[ExposeToWeb]` interfaces of the given assemblies: `zyc web-types --assembly <path.dll> --output <file.d.ts>`. The output file is only rewritten when its content changes. The command reads assembly metadata only, so it can run against a host output folder.
* The `react` project template now calls C# services with types, for example `await hostService('GreetingService').greetAsync('React')`. It includes a sample `Services/IGreetingService.cs` marked with `[ExposeToWeb]` and `[ExposeToMCP]`, so the same interface serves C# code, the MCP server, and the page. The view passes page messages to `WebBridge`, and every build regenerates `ClientApp/src/host-services.d.ts` from the `[ExposeToWeb]` interfaces.

> ⚠️ Only pass messages from pages you trust to `WebBridge` (check `CoreWebView2WebMessageReceivedEventArgs.Source`): every `[ExposeToWeb]` interface becomes callable by the page. The `react` template only forwards messages from the page it loaded.

---

## 🛠 Improvements

* `AppConfig.IconPath` now defaults to an empty string instead of `null`.

---

## 🐛 Fixes

* Fixed the host's main window not always becoming `Application.MainWindow`. A window constructed by a module during `LoadAsync` (for example, a tab item factory that injects `IAppBusyWindow`) became `MainWindow` instead, which broke `LifetimeScopeResource` because it reads the lifetime scope from `MainWindow.DataContext`. The host now sets `MainWindow` explicitly.
* Fixed `TabItemInstanceBase.Dispose` creating the view of a tab item whose view had never been created, only to dispose it.
* Fixed the plugins button in the WebView2 menu bar staying disabled when its command was evaluated before the browser extension list was bound.
* Fixed the CLI logging `System.String[]` instead of the file name when it ignores a module assembly.

---

## 🔄 Compatibility

* In projects created from the `react` template, `ClientApp/src/webview.ts` now exports `callHost` and `hostService` instead of `postToHost` and `onHostMessage`, and the `ping`/`pong` sample is replaced by the `GreetingService` call. The build also runs `ZYC.Framework.CLI.exe web-types` after every build, including with `-p:SkipClientAppBuild=true`. Projects created with earlier versions are not affected.

---

## 📦 Installation

```bash
dotnet add package ZYC.Framework.Alpha --version $(Version)
dotnet tool install --global ZYC.Framework.CLI --version $(Version)
```

---

## 📚 Resources

* 📖 [Documentation]($(DocumentUrl))
* 🐞 [Report an Issue]($(IssuesUrl))

---

**Thank you for trying out ZYC.Framework.Alpha!**
Your feedback will help shape future releases.
