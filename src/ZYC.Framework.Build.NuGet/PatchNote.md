# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added compatibility updates for ZYC.CoreToolkit 3.9.4 and Aspire 13.4.5
* Added version 1.3.6 package metadata for framework runtime and NuGet publishing

---

## 🛠 Improvements

* Updated README and multilingual documentation install examples, demo download links, and project template samples for version 1.3.6
* Simplified WebView2 plugin menu item initialization to align with the generated menu model
* Polished the WebView2 plugin split-button text weight for a lighter menu appearance
* Clarified the workspace tab lifetime-scope warning comment for future maintenance

---

## 🐛 Bug Fixes

* Prevented Aspire service disposal from breaking teardown when `DistributedApplication.Dispose()` raises an assembly version mismatch; the exception is now captured and logged
* Kept Aspire service cleanup resilient by disposing local gates and composite subscriptions before attempting distributed application disposal

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
