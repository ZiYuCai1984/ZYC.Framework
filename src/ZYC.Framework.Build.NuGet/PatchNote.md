# 🚀 Release Notes - Version $(Version)

**Release Date:** $(ReleaseDate)

---

## 🆕 New Features

* Added the `build` project template: `zyc new MyProject --template build` creates an independent `MyProject.Build` project for version generation, compilation, and NuGet packaging, with a manual publishing workflow and setup instructions
* Added `zyc clear-nuget-http-cache`, which directly runs `dotnet nuget locals http-cache --clear` and returns its exit code

---

## 🛠 Improvements

* Updated `ZYC.CoreToolkit` to 4.0.4 and made project templates use the CLI's CoreToolkit dependency version through `__V_ZYC_CORETOOLKIT__`
* Renamed the framework package-version token from `__PACKAGE_VERSION__` to `__V_ZYC_FRAMEWORK_ALPHA__` across the generator, project templates, and documentation
* Added token replacement for `.yaml` and `.yml` files so generated workflows reference the selected project name
* Updated multilingual installation guides, project-template documentation, and examples for this release

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
