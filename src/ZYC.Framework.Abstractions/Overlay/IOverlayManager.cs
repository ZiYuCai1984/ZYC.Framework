namespace ZYC.Framework.Abstractions.Overlay;

public interface IOverlayManager
{
    IOverlay Show(object target, object? passThrough = null);
}