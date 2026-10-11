# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added `EmojiWpfTools.FocusZ()` and `FocusAndSelectAll()` extension methods for `Emoji.Wpf.TextBox` to focus its internal editor and optionally select all text

---

## 🛠 Improvements

* Updated sign-in, sign-out, WebView2 plugin dialog, and navigation bar commands to use the shared parameter validation in their command base classes
* Updated installation commands, project template examples, and demo download links for this release across all supported documentation languages

---

## 🐛 Fixes

* Fixed invalid parameter casts in generic command base classes when WPF passes a disconnected item while removing a bound container; null and mismatched parameters are now rejected
* Fixed `CommandBase<T>`, `AsyncCommandBase`, and both `ParameterPairCommandBase` variants executing when `CanExecute` returns `false`
* Added parameter type validation to `CanExecute` in the three-command `ParameterPairCommandBase` variant

---

## 🔄 Compatibility

* Generic command base classes now return `false` from `CanExecute` for null or mismatched parameters, including null values for reference and nullable value types; callers must supply a value matching the command's parameter type
* Direct calls to the affected `Execute` and `ExecuteAsync` methods now respect `CanExecute`; rejected calls do not run the command body or raise `Executed`
* When customizing `CommandBase<T>` or `AsyncCommandBase<T>`, override `InternalCanExecute` to retain the base class's parameter type validation

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
