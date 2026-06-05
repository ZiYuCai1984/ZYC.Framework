using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Modules.ChromeExtensions;

[RegisterSingleInstance]
internal class ChromeExtensionPackageStoreEvents
{
    public event EventHandler? InstalledExtensionsChanged;

    public void RaiseInstalledExtensionsChanged()
    {
        InstalledExtensionsChanged?.Invoke(this, EventArgs.Empty);
    }
}
