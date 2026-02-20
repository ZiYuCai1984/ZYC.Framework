namespace ZYC.Framework.Abstractions.Config;

public static class AppConfigEx
{
    public static bool GetIsDebugItemVisible(this AppConfig config)
    {
#if DEBUG
        return true;
#endif
        // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable CS0162 // Unreachable code detected
        return config.DebugMode;
#pragma warning restore CS0162 // Unreachable code detected
    }
}