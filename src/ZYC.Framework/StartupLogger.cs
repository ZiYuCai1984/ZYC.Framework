using System.IO;
using System.Text;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework;

/// <summary>
///     Used to record startup logs, instantiated before dependency injection container construction
/// </summary>
internal class StartupLogger : IAppLogger
{
    private readonly object _lock = new();
    private readonly string _logFilePath;

    private StartupLogger()
    {
        _logFilePath = GetOrCreateLogFile();
    }

    private static string LogDirectory { get; } = Path.Combine(
        AppContext.GetMainAppDirectory(),
        "logs");


    public void Debug(string message)
    {
        WriteLog("DEBUG", message);
    }

    public void Info(string message)
    {
        WriteLog("INFO", message);
    }

    public void Warn(string message)
    {
        WriteLog("WARN", message);
    }

    public void Error(string message)
    {
        WriteLog("ERROR", message);
    }

    public void Error(Exception exception)
    {
        WriteLog("ERROR", exception.ToString());
    }

    private static string GetOrCreateLogFile()
    {
        if (!Directory.Exists(LogDirectory))
        {
            Directory.CreateDirectory(LogDirectory);
        }

        var flag = StartupTarget.Main;
        if (AppContext.IsSelfAlternate())
        {
            flag = StartupTarget.Alternate;
        }

        var logFileName = $"{flag}_startup_{DateTime.Now:yyyyMMdd_HHmmss}.log";


        var filePath = Path.Combine(LogDirectory, logFileName);
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, $"[Startup Log Created at {DateTime.Now}]{Environment.NewLine}");
        }

        return filePath;
    }

    private void WriteLog(string level, string message)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}{Environment.NewLine}";

        lock (_lock)
        {
            File.AppendAllText(_logFilePath, logEntry, Encoding.UTF8);
        }
    }

    public static StartupLogger CreateInstance()
    {
        return new StartupLogger();
    }
}