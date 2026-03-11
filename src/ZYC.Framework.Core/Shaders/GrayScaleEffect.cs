using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core.Shaders;

public class GrayScaleEffect : ShaderEffect
{
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            nameof(Input),
            typeof(GrayScaleEffect),
            0);

    public GrayScaleEffect()
    {
        var uri = UriTools.BuildShaderUri(typeof(GrayScaleEffect));

        PixelShader = new PixelShader
        {
            UriSource = uri
        };

        UpdateShaderValue(InputProperty);
    }

    public Brush Input
    {
        get => (Brush)GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }
}