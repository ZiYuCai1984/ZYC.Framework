# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added a **Save Custom Layout** dialog with editable layout names and generated workspace-layout thumbnails
* Added a **Manage Custom Layouts** dialog for renaming, deleting, and reordering saved workspace layouts
* Added the bundled **ZYC.Indigo.Light** application theme based on MahApps.Metro resources
* Added a reusable static-resource markup extension for theme resource aliases

---

## 🛠 Improvements

* Reworked the **View** menu layout commands with explicit ordering and a dedicated custom-layout management entry
* Moved custom-layout thumbnail generation into a shared builder used by the save and management flows
* Updated custom-layout apply notifications to include the selected layout name
* Sorted TaskManager entries by creation time with newest tasks first
* Removed obsolete placeholder and duplicate custom-layout removal menu types
* Removed the TextEditor module's direct **System.Text.Encoding.CodePages** package reference now that the package version is centrally managed

---

## 🐛 Bug Fixes

* Fixed CLI startup URI overrides so non-empty startup command values are applied to the terminal control
* Kept toast popups visible while owned dialog windows are active
* Preserved the original accent brush for main-window borders and active tab indicators after switching to the custom theme

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
