# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added **Microsoft.Extensions.Logging** support across the codebase, including:

  * log4net adapter
  * compatibility extensions for existing logging infrastructure
* Introduced unified logging integration via **ILoggerFactory** and **ILogger<>**, backed by log4net

---

## 🛠 Improvements

* Updated DI registrations to consistently provide `ILoggerFactory` and `ILogger<>`
* Refactored **CLIView** and **NuGetModuleManagerView** to:

  * Use injected loggers
  * Integrate busy window / busy state handling
* Improved tab management logic and removed obsolete or redundant code
* Centralized and cleaned up **NuGet package references and MSBuild targets**
* General code cleanup and improved dependency injection patterns

---

## 🐛 Bug Fixes

* N/A (no user-facing bug fixes in this release)

---

## 📦 Installation

```bash
dotnet add package ZYC.Framework.Alpha --version $(Version)
```

---

## 📚 Resources

* 📖 [Documentation](https://github.com/ZiYuCai1984/ZYC.Framework)
* 🐞 [Report an Issue](https://github.com/ZiYuCai1984/ZYC.Framework/issues)

---

**Thank you for trying out ZYC.Framework.Alpha!**
Your feedback will help shape future releases.
