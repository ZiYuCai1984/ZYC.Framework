# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added a Web Browser plugin manager dialog for searching installed Chrome extension packages and adding or removing them from the browser startup configuration
* Added a WebView2 plugin split-button menu with quick access to plugin management and installed extension popup/options pages
* Added update download progress reporting, including status text and a percentage progress bar in the update UI
* Added support for configuring custom WebView2 browser arguments, including `--load-extension` startup scenarios

---

## 🛠 Improvements

* Stabilized Chrome extension package identity by synchronizing the CRX public key into unpacked manifests and validating CRX2/CRX3 package headers
* Improved extension page navigation by storing manifest page paths in package metadata and resolving `chrome-extension://` URLs against the runtime-loaded extension ID
* Improved the WebView2 menu extension model with plugin-specific menu items and command parameters
* Updated WebView2 startup settings to disable custom crash reporting and reputation checking for embedded browser hosts
* Updated package metadata, documentation, and multilingual install examples for version 1.3.5

---

## 🐛 Bug Fixes

* Fixed canceled update download tasks so cancellation is recorded and the download command does not surface an expected cancellation as a failure
* Restored the update context to the available-update state when a download is canceled
* Corrected the plugin count display format and the localized Dev Tools resource key

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
