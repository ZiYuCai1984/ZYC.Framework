# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added **Settings → Others**, with **Reset All** moved into this submenu, and exposed `ISettingsOthersMainMenuItemsProvider` so modules can register additional items there
* Added `ModuleManagerMainMenuAnchors.ModuleManager` and `SettingsMainMenuPriority.Others` for menu customization

---

## 🛠 Improvements

* Moved **Local Modules** and **NuGet Modules** directly under **Extensions**, grouped with the shared module-manager anchor
* Reordered Settings menu groups so language and localization resources appear after settings and secrets, followed by **Others**
* Updated multilingual installation commands, project examples, and demo download links for this release

---

## 🐛 Fixes

* Fixed unintended address-bar navigation caused by text matching, arrow-key selection, or binding updates; navigation now starts when pressing Enter, clicking Go, or clicking a history item
* Fixed Enter handling while the history drop-down is open so the committed history entry is submitted; holding Enter no longer repeatedly submits navigation
* Queued address-bar submissions to prevent overlapping navigation requests, while allowing later submissions to continue after a failed request
* Fixed address-bar navigation targeting the wrong workspace after focus changes by routing each request to the workspace that owns the address bar
* Preserved the address bar's binding to the focused tab when normalizing and submitting an address, keeping the displayed address synchronized when switching tabs

---

## 🔄 Compatibility

* Renamed `SettingMainMenuAnchors` to `SettingsMainMenuAnchors`, and renamed its `Other` member to `Others` with anchor value `090Others`. Update references and rebuild dependent modules
* Changed `LanguageModuleConstants.Anchor` from `Language` to `050Language`. Rebuild modules that reference this constant and update any hard-coded anchor values to preserve menu grouping
* Removed the built-in implementation and registration of `IModuleManagerMainMenuItemsProvider`; the interface remains available. Extensions that resolved this provider should register items through `RegisterExtensionsMainMenuItem<T>()` or `IExtensionsMainMenuItemsProvider`, using `ModuleManagerMainMenuAnchors.ModuleManager` to join the module-manager group

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
