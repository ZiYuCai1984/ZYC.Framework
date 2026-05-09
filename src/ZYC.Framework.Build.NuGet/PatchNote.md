# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added the built-in **TextEditor** module to the release package
* Added read-only text file preview with an **Edit** action for switching into editor mode
* Added text editing support with reload, save, save-as, dirty-state tracking, and unsaved-change confirmation
* Added syntax highlighting for common text, code, markup, project, script, and configuration files
* Added encoding-aware text document snapshots for reliable text loading and saving

---

## 🛠 Improvements

* Updated product version, documentation, quick-start samples, installer links, and package references to **1.2.8**
* Updated **Aspire** tooling dependency to **13.3.0**
* Added **System.Text.Encoding.CodePages** for Windows legacy code page support in the TextEditor module
* Preserved detected text encoding and BOM style when saving edited files
* Displayed detected encoding in TextEditor preview and editor status text
* Refined TextEditor labels and icons for separate **Preview** and **Editor** surfaces
* Refactored TextEditor file selection into a reusable command with centralized exception notification and logging
* Changed **IconButton** to derive from **Button**, preserving standard command, style, focus, and button behavior
* Added application-level default styling for **IconButton**

---

## 🐛 Bug Fixes

* Restored the **File > Open** submenu registration so file-open commands are visible from the File menu
* Prevented unsupported characters from being silently replaced when saving with a non-Unicode text encoding
* Improved TextEditor watcher, reload, edit, read, and save exception handling with logged errors and user-facing notifications
* Replaced debugger breaks and raw exception toast calls with centralized **PromptException** handling in Aspire, Localization Resources, and workspace drag-drop flows

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
