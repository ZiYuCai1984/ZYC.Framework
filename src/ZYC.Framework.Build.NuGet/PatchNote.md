# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added a responsive main menu that automatically moves items into a `More` (`⋯`) menu when space is limited and restores them as the window expands

---

## 🛠 Improvements

* Improved menu behavior during resizing by preserving open submenus for unchanged items and restoring keyboard focus when items move into or out of the overflow menu
* Adjusted the title-bar layout to reserve space for quick-access items, title extensions, and window actions while allowing the main menu to adapt to the remaining width
* Changed the default value of `WorkspaceConfig.IsWorkspaceEmptyIndexVisible` to `false`, hiding the large workspace index when a workspace has no tabs; set it to `true` to show the index
* Updated multilingual download links, installation commands, quick-start guides, project-template examples, and troubleshooting documentation for version 1.4.7

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
