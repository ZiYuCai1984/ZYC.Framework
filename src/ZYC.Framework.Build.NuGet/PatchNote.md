# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added a standalone `ZYC.Framework.Core` NuGet package for referencing the Core library directly
* Added `ToastConfig.IsMuted` to suppress new toast notifications when enabled
* Added `DoubleToGridLengthConverter` for converting between `double` values and WPF `GridLength` values

---

## 🛠 Improvements

* Added a Mock module demo comparing standard and emoji text boxes with short text and wrapped long text
* Updated installation commands, project template examples, and demo download links for this release across all supported documentation languages

---

## 🐛 Fixes

* Fixed `Emoji.Wpf.TextBox` styling by applying the application theme to its internal editor, forwarding border, padding, font, alignment, and focus/hover settings, and removing default paragraph margins

---

## 🔄 Compatibility

* `ToastConfig.IsMuted` defaults to `false`, preserving existing toast behavior unless explicitly enabled
* Enabling toast muting suppresses new notifications; it does not dismiss notifications already displayed

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
