# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added tab reload support through `ITabManager.ReloadAsync(ITabItemInstance)`
* Added a built-in tab header `Reload` context menu item
* Added `TabItemReloadedEvent` so reload replacements can be observed by the shell and extensions

---

## 🛠 Improvements

* Reloaded tabs now replace the existing tab at the same workspace position, keep focus when appropriate, and create a fresh instance even for single-instance tab routes
* Reload requests now respect locked tabs and tab closing cancellation
* Reordered built-in tab header context menu items so reload, close, lock, and move actions appear in a clearer sequence
* Ignored `.claude/` workspace artifacts in source control

---

## 📚 Documentation

* Added expanded documentation for architecture, navigation/workspace behavior, extension points, built-in modules, module development, project templates, and troubleshooting
* Added Japanese, Korean, Simplified Chinese, and Traditional Chinese documentation pages for the new topics
* Updated the documentation table of contents to expose the new topic pages
* Added the same documentation topics to the project documentation template output

---

## 🐛 Bug Fixes

* Fixed module delete command availability checks so null command parameters return `false` instead of throwing during UI command evaluation

---

## 📦 Installation

```bash
dotnet add package ZYC.Framework.Alpha --version $(Version)
dotnet tool install --global ZYC.Framework.CLI --version $(Version)
```

---

## 📚 Resources

* 📖 [Documentation](https://github.com/ZiYuCai1984/ZYC.Framework)
* 🐞 [Report an Issue](https://github.com/ZiYuCai1984/ZYC.Framework/issues)

---

**Thank you for trying out ZYC.Framework.Alpha!**
Your feedback will help shape future releases.
