# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added a **Localization Resources** page for inspecting, filtering, and editing language resource values
* Added **ILanguageResourcesManager** for retrieving and updating editable localization entries
* Added **LanguageResourceEntry** and **OverrideDefaultLanguageResourcesConfig** to support persisted resource overrides
* Added a Settings menu entry and tab route for **Localization Resources**
* Added shared selection styling for **DataGrid** and **ComboBox** controls

---

## 🛠 Improvements

* Updated documentation, quick-start samples, installer links, and package references to **1.2.7**
* Updated core dependency versions including **ZYC.CoreToolkit**, **Aspire**, **NuGet**, **WPFHexaEditor**, **log4net**, and **Namotion.Reflection**
* Improved the Language module menu structure with a stable **Language** anchor and localized **Localization Resources** labels
* Saved generated and manually edited language resources to the application settings directory
* Let the CLI project use the standard output path by default

---

## 🐛 Bug Fixes

* Kept built-in default language resources read-only and redirected runtime changes to override resources
* Treated English localization entries as source keys so they are visible but not editable
* Delayed **ShowInTaskbar** change observation until the main window is loaded
* Improved selected and hovered row readability for grid and combo box selection states

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
