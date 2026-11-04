# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added a built-in MCP Inspector under the MCP Server menu to browse and search grouped tools, inspect input schemas, execute tools with JSON arguments, and view formatted or raw results with execution timing.
* Added `MCPServerConfig.ExcludedToolsPattern` to exclude auto-discovered MCP tools by a case-insensitive regular expression matched against tool names.
* Added `zyc update` to update the globally installed `ZYC.Framework.CLI` tool to the latest version.
* Added `MCPServiceStatusChangedEvent` for subscribing to MCP service status changes.

---

## 🛠 Improvements

* MCP Inspector includes server start/stop controls, endpoint copying, tool refresh, and JSON argument templates generated from input schemas.
* On Windows, `zyc update` starts the update in the background and exits to release its own tool files.

---

## 🐛 Fixes

* Fixed actions in `Expander` headers also toggling the details. Expansion is now controlled by a dedicated chevron button.

---

## 🔄 Compatibility

* Tool exclusion is disabled by default. Changes to `ExcludedToolsPattern` take effect when the MCP server starts; restart a running server to apply them.
* **Binary compatibility:** `MCPServer.StartAsync` and `MCPAutoToolDiscoveryExtensions.AddAutoDiscoveredTools` now include an optional `excludedToolsPattern` parameter. Existing source calls remain valid, but assemblies compiled against the previous signatures must be recompiled.

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
