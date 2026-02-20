using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Modules.WebBrowser.Abstractions;

public interface IWebTabItemInstance : ITabItemInstance
{
    void SetTitle(string title);

    void SetIcon(string icon);

    Task TabInternalNavigatingAsync(Uri newUri);
}