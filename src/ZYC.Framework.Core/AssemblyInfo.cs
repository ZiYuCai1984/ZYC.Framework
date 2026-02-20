// ReSharper disable all

using System.Reflection;
using System.Runtime.CompilerServices;



#if DEBUG

#else
using System.Windows.Markup;

[assembly: XmlnsPrefix("https://github.com/ZiYuCai1984", "zyc")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.Bindings")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.Buttons")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.Commands")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.Converters")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.DragDrop")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.Localizations")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.MarkupEx")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.Menu")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.MenuItems")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.Notification")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.Notification.Banner")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.Notification.Toast")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.Page")]
[assembly: XmlnsDefinition("https://github.com/ZiYuCai1984", "ZYC.Framework.Core.Resources")]

#endif


[assembly: InternalsVisibleTo("ZYC.Framework")]
[assembly: InternalsVisibleTo("ZYC.Framework.CLI")]


namespace ZYC.Framework.Core;

public static class AssemblyInfo
{
    public static Assembly GetAssembly()
    {
        return typeof(AssemblyInfo).Assembly;
    }
}