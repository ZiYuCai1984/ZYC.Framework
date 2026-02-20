using System.Diagnostics;
using System.IO;
using System.Text;

namespace ZYC.Framework.CLI;

public static class FileReplaceTools
{
    public static void SafeCopyWithDelayedReplace(string sourceDir, string targetDir)
    {
        var currentExe = Process.GetCurrentProcess().MainModule!.FileName;
        var pendingReplaces = new List<(string Source, string Target)>();

        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourceDir, file);
            var targetPath = Path.Combine(targetDir, relativePath);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                File.Copy(file, targetPath, true);
                Console.WriteLine($"Copied: {relativePath}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"⚠️ Copy failed: {relativePath} → {ex.Message}");
                pendingReplaces.Add((file, targetPath));
            }
        }

        if (pendingReplaces.Any())
        {
            var batPath = Path.Combine(targetDir, "apply_update.bat");
            GenerateDelayedUpdateScript(pendingReplaces, currentExe, batPath);
            Console.WriteLine("✅ Delayed update script generated.");
        }
        else
        {
            Console.WriteLine("✅ All files copied successfully. No delayed updates required.");
        }
    }

    private static void GenerateDelayedUpdateScript(List<(string Source, string Target)> pendingReplaces,
        string currentExe, string batPath)
    {
        using var writer = new StreamWriter(batPath, false, Encoding.Default);

        writer.WriteLine("@echo off");
        writer.WriteLine("echo Applying pending updates...");
        writer.WriteLine("timeout /t 1 >nul");

        foreach (var (src, dst) in pendingReplaces)
        {
            writer.WriteLine($"echo Updating {Path.GetFileName(dst)}");
            writer.WriteLine($"copy /Y \"{src}\" \"{dst}\" >nul");
        }

        writer.WriteLine($"start \"\" \"{currentExe}\"");
        writer.WriteLine("echo Cleanup...");
        writer.WriteLine("del \"%~f0\"");

        writer.WriteLine("exit");
    }
}