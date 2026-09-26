# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added `HexEditor`: open binary files through **File → Open → Binary File** in dedicated tabs, with hexadecimal/ASCII views, byte editing, undo/redo, and search/replace
* Added Save, Save As, Reload, `Ctrl+S` / `Ctrl+Shift+S`, unsaved-change prompts, read-only file handling, and external file-change detection for binary documents
* Added `HexEditorModuleConstants.CreateEditorUri(Uri)` so other modules can navigate directly to a binary editor

---

## 🛠 Improvements

* Updated Aspire to 13.5.4, WebView2 to 1.0.4191.47, System.Reactive to 7.0.0, and WPFHexaEditor to 3.4.5
* Included the framework version in the CLI root-command description
* Updated the multilingual README, built-in module, and extension-point templates for this release

---

## 🐛 Fixes

* Added host-theme resources for HexEditor search dialogs to correct unreadable backgrounds, text, and title-bar buttons; used the framework's keyed-resource extension to avoid the `StaticResourceHolder` XAML loading exception
* Fixed a compilation error in the NuGet publishing project

---

## 🔄 Compatibility

* `ReactiveExtensions.ObserveProperty<T>(...)` now returns `IObservable<T>` instead of `IObservable<System.Reactive.Unit>`, emitting the `PropertyChanged` event sender. Rebuild dependent modules and update explicitly typed observers or pipelines. Use `.Select(_ => System.Reactive.Unit.Default)` when a downstream API still requires a `Unit` stream
* `ObserveAnyChange<T>()` also emits the `PropertyChanged` event sender

---

## 📝 HexEditor Limitations

* Files are loaded entirely into memory; saving or reloading resets undo/redo history

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
