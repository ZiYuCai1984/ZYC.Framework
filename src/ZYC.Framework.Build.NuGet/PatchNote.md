# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

None

---

## 🛠 Improvements

* Removed explicit UI Automation names from the Coming soon placeholder icon and the main menu overflow button
* Updated installation commands, project template examples, and demo download links for this release across all supported documentation languages

---

## 🐛 Fixes

* Fixed MCP initialization and tool requests failing with HTTP 500 (`MissingMethodException`) when the MCP Server and Aspire modules loaded incompatible ModelContextProtocol assemblies

## 🔄 Compatibility

* Updated `ModelContextProtocol.AspNetCore` to `1.3.0` to match the ModelContextProtocol dependency used by Aspire `13.5.4`
* The MCP SDK version is now configured through `V_ModelContextProtocol` alongside `V_Aspire` in `nuget.props`; keep these versions compatible when updating dependencies

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
