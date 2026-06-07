# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added a WebView2 plugins button and dialog to inspect loaded browser extensions, including name, ID, enabled state, and total count
* Added support for forwarding configured custom browser arguments into the built-in Web Browser module, including extension-loading scenarios
* Added a Tools menu entry for the built-in Web Browser module

---

## 🛠 Improvements

* Improved the Chrome Extensions manager with manifest-based extension icons and localized labels for the page title, Chrome Web Store action, and empty selection state
* Added localized resources for Plugins, Chrome Extensions, and Chrome Web Store across Japanese, Simplified Chinese, Traditional Chinese, and Korean
* Extended the dialog manager so dialogs can be resolved with Autofac parameters when they need runtime payloads
* Updated dependency versions for ZYC.CoreToolkit and Aspire, and added the WebView2 MahApps dependency required by the new dialog surface

---

## 🐛 Bug Fixes

* Prevented the application busy window from stealing activation when displayed
* Hardened extension icon loading with safe manifest parsing, path validation, caching, and fallback icons

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
