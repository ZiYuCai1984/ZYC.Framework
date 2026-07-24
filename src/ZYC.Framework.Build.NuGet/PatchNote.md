# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added release packaging for `ZYC.Framework.Modules.ApiReference` and its `.Abstractions` package, with the generated DocFX site bundled alongside the module
* Made the large local API reference an optional NuGet module instead of shipping it in the main `ZYC.Framework.Alpha` package

---

## 🛠 Improvements

* Enabled documentation generation in the NuGet.org release workflow and linked release notes to `ProductInfoExtended.DocumentUrl`
* Resolved local API reference content relative to the module assembly so it can load directly from the NuGet package cache
* Kept framework host project references private when packing the API reference module, avoiding invalid per-project package dependencies
* Added Japanese, Simplified Chinese, Traditional Chinese, and Korean localization for Accounts and Online Documentation
* Updated ZYC.CoreToolkit to 3.9.8, Microsoft WebView2 to 1.0.4078.44, and refreshed documentation and installation examples for version 1.4.1

---

## 🐛 Bug Fixes

* Prevented duplicate NuGet package pushes by limiting publication to the copied root-level release artifacts
* Corrected the `OnlineDocuemnt` typo to `OnlineDocumentation`, updated its visible title, and standardized `API Reference` capitalization
* Applied localization to the Accounts title-bar fallback text instead of always displaying the English label

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
