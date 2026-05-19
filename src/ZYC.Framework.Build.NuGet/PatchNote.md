# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added startup URI support for `zyc://` links, including command-line parsing, single-instance forwarding, and navigation after tab restore completes
* Added Windows installer registration for the `zyc` URL protocol
* Added CLI tool packaging to the NuGet release build so `ZYC.Framework.CLI` is packed alongside the framework packages

---

## 🛠 Improvements

* Converted the CLI project to SDK-style `net10.0` dotnet tool packaging and kept project templates in the CLI output
* Simplified target framework selection so WPF projects default to `net10.0-windows`, while Abstractions and build tools stay on non-WPF `net10.0`
* Moved opt-in Fody wiring into `Directory.Build.targets` after shared NuGet package version updates
* Marked generated project and module template projects with `IgnoreFromPublish` so scaffolded templates stay out of product publish packaging
* Updated Aspire package references to `13.3.3`
* Refreshed README and quick-start version references for `1.3.0`
* Added XML documentation for `IFileOpenMainMenuItemsProvider`

---

## ⚠️ Breaking Changes

* Removed the unused `IFileNewMainMenuItemsProvider` abstraction and built-in `FileNewMainMenuItemsProvider`
* Removed the legacy CLI `DotnetToolSettings.xml` packaging path and disabled the `--gui` CLI shortcut while the CLI is packaged as a .NET tool

---

## 🐛 Bug Fixes

* Forwarded startup URI requests from secondary app launches to the already-running instance instead of dropping them during single-instance shutdown
* Queued startup URI navigation until tab restore is complete so startup links can resolve against restored tab state

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
