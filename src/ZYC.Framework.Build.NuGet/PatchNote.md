# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added `console` and `wpf` project templates to `zyc new`, with ready-to-run .NET 10 solutions and shared build configuration
* Added `UpdateConfig.ShowUpdateMenu` so applications can dynamically show or hide the update menu

---

## 🛠 Improvements

* Simplified `zyc new-module` by defaulting `--src-root` to the current directory and keeping generation scoped to the module and abstractions templates
* Improved the version switcher for development-template installations and added distinct `Running` and `Startup` status labels
* Refreshed the multilingual README, quick-start, project-template, and troubleshooting documentation for version 1.4.5 and the new CLI templates

---

## 🐛 Bug Fixes

* Fixed spacing for top-positioned toast notifications so gaps are applied on the correct edge

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
