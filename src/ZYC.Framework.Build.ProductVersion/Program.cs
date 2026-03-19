using ZYC.Framework.Build.Utilities;

namespace ZYC.Framework.Build.ProductVersion;

internal static class Program
{
    public static void Main()
    {
        BuildEnvironment.UpdateVersionProps();
    }
}