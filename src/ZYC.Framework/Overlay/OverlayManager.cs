using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Overlay;

namespace ZYC.Framework.Overlay;

[RegisterSingleInstanceAs(typeof(IOverlayManager))]
internal class OverlayManager : IOverlayManager
{
    public OverlayManager(IMainWindow mainWindow)
    {
        MainWindow = mainWindow;
    }

    private IMainWindow MainWindow { get; }

    public IOverlay Show(object target, object? passThrough = null)
    {
        return Show([target], passThrough);
    }

    public IOverlay Show(object[] targets, object? passThrough = null)
    {
        if (targets == null)
        {
            throw new ArgumentNullException(nameof(targets));
        }

        var targetElements = targets.Cast<UIElement>().ToArray();
        UIElement? passThroughElement = null;
        if (passThrough != null)
        {
            passThroughElement = (UIElement)passThrough;
        }

        var guideOverlay = new GuideOverlay((Window)MainWindow.GetMainWindow());
        guideOverlay.Show(targetElements, passThroughElement);

        return guideOverlay;
    }
}
