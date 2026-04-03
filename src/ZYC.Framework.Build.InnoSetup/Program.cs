using System.IO;
using System.Text;
using System.Text.Json;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions;
using ZYC.CoreToolkit.Dotnet;
using ZYC.Framework.Build.Utilities;
using ProductInfo = ZYC.Framework.Abstractions.ProductInfo;
using ProductInfoExtended = ZYC.Framework.Abstractions.ProductInfoExtended;

namespace ZYC.Framework.Build.InnoSetup;

internal class Program
{
    private const string InnoSetupPackageName = "Tools.InnoSetup";
    private const string InnoSetupPackageVersion = "6.3.1";

    private static async Task Main()
    {
        await ZYC.Framework.Build.NuGet.Program.Main();

        IOTools.SetCurrentDirectoryToEntryScriptFileDirectory();

        var setupFilePath = await BuildSetupAsync();

#if PUBLISH_GITHUB_RELEASE
        await PublishSetupToGitHubReleaseAsync(setupFilePath);
#endif
    }

    private static async Task<string> BuildSetupAsync()
    {
        IOTools.DeleteDirectoryIfExists("packages");

        await DotnetNuGetTools.DownloadNuGetPackagesAsync(new NuGetPackage
        {
            Name = InnoSetupPackageName,
            Version = InnoSetupPackageVersion
        });

        var toolFolder = $"packages/{InnoSetupPackageName}/{InnoSetupPackageVersion}/tools";

        IOTools.CopyFile("../ZYC.Framework/app.ico", $"{toolFolder}/app.ico");
        IOTools.CopyDirectory("../_bin", $"{toolFolder}/_bin");
        IOTools.CopyFile("./app.iss", $"{toolFolder}/app.iss");

        IOTools.SetCurrentDirectory(toolFolder);

        var command =
            $"iscc.exe app.iss /DVersion=\"{ProductInfo.Version}\" /DCopyright=\"{ProductInfo.Copyright}\" /DAuthor=\"{ProductInfo.Author}\"";

        var buildResult = await CommandTools.ExecuteCommandAsync(command);
        if (buildResult != 0)
        {
            throw new InvalidOperationException("Setup build failed.");
        }

        var setupFilePath = BuildEnvironment.SetupFilePath;
        File.Copy("./Output/ZYC.Framework.Setup.exe", setupFilePath, true);
        return setupFilePath;
    }

#if PUBLISH_GITHUB_RELEASE
    private static async Task PublishSetupToGitHubReleaseAsync(string setupFilePath)
    {
        EnsureGitHubToken();

        var releaseTag = GetRequiredReleaseTag();
        ValidateReleaseTag(releaseTag);

        var repository = GetGitHubRepository();
        IOTools.SetCurrentDirectory(BuildEnvironment.RootFolder);

        var releaseNotesFilePath = WriteReleaseNotesFile();

        try
        {
            await EnsureGitHubReleaseExistsAsync(repository, releaseTag, releaseNotesFilePath);
        }
        finally
        {
            IOTools.DeleteFileIfExists(releaseNotesFilePath);
        }

        var uploadCommand =
            $"gh release upload \"{releaseTag}\" \"{setupFilePath}\" --repo \"{repository}\" --clobber";

        var uploadResult = await CommandTools.ExecuteCommandAsync(uploadCommand);
        if (uploadResult != 0)
        {
            throw new InvalidOperationException(
                $"Setup upload failed for repository '{repository}' and release '{releaseTag}'.");
        }

        Console.WriteLine($"Uploaded setup to release '{releaseTag}'.");
    }

    private static string GetRequiredReleaseTag()
    {
        var releaseTag = TryGetExplicitReleaseTag();
        if (!string.IsNullOrWhiteSpace(releaseTag))
        {
            return releaseTag;
        }

        throw new InvalidOperationException(
            "Release tag is required. Provide the workflow_dispatch tag input or run from a release/tag context.");
    }

    private static async Task EnsureGitHubReleaseExistsAsync(
        string repository,
        string releaseTag,
        string releaseNotesFilePath)
    {
        var viewCommand = $"gh release view \"{releaseTag}\" --repo \"{repository}\"";
        var viewResult = await CommandTools.ExecuteCommandAsync(viewCommand);
        if (viewResult == 0)
        {
            var editCommand =
                $"gh release edit \"{releaseTag}\" --repo \"{repository}\" --title \"{releaseTag}\" --notes-file \"{releaseNotesFilePath}\"";

            var editResult = await CommandTools.ExecuteCommandAsync(editCommand);
            if (editResult != 0)
            {
                throw new InvalidOperationException(
                    $"GitHub release '{releaseTag}' exists, but updating release notes failed.");
            }

            return;
        }

        var target = Environment.GetEnvironmentVariable("GITHUB_SHA");
        var targetArgument = string.IsNullOrWhiteSpace(target)
            ? string.Empty
            : $" --target \"{target}\"";

        var createCommand =
            $"gh release create \"{releaseTag}\" --repo \"{repository}\" --title \"{releaseTag}\" --notes-file \"{releaseNotesFilePath}\"{targetArgument}";

        var createResult = await CommandTools.ExecuteCommandAsync(createCommand);
        if (createResult != 0)
        {
            throw new InvalidOperationException(
                $"GitHub release '{releaseTag}' was not found and automatic creation failed.");
        }
    }

    private static string? TryGetExplicitReleaseTag()
    {
        var eventTag = ReadReleaseTagFromGitHubEvent();
        if (!string.IsNullOrWhiteSpace(eventTag))
        {
            return eventTag;
        }

        var envTag = Environment.GetEnvironmentVariable("RELEASE_TAG");
        if (!string.IsNullOrWhiteSpace(envTag))
        {
            return envTag;
        }

        var refType = Environment.GetEnvironmentVariable("GITHUB_REF_TYPE");
        var refName = Environment.GetEnvironmentVariable("GITHUB_REF_NAME");
        if (string.Equals(refType, "tag", StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(refName))
        {
            return refName;
        }

        return null;
    }

    private static string? ReadReleaseTagFromGitHubEvent()
    {
        var githubEventPath = Environment.GetEnvironmentVariable("GITHUB_EVENT_PATH");
        if (string.IsNullOrWhiteSpace(githubEventPath) || !File.Exists(githubEventPath))
        {
            return null;
        }

        using var stream = File.OpenRead(githubEventPath);
        using var document = JsonDocument.Parse(stream);
        var root = document.RootElement;

        if (root.TryGetProperty("release", out var releaseElement) &&
            releaseElement.ValueKind == JsonValueKind.Object &&
            releaseElement.TryGetProperty("tag_name", out var releaseTagElement))
        {
            return releaseTagElement.GetString();
        }

        if (root.TryGetProperty("inputs", out var inputsElement) &&
            inputsElement.ValueKind == JsonValueKind.Object &&
            inputsElement.TryGetProperty("tag", out var inputTagElement))
        {
            return inputTagElement.GetString();
        }

        return null;
    }

    private static void ValidateReleaseTag(string releaseTag)
    {
        if (!releaseTag.StartsWith('v'))
        {
            throw new InvalidOperationException(
                $"Release tag '{releaseTag}' must start with 'v' (for example: 'v{ProductInfo.Version}').");
        }

        var normalizedTag = releaseTag.TrimStart('v');
        if (!string.Equals(normalizedTag, ProductInfo.Version, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Release tag '{releaseTag}' does not match ProductInfo.Version '{ProductInfo.Version}'.");
        }
    }

    private static void EnsureGitHubToken()
    {
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GH_TOKEN")) ||
            !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GITHUB_TOKEN")))
        {
            return;
        }

        throw new InvalidOperationException(
            "GH_TOKEN or GITHUB_TOKEN is required when publishing the setup release.");
    }

    private static string GetGitHubRepository()
    {
        var repository = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
        if (!string.IsNullOrWhiteSpace(repository))
        {
            return repository;
        }

        if (!Uri.TryCreate(ProductInfoExtended.Repository, UriKind.Absolute, out var repositoryUri))
        {
            throw new InvalidOperationException("Could not resolve the GitHub repository.");
        }

        var segments = repositoryUri.AbsolutePath.Trim('/').Split('/');
        if (segments.Length < 2)
        {
            throw new InvalidOperationException("Could not resolve the GitHub repository.");
        }

        return $"{segments[0]}/{segments[1]}";
    }

    private static string WriteReleaseNotesFile()
    {
        var releaseNotesFilePath = Path.Combine(
            Path.GetTempPath(),
            $"ZYC.Framework.ReleaseNotes.{Guid.NewGuid():N}.md");

        File.WriteAllText(releaseNotesFilePath, PatchNoteTools.GetPatchNote(), new UTF8Encoding(false));
        return releaseNotesFilePath;
    }
#endif
}
