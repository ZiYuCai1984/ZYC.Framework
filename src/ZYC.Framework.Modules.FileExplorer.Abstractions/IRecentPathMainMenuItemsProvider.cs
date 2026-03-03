using ZYC.Framework.Abstractions.MainMenu;

namespace ZYC.Framework.Modules.FileExplorer.Abstractions;

/// <summary>
///     Provides functionality to manage and retrieve recent file or folder paths
///     within the main menu system.
/// </summary>
public interface IRecentPathMainMenuItemsProvider : IMainMenuItemsProvider
{
    /// <summary>
    ///     Adds a new path to the list of recently accessed items.
    /// </summary>
    /// <param name="path">The full string path of the file or directory to add.</param>
    void AddRecentPath(string path);

    /// <summary>
    ///     Retrieves an array of all currently stored recent paths.
    /// </summary>
    /// <returns>An array of strings representing the recent paths, typically ordered by recency.</returns>
    string[] GetRecentPaths();
}