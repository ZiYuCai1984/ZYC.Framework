# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added `TabItemsPreloadCompletedEvent` in `ZYC.Framework.Abstractions.Event` to notify subscribers when the initial tab preloading stage completes during workspace startup

---

## 🛠 Improvements

* Updated the restart prompt banner to use a dedicated light yellow background (`#FFFFE1`), with `ZYC.Colors.BannerBackground` and `ZYC.Brushes.BannerBackground` resources in the default theme
* Updated installation commands, project template examples, and demo download links for this release across all supported documentation languages

---

## 🐛 Fixes

None

## 🔄 Compatibility

* `TabManagerRestoreCompleted` remains available and is published first; the new `TabItemsPreloadCompletedEvent` follows after a `DispatcherPriority.Background` callback during initial workspace loading
* The restart prompt banner now uses `ZYC.Brushes.BannerBackground` instead of `MahApps.Brushes.SystemControlBackgroundChromeMediumLow`; override the new brush resource when customizing its background

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
