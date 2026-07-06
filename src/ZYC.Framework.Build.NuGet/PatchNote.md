# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added documentation coverage for the Accounts, Accounts.GitHub, and ChromeExtensions modules in the architecture and built-in module guides
* Added window-title extension point guidance for IWindowTitleManager, IWindowTitleExtendManager, and module-owned title-bar content

---

## 🛠 Improvements

* Updated Aspire hosting to configure ProjectDirectory, Aspire:Store:Path, DcpPublisher:CliPath, and DcpPublisher:DashboardPath through builder configuration
* Removed the reflection-based DCP options override and kept host-builder discovery in a focused Aspire partial class
* Updated package metadata, documentation, CLI examples, project template references, generated docs, and installer links for version 1.3.9
* Updated dependencies to ZYC.CoreToolkit 3.9.6 and Aspire 13.4.6

---

## 🐛 Bug Fixes

* Kept Aspire state under the framework settings directory by introducing a dedicated aspire-store path
* Preserved updated Aspire sidecar path resolution for orchestration and dashboard packages without relying on internal DCP option members
* Clarified troubleshooting guidance for mutex overrides, NuGet search pagination, and serialized ModuleManager install/uninstall/refresh operations

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
