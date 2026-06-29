# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added a Tools menu entry for editing the mutex override ID without manually changing files
* Added a dedicated mutex override dialog with save/delete actions, localized UI text, and restart prompts after changes

---

## 🛠 Improvements

* Updated package metadata, documentation, CLI examples, and project template references for version 1.3.7
* Raised NuGet module search defaults to the NuGet.org request limit and clamped configured values between 1 and 1000
* Improved minimize behavior so windows hidden from the taskbar are hidden instead of minimized
* Made restart banners stay visible until dismissed, so restart-required actions are less likely to disappear unnoticed
* Centralized mutex ID generation in `MutexTools` and reused it for single-instance startup and startup-URI pipe naming

---

## 🐛 Bug Fixes

* Kept custom layout management compatible with framework extension points that need non-sealed dialog classes
* Refined mutex override persistence so save/delete flows use the same startup mutex path

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
