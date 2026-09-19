# __PROJECT_NAME__

This solution includes an application project and an independent `__PROJECT_NAME__.Build` project that manages versioning, builds, and NuGet packaging.

Run the build project from the solution directory on Windows with the .NET 10 SDK:

```powershell
dotnet run --project __PROJECT_NAME__.Build/__PROJECT_NAME__.Build.csproj -c Release
```

Set the package version in `__PROJECT_NAME__.Build/BuildEnvironment.cs`. The build project writes `version.props`, builds the publishable projects in Release configuration, and creates NuGet packages in `_bin/`. Its own output is isolated in `_bin_build/`, and it is excluded from packaging. Projects marked with `<IgnoreFromPublish>true</IgnoreFromPublish>` are excluded from the generated build solution and packaging.

The temporary `_temp.sln` is removed when the build finishes. Local builds only create packages.

To publish packages, configure a NuGet trusted publishing policy for this repository and `.github/workflows/publish-nuget-manual.yml`. Set the GitHub repository variable `NUGET_USER` to the NuGet account that owns the policy, then manually run the `publish-nuget-manual` workflow. It enables `PUSH_NUGET` to publish packages to nuget.org.
