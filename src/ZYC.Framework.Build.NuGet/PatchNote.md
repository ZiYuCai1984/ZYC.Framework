# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added the `react` project template. `zyc new <ProjectName> --template react` creates a host whose tab is a WebView2 view running a Vite + React + TypeScript front end, with a typed `window.chrome.webview` message bridge between the page and C#.
* Added the `__PROJECT_WEB_NAME__` project template token: a lowercase, hyphen-separated project name used as the npm package name and the WebView2 virtual host, for example `mycompany-tools`.
* Added the application-level `HybridIcon16Style` and `PackIconMaterial16Style` styles for 16 px icons.

---

## 🛠 Improvements

* Projects created from the `react` template build their front end as part of `dotnet build`: `npm install` runs when `package.json` changes and `npm run build` runs when front-end sources change. Debug builds load the Vite dev server at `http://localhost:5173` while `npm run dev` is running, so front-end changes appear without rebuilding.
* `zyc new` writes front-end template files (`package.json`, `tsconfig.json`, `.ts`, `.tsx`, `.html`, `.css`) as UTF-8 without a BOM so Node.js tooling reads them correctly. Other text files are still written as UTF-8 with BOM.
* The `minimal` template now includes an `.editorconfig`, and the `.editorconfig` files of the `console`, `build`, and `wpf` templates add rules for `*.DotSettings` and `*.fx` files.

---

## 🐛 Fixes

None

---

## 🔄 Compatibility

* **Breaking change:** `AspireModuleContansts` in `ZYC.Framework.Modules.Aspire.Abstractions` has been renamed to `AspireModuleConstants`. Update source references; assemblies compiled against the old name must be recompiled.
* Building a project created from the `react` template requires Node.js 20.19+ or 22.12+, and running it requires the Microsoft Edge WebView2 Runtime. Pass `-p:SkipClientAppBuild=true` to skip the front-end build.

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
