using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.Mock.Abstractions;

// ReSharper disable StringLiteralTypo
#pragma warning disable CS1591
[AddINotifyPropertyChangedInterface]
public class TestHybridIconPageState : IState
{
    public string IconData { get; set; } =
        "data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjx0ZXh0IHk9IjIwIj4weDwvdGV4dD48L3N2Zz4=";

    public int Width { get; set; } = 96;

    public int Height { get; set; } = 96;

    public double Ratio { get; set; } = 0.5;
}