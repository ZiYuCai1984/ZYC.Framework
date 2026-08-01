# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added manual NuGet module installation by package ID and version, enabling modules that are not returned by the configured feed's search results to be installed directly
* Added input validation and in-progress state to the manual installer; installation failures now show an exception toast without closing the dialog, while successful installs prompt for an application restart

---

## 🛠 Improvements

* Refreshed the multilingual README, quick-start, project-template, and troubleshooting documentation with version 1.4.3 installer links and CLI/package examples

---

## 🐛 Bug Fixes

* Fixed the Aspire binary-source selector so the active source uses a dedicated checkmark icon while option titles continue to use localization

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
