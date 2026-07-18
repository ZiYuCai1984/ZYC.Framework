# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Expanded MdXaml with robust HTML fragment rendering, country-flag emoji support, asynchronous remote and data URI images, and customizable hyperlink handling
* Added Copy Image and Save Image As context-menu commands for images rendered from Markdown and HTML, including SVG content
* Added an Online Documentation command under About and exposed ProductInfoExtended.DocumentUrl

---

## 🛠 Improvements

* Hardened HTML rendering with AngleSharp fragment parsing, URL sanitization, safer whitespace and entity handling, bounded table spans, image validation, and caching
* Centralized URI path normalization and adopted standards-based query parsing for tab routes, GitHub sign-in callbacks, and deep links
* Registered NuGet-loaded modules as ModuleBase so they can be resolved and unloaded correctly during shutdown
* Updated package metadata, documentation, CLI examples, project templates, and installer links for version 1.4.0, and updated AngleSharp to 1.5.2

---

## 🐛 Bug Fixes

* Prevented tab-close and update-notification commands from acting on disconnected WPF data contexts and made update navigation close its toast consistently
* Excluded ApiReference projects and stale *_wpftmp.csproj files from release solution generation, preventing unintended publish content and missing old-version assembly failures
* Improved GitHub authentication route matching and callback parameter decoding, including nested nonce extraction

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
