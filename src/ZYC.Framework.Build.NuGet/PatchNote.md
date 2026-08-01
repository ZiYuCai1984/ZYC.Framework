# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added an extensible **Tools > Others** menu through `IToolsOthersMainMenuItemsProvider`, with built-in items now grouped under the new section
* Added direct executable launching for Aspire command-line services through `ExecutablePath` and `Arguments`, while retaining shell-command fallback support
* Added `ISettingsManager.SaveConfig` so modules can persist configuration through the settings abstraction

---

## 🛠 Improvements

* Updated language configuration persistence to use `ISettingsManager` instead of writing directly to the settings directory
* Disabled Aspire CLI telemetry by default and added a helper for resolving the Dashboard URI from the configured environment
* Updated generated project templates to ignore framework and .NET build output directories
* Refreshed multilingual documentation, installer links, and CLI/package examples for version 1.4.2

---

## 🐛 Bug Fixes

* Fixed the Aspire startup notification's Dashboard button to navigate through the injected command and close the notification after activation
* Made module-load error details scrollable when the content exceeds the available page space

---

## 📦 Installation

```bash
dotnet add package ZYC.Framework.Alpha --version $(Version)
dotnet tool install --global ZYC.Framework.CLI --version $(Version)
```

---

## 📚 Resources

* 📖 [Documentation]($(DocumentUrl))
* 🐞 [Report an Issue](https://github.com/ZiYuCai1984/ZYC.Framework/issues)

---

**Thank you for trying out ZYC.Framework.Alpha!**
Your feedback will help shape future releases.
