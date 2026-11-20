# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added `AppConfig.IconPath` to replace the application icon with an `.ico` file. The icon is used for the main window, the taskbar, the tray icon, dialogs, and the desktop shortcut. Set it in `settings/AppConfig.json`; a relative path is resolved against the application root directory (the folder that contains `settings`). Changes take effect after a restart. If the file cannot be loaded, a warning is logged and the icon embedded in the executable is used.
* Added `IconTools.SetOverrideIcon` and `IconTools.OverrideIconPath` to `ZYC.Framework.Core`. `SetOverrideIcon` must be called before any window or tray icon reads the application icon.

---

## 🛠 Improvements

* The `minimal` and `react` project templates now generate `Directory.Build.props` and `Directory.Build.targets`, like the other templates. The target framework, WPF, nullable, and implicit-usings settings move out of the project file, and the `ZYC.Framework.Alpha` version is set in `Directory.Build.targets`. Once the package is restored, `Directory.Build.targets` also imports the package's dependency versions, so `ZYC.CoreToolkit.*` and other framework dependencies added later can omit `Version`.
* Added XML documentation for `DropAction`, `DropContext`, and `TabItemsPreloadCompletedEvent`.

---

## 🐛 Fixes

* Fixed the page of a project created from the `react` template showing white text on a white background when Windows uses dark mode. The page now declares `color-scheme: light`, matching the light-only host UI.

---

## 🔄 Compatibility

* Projects created from the `minimal` and `react` templates now build to `_bin/app/` instead of `bin/<Configuration>/net10.0-windows/`, and the host creates its `settings/` and `logs/` folders in `_bin/`, as in the `modular` template. Projects created with earlier versions are not affected.

---

## 📦 Installation

```bash
dotnet add package ZYC.Framework.Alpha --version $(Version)
dotnet tool install --global ZYC.Framework.CLI --version $(Version)
```

---

## 📚 Resources

* 📖 [Documentation]($(DocumentUrl))
* 🐞 [Report an Issue]($(IssuesUrl))

---

**Thank you for trying out ZYC.Framework.Alpha!**
Your feedback will help shape future releases.
