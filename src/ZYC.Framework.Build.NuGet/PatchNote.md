# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added release artifacts and documentation references for ZYC.Framework `1.3.3`

---

## 🛠 Improvements

* Improved URI-based tab focusing so every matching tab instance can be brought into focus instead of stopping at the first match
* Hardened background tab navigation by ignoring null tab descriptors during navigation startup
* Made navigation history updates resilient by logging history update failures instead of letting them interrupt tab navigation
* Improved tab drag feedback by forcing the drag adorner to repaint during the OLE drag/drop message loop

---

## 📚 Documentation

* Updated README installer links from `1.3.2` to `1.3.3`
* Updated CLI installation, CLI update, project template, and package reference examples from `1.3.2` to `1.3.3`
* Applied the same version updates across English, Japanese, Korean, Simplified Chinese, and Traditional Chinese documentation

---

## 🐛 Bug Fixes

* Fixed tab drag preview refresh behavior that could fail to repaint correctly in release builds
* Fixed URI focus behavior for multiple open tab instances that share the same route

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
