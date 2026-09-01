# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added `WorkspaceConfig.IsWorkspaceEmptyIndexVisible` so applications can show or hide the large workspace index displayed when a workspace has no tabs

---

## ⚠️ Breaking Changes

* Renamed `WorkspaceMenuConfig` to `WorkspaceConfig` and renamed `IsVisible` to `IsMenuVisible`; applications using the previous workspace-menu configuration API must update their references

---

## 🛠 Improvements

* Updated the framework dependency baseline to ZYC.CoreToolkit 4.0.1 and completed the FxC dependency version settings in generated console and WPF projects
* Refreshed multilingual download, installation, quick-start, project-template, and troubleshooting documentation for version 1.4.6

---

## 🐛 Bug Fixes

* Fixed the update main-menu item's configuration subscription lifetime so it is disposed together with the menu item

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
