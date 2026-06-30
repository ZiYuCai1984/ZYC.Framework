# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added a provider-based Accounts module with session management, sign-in/sign-out commands, and protected token storage
* Added a window-title account menu that displays the current account state and exposes provider sign-in/sign-out actions
* Added GitHub account sign-in through an internal WebView2 OAuth flow with PKCE, state/nonce validation, redirect relay support, and callback interception
* Added a dedicated GitHub account module and configuration surface for client id, redirect URI, deep-link URI, scopes, API version, and token exchange endpoint settings

---

## 🛠 Improvements

* Moved the GitHub account provider into its own module so more account providers can be added without coupling them to the core Accounts module
* Added a reusable top-right drop-down button style and updated the window-title extension host for richer embedded controls
* Refined toast and account-menu presentation to better match the compact title-bar UI
* Updated package metadata, documentation, CLI examples, project template references, and installer links for version 1.3.8

---

## 🐛 Bug Fixes

* Kept GitHub OAuth completion inside the application by intercepting configured deep-link callbacks from WebView2 navigation, external URI launches, and new-window requests
* Improved GitHub token exchange validation so missing direct secrets or server-side exchange endpoints fail with clearer configuration errors
* Removed obsolete account-menu click handling and unused dependencies after moving the UI to command-backed drop-down items
* Kept banner popup hosting extensible for framework extension points

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
