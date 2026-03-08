using System.Windows;
using System.Windows.Media.Effects;

namespace ZYC.Framework.Modules.Mock.Shaders;

internal sealed class CrtEffect : ShaderEffect
{
    // ReSharper disable once InconsistentNaming
    private static readonly PixelShader _pixelShader = new()
    {
        UriSource = new Uri(
            "/ZYC.Framework.Modules.Mock.Shaders;component/Shaders/crt.ps",
            UriKind.Relative)
    };

    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            nameof(Input), typeof(CrtEffect), 0);

    public static readonly DependencyProperty TimeProperty =
        DependencyProperty.Register(
            nameof(Time), typeof(double), typeof(CrtEffect),
            new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

    public static readonly DependencyProperty DistortionProperty =
        DependencyProperty.Register(
            nameof(Distortion), typeof(double), typeof(CrtEffect),
            new UIPropertyMetadata(0.05, PixelShaderConstantCallback(1)));

    public static readonly DependencyProperty ScanlineIntensityProperty =
        DependencyProperty.Register(
            nameof(ScanlineIntensity), typeof(double), typeof(CrtEffect),
            new UIPropertyMetadata(0.10, PixelShaderConstantCallback(2)));

    public static readonly DependencyProperty NoiseAmountProperty =
        DependencyProperty.Register(
            nameof(NoiseAmount), typeof(double), typeof(CrtEffect),
            new UIPropertyMetadata(0.03, PixelShaderConstantCallback(3)));

    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register(
            nameof(Width), typeof(double), typeof(CrtEffect),
            new UIPropertyMetadata(800.0, PixelShaderConstantCallback(4)));

    public static readonly DependencyProperty HeightProperty =
        DependencyProperty.Register(
            nameof(Height), typeof(double), typeof(CrtEffect),
            new UIPropertyMetadata(600.0, PixelShaderConstantCallback(5)));

    public CrtEffect()
    {
        PixelShader = _pixelShader;

        UpdateShaderValue(InputProperty);
        UpdateShaderValue(TimeProperty);
        UpdateShaderValue(DistortionProperty);
        UpdateShaderValue(ScanlineIntensityProperty);
        UpdateShaderValue(NoiseAmountProperty);
        UpdateShaderValue(WidthProperty);
        UpdateShaderValue(HeightProperty);
    }

    public object Input
    {
        get => GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }

    public double Time
    {
        get => (double)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public double Distortion
    {
        get => (double)GetValue(DistortionProperty);
        set => SetValue(DistortionProperty, value);
    }

    public double ScanlineIntensity
    {
        get => (double)GetValue(ScanlineIntensityProperty);
        set => SetValue(ScanlineIntensityProperty, value);
    }

    public double NoiseAmount
    {
        get => (double)GetValue(NoiseAmountProperty);
        set => SetValue(NoiseAmountProperty, value);
    }

    public double Width
    {
        get => (double)GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    public double Height
    {
        get => (double)GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }
}