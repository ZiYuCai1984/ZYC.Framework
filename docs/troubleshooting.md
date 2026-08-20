<p align="center">
  <a href="./troubleshooting.md">English</a> |
  <a href="./troubleshooting.ja.md">日本語</a> |
  <a href="./troubleshooting.zh-CN.md">简体中文</a> |
  <a href="./troubleshooting.zh-TW.md">繁體中文</a> |
  <a href="./troubleshooting.ko.md">한국어</a> |
</p>


# Troubleshooting

This page lists the most common failure points in a ZYC.Framework host or module project. Start from the visible symptom, then check the layer that owns that behavior.

## Quick Triage

| Symptom | First checks |
| --- | --- |
| `zyc` is not found | Install or update the global `ZYC.Framework.CLI` tool, then open a new shell and run `zyc new --help`. |
| `zyc new` fails | Check the project name, `--template`, `--output`, `--package-version`, and whether the target folder already contains files. Use `--overwrite` only when replacing generated files is intentional. |
| A module is missing | Check that the runtime DLL exists in the app directory, matches `ZYC.Framework.Modules*.dll` or is listed in `ModuleConfig.AdditionalAssemblyNames`, and contains a `ModuleBase` entrypoint. |
| A module is discovered but not loaded | Check `ModuleConfig.DisabledAssemblyNames`. Disabled modules are registered as module info but `LoadAsync` is skipped. |
| The module load error page opens | Inspect the module name, exception, and function name. The failure happened during `LoadAsync` or `AfterLoadedAsync`. |
| A menu item is missing | Check that the owning module loaded, registered the correct menu provider, and did not mark the item hidden. |
| Navigation opens Not Found | No `ITabItemFactory` matched the URI. Check route attributes, factory registration, and the URI passed to `ITabManager.NavigateAsync(...)`. |
| Navigation opens an error tab | A factory, tab item, view, or tab `LoadAsync` failed. Check the exception shown by the error tab and logs. |
| A NuGet-installed module is not active | Reinstall or update it from ModuleManager, confirm `settings/nuget.module.assets.json` was written, then restart the host. |
| Aspire resources do not appear | Check that the module registers `IExtensionResourcesProvider` or `ICommandlineResourcesProvider`, and that Aspire is enabled or started. |
| The embedded terminal fails | Confirm the terminal native DLLs are copied to the expected `runtimes` folders in the output directory. |
| Documentation changes do not appear | Edit files under `src/ZYC.Framework.Build.Doc/Templates`, then regenerate the published docs if needed. |

## CLI and Project Creation

The recommended creation flow uses the global dotnet tool:

```bash
dotnet tool install --global ZYC.Framework.CLI --version 1.4.4
dotnet tool update --global ZYC.Framework.CLI --version 1.4.4
zyc new MyCompany.Tools --template minimal
```

If `zyc` is not available after installing the tool:

- open a new terminal so the updated tool path is applied;
- run `dotnet tool list --global` and confirm `ZYC.Framework.CLI` is installed;
- run `zyc new --help` to verify that the CLI command resolves.

If project creation fails:

- use a valid dotted C# project name such as `MyCompany.Tools`;
- use `--template minimal` for a host-only project or `--template modular` for a host plus module split;
- use `--output` when the target folder should not be derived from the project name;
- use `--package-version` when the generated project must reference a specific package version;
- use `--overwrite` only when replacing existing generated files is expected.

## Module Discovery

At startup, the host discovers module assemblies from the application directory. Standard built-in module DLLs are matched by file name, and additional modules can be listed in `ModuleConfig.AdditionalAssemblyNames`:

```json
{
  "AdditionalAssemblyNames": [
    "MyCompany.Tools.dll"
  ],
  "DisabledAssemblyNames": []
}
```

If a module is not discovered:

- make sure the DLL is in the application directory;
- make sure the DLL name is either matched by the standard `ZYC.Framework.Modules*.dll` pattern or listed in `AdditionalAssemblyNames`;
- make sure the assembly contains a concrete type derived from `ModuleBase`;
- do not list only an `*.Abstractions` assembly, because abstractions projects define contracts but are not runtime module entrypoints.

If a module appears in module information but never loads:

- check whether its DLL file name is present in `DisabledAssemblyNames`;
- remember that disabled modules are still discovered, but `LoadAsync` is not called;
- remove the file name from `DisabledAssemblyNames` or re-enable it through ModuleManager, then restart if the running host does not reload that module dynamically.

## Module Load Errors

The host records load failures from two phases:

- `LoadAsync`, where the module normally registers menus, tabs, status items, and runtime services;
- `AfterLoadedAsync`, where the module can run work that depends on other loaded modules.

When the module load error page opens, use the displayed module name, function name, and exception as the starting point. `AppConfig.SuppressModuleLoadError` can suppress the page, but it does not fix the underlying failure.

Typical causes:

- a required service was not registered before the module resolved it;
- a view or tab item constructor throws during registration or startup navigation;
- a module assumes another module is enabled, but that dependency is disabled or missing;
- a local file, native DLL, or external process expected by the module is missing.

## Menus, Tabs, and Routing

Menu entries are usually registered from a module's `LoadAsync`. If a menu item is missing:

- confirm the module itself loaded without error;
- register the item under the correct provider, such as File, Tools, Extensions, About, or Settings;
- check whether the item is hidden by state or configuration;
- check ordering only after the item is visible, because priority and anchors do not create the item by themselves.

Tab navigation depends on `ITabItemFactory`. If navigation opens Not Found:

- check that the factory is registered in the loaded assembly;
- check `TabItemRoute` scheme, host, and path against the URI being navigated to;
- check factory priority when a generic route, such as a file preview route, can match before a more specific route;
- check whether a single-instance tab is reusing an existing tab instead of opening a new one.

If navigation opens an error tab, the route matched but creation or loading failed. Check the exception from the error tab and then inspect the factory, tab item constructor, view constructor, and tab `LoadAsync`.

## Workspace and Restore Timing

Startup navigation, protocol-forwarded navigation, and restore-time module actions should wait until the workspace and tab restore pipeline is ready. If a tab is opened in the wrong workspace or disappears after restore:

- run startup navigation after `TabManagerRestoreCompleted`;
- use the focused workspace when the action is user-driven;
- use an explicit workspace id when restoring or forwarding a known workspace target;
- move, create, and close tabs through `ITabManager` instead of mutating UI collections directly.

## Config and State

Concrete `IConfig` and `IState` types are loaded from the settings directory while the module assembly is registered. If settings are not read or saved:

- confirm the config or state type is concrete and lives in a loaded runtime assembly;
- confirm the module assembly is discovered before expecting its config or state type to exist;
- confirm the settings file is under the host settings directory, not the source tree;
- do not put only the contract type in an abstractions assembly and expect runtime state to be created from it.

## Single Instance and Mutex Override

The host derives the single-instance mutex id from product information unless `settings/mutex-id.override` exists. Use Tools > Override Mutex Id to create, update, or delete that file.

After changing the override, restart the host. The mutex and startup URI pipe name are created during startup, so a running process does not switch identities immediately. If side-by-side instances, startup URI forwarding, or foreground-window activation behave unexpectedly, check the current `mutex-id.override` file first.

## NuGet Modules

ModuleManager installs NuGet modules by restoring a temporary project and writing the resolved runtime asset graph to `settings/nuget.module.assets.json`. The host reads that file during the next startup.

If a NuGet module was installed but is not active:

- check that restore succeeded and the assets file exists under `settings`;
- restart the host so startup discovery can load the runtime assemblies;
- confirm the package contains a compatible runtime assembly for the host target, currently `net10.0-windows`;
- check whether the installed module assembly is disabled in `ModuleConfig.DisabledAssemblyNames`;
- reinstall or remove and install again if the assets file points to stale package content.

If a known package does not appear in search results, remember that NuGet search runs before `IncludeRegex`. A package missing from the returned NuGet page never reaches the regex filter. Check `NuGetModuleConfig.SearchTerm`, `SearchSkip`, and `SearchTake`; `SearchTake` is clamped to the NuGet.org single-request limit of 1000, so use `SearchSkip` for later pages.

Install, uninstall, and refresh share the same module-assets pipeline and are serialized by the ModuleManager operation coordinator. If those commands appear disabled, wait for the current restore/search operation to finish before starting another one.

## Aspire and Sidecar Resources

Aspire resources are contributed by modules through extension providers. A module can register `IExtensionResourcesProvider` for direct Aspire builder customization, or `ICommandlineResourcesProvider` for a command-line sidecar resource.

If a resource does not appear:

- confirm the providing module loaded before Aspire builds the resource graph;
- confirm the provider type is registered in the module assembly;
- for command-line resources, confirm the resource name, working directory, and command are valid;
- check whether `AspireConfig.AutoStart` is disabled and start Aspire manually if needed.

If the Aspire dashboard cannot open, check that the Aspire process produced `ASPNETCORE_URLS` and `AppHost:BrowserToken`. The dashboard URI is built from those values.

## CLI Terminal Native Dependencies

The CLI module loads terminal native dependencies from the application output. If the embedded terminal fails early, confirm these files exist:

```text
runtimes\win10-x64\native\conpty.dll
runtimes\win-x64\native\Microsoft.Terminal.Control.dll
```

If they are missing, inspect the package output and copy-local behavior for the CLI module and terminal dependency.

## Documentation Templates

The documentation source used by `ZYC.Framework.Build.Doc` lives under:

```text
src\ZYC.Framework.Build.Doc\Templates
```

If editing a generated root `docs` file appears to work locally but disappears later, move the change into the matching template file and regenerate the docs.
