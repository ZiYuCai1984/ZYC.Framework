# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added `IOverlayManager.Show(object[] targets, object? passThrough = null)` to highlight multiple target areas in a single guide overlay while keeping them interactive; overlapping target areas remain transparent
* Added a `Text` dependency property to `NoItemView` for customizable, localized empty-state messages

---

## 🛠 Improvements

* Replaced translucent backgrounds in the window title, main menu button, tab area, status bar, and update/restart banners with a consistent theme background
* Guide overlays now detach target layout and visibility handlers and clear target and pass-through references when disposed
* Added a guide overlay demo to the Mock module showing multiple targets in a single overlay

---

## 🐛 Fixes

* Fixed automatic revealing of masked values in the embedded Aspire Dashboard when values change or previously revealed controls are reused

---

## 🔄 Compatibility

* Existing single-target overlay calls remain supported
* Custom implementations of `IOverlayManager` must implement the new `Show(object[] targets, object? passThrough = null)` overload and be rebuilt

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
