namespace ZYC.Framework.Core.Page;

public class NavigateProxyParameter
{
    public NavigateProxyParameter(
        string content,
        string caption,
        string buttonText,
        Uri source,
        Uri target,
        Func<bool> canNavigateFunc,
        Action? navigatedCallback = null,
        bool localization = true)
    {
        Content = content;
        Caption = caption;
        ButtonText = buttonText;
        Source = source;
        Target = target;
        CanNavigateFunc = canNavigateFunc;
        NavigatedCallback = navigatedCallback;
        Localization = localization;
    }

    public string Content { get; }

    public string Caption { get; }

    public string ButtonText { get; }

    public Uri Source { get; }

    public Uri Target { get; }

    public Func<bool> CanNavigateFunc { get; }

    public Action? NavigatedCallback { get; }

    public bool Localization { get; }
}