using ZYC.CoreToolkit;

namespace ZYC.Framework.Build.NuGet.SharedSources
{
    public static class Entry
    {
        public static async Task Main()
        {
            await CommandTools.ExecuteProgramAsync("ZYC.Framework.exe", "");
        }
    }
}
