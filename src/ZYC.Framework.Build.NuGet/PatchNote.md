# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added the bubbling `Component.ResolveFailed` event and `ComponentResolveFailedEventArgs` so containing views can handle component content resolution failures
* Added the read-only `ITaskManager.Initialized` property to expose the task manager's initialization state

---

## 🛠 Improvements

* Added `ZYC.Framework.Abstractions` as a standalone NuGet package alongside `ZYC.Framework.Core` in release artifacts
* Added a Mock module example for an unregistered dependency inside a `Component`
* Workspace tab preload exceptions are now logged for troubleshooting
* Updated installation commands, project template examples, and demo download links for this release across all supported documentation languages

---

## 🐛 Fixes

* Fixed deferred component resolution failures escaping tab error handling; affected tabs are now replaced with an error tab while preserving their position, tab reference, and focus when selected
* Fixed tab selection not being restored when an exception occurs during view preloading

---

## 🔄 Compatibility

* Implementations of `ITaskManager` must now provide the `Initialized` property; the built-in implementation sets it to `true` when initialization begins
* Set `Handled = true` on `Component.ResolveFailed` event arguments to consume the exception; unhandled resolution failures continue to propagate

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
