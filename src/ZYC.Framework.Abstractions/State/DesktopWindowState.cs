using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.State;

#pragma warning disable CS1591

public class DesktopWindowState : IState
{
    public int Width { get; set; } = 800;

    public int Height { get; set; } = 600;

    public int Top { get; set; } = 150;

    public int Left { get; set; } = 300;

    public bool Topmost { get; set; } = false;

    public WindowState WindowState { get; set; } = WindowState.Normal;

    public bool IsFrozen { get; set; }

    public bool IsPreventExit { get; set; }
}