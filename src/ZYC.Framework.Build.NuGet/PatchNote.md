# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added in-tab navigation through `INavigableTabItemInstance` and `ITabManager.NavigateAsync(ITabItemInstance, Uri)`.

---

## 🛠 Improvements

* The address bar now navigates within the current browser or file explorer tab when supported. Unsupported URIs are opened through the registered tab factories.

---

## 🐛 Fixes

* Fixed singleton tabs opening multiple instances when URI parameters differ. Opening an existing singleton now focuses its tab and workspace while preserving its current URI.
* Fixed background URI changes and closing inactive tabs unexpectedly switching the selected tab.
* Fixed persisted tab URI updates targeting the wrong tab when multiple tabs share the same URI.

---

## 🔄 Compatibility

* **Breaking:** Replaced `TabInternalNavigatingAsync` with `NotifyUriChangedAsync` for URI change notifications. Update implementations and callers of the affected tab interfaces.
* **Behavior change:** `IsSingle = true` now allows only one tab per registered factory across all workspaces, regardless of URI. Set it to `false` for multi-instance tabs; external opens then create new tabs even for the same URI. Text editors, hex editors, and text previews now explicitly allow multiple instances.

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
