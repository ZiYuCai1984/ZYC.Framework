using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.WebBrowser.Abstractions;
using ZYC.Framework.Modules.WebBrowser.UI;

namespace ZYC.Framework.Modules.WebBrowser;

[Register]
[ConstantsSource(typeof(WebBrowserModuleConstants))]
internal class WebBrowserTabItem : TabItemInstanceBase, IWebTabItemInstance, INotifyPropertyChanged
{
    public WebBrowserTabItem(
        ILifetimeScope lifetimeScope,
        ITabManager tabManager, MutableTabReference tabReference) : base(lifetimeScope, tabReference)
    {
        TabManager = tabManager;
        Title = tabReference.Uri.Host;
    }

    private MutableTabReference MutableTabReference => (MutableTabReference)TabReference;

    private ITabManager TabManager { get; }

    public Uri Uri
    {
        get => MutableTabReference.Uri;
        set => MutableTabReference.Uri = value;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public new string Icon { get; set; } = WebBrowserModuleConstants.MenuIcon;

    public override bool Localization => false;

    public new string Title { get; private set; }

    public override object View => _view ??= LifetimeScope.Resolve<WebBrowserView>(
        new TypedParameter(typeof(Uri), Uri),
        new TypedParameter(typeof(IWebTabItemInstance), this));

    public void SetTitle(string title)
    {
        Title = title;
        OnPropertyChanged(nameof(Title));
    }

    public void SetIcon(string icon)
    {
        Icon = icon;
        OnPropertyChanged(nameof(Icon));
    }

    public async Task TabInternalNavigatingAsync(Uri newUri)
    {
        var oldUri = Uri;
        Uri = newUri;

        if (UriTools.Equals(oldUri, newUri))
        {
            return;
        }

        await TabManager.TabInternalNavigatingAsync(oldUri, newUri);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}