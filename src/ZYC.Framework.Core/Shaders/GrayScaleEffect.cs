using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core.Shaders;

[Register]
public class GrayScaleEffect : ShaderEffect
{
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            nameof(Input),
            typeof(GrayScaleEffect),
            0);

    public GrayScaleEffect(ILogger<GrayScaleEffect> logger)
    {
        try
        {
            var uri = UriTools.BuildShaderUri(typeof(GrayScaleEffect));

            PixelShader = new PixelShader
            {
                UriSource = uri
            };

            UpdateShaderValue(InputProperty);
        }
        catch (Exception e)
        {
            logger.Error(e);
            throw;
        }
    }

    public Brush Input
    {
        get => (Brush)GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }
}