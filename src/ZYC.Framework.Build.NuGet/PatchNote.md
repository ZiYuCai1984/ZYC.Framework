# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added a current-workspace submenu to the workspace context menu, exposing the registered workspace layout, split, merge, swap, focus, and reset actions directly from the workspace area

---

## 🛠 Improvements

* Updated the Aspire Dashboard integration to display executable arguments and environment-variable values without sensitive-value masking, while logging compatibility-patch failures without blocking Aspire startup
* Changed the active-language indicator from appended title text to a dedicated check icon, preserving localized menu titles
* Updated ZYC.CoreToolkit to 4.0.0 and Aspire to 13.5.0
* Refreshed the multilingual README, quick-start, project-template, and troubleshooting documentation with version 1.4.4 installer links and CLI/package examples

---

## 🐛 Bug Fixes

* Fixed hidden single-instance application windows not being restored when the application is launched again

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
