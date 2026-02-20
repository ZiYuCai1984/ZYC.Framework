using System.Reflection;

namespace ZYC.Framework.MetroWindow;

public static class AssemblyInfo
{
    public static Assembly GetAssembly()
    {
        return typeof(AssemblyInfo).Assembly;
    }
}