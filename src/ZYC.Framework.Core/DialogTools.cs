using System.Windows.Forms;

namespace ZYC.Framework.Core;

public static class DialogTools
{
    public static string? SelectFileDialog(
        string initDirectory = "",
        string filter = "All Files(*.*)|*.*")
    {
        using var dialog = new OpenFileDialog();
        dialog.InitialDirectory = initDirectory;
        dialog.Filter = filter;
        dialog.FilterIndex = 0;
        dialog.RestoreDirectory = true;

        if (dialog.ShowDialog() != DialogResult.OK)
        {
            return null;
        }

        return dialog.FileName;
    }

    public static string? SaveFileDialog(
        string fileName,
        string initDirectory = "",
        string filter = "All Files(*.*)|*.*")
    {
        using var dialog = new SaveFileDialog();
        dialog.FileName = fileName;
        dialog.InitialDirectory = initDirectory;
        dialog.Filter = filter;
        dialog.FilterIndex = 0;
        dialog.RestoreDirectory = true;

        if (dialog.ShowDialog() != DialogResult.OK)
        {
            return null;
        }

        return dialog.FileName;
    }

    public static string? SelectFolderDialog(
        string initDirectory = "",
        string description = "Please select a folder")
    {
        using var dialog = new FolderBrowserDialog();
        dialog.InitialDirectory = initDirectory;
        dialog.ShowNewFolderButton = true;
        dialog.Description = description;

        return dialog.ShowDialog() == DialogResult.OK
            ? dialog.SelectedPath
            : null;
    }
}