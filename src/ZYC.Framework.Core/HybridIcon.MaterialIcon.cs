using System.Windows.Data;
using MahApps.Metro.IconPacks;
using ZYC.CoreToolkit;

namespace ZYC.Framework.Core;

public partial class HybridIcon
{
    private void SetFromMaterialIcon(PackIconMaterialKind kind)
    {
        Content?.TryDispose();

        Content = new PackIconMaterial
        {
            Kind = kind
        };

        if (Content is PackIconMaterial pi)
        {
            pi.SetBinding(VerticalAlignmentProperty, new Binding(nameof(VerticalAlignment)) { Source = this });
            pi.SetBinding(VerticalContentAlignmentProperty,
                new Binding(nameof(VerticalContentAlignment)) { Source = this });

            pi.SetBinding(HorizontalAlignmentProperty, new Binding(nameof(HorizontalAlignment)) { Source = this });
            pi.SetBinding(HorizontalContentAlignmentProperty,
                new Binding(nameof(HorizontalContentAlignment)) { Source = this });


            pi.SetBinding(WidthProperty, new Binding(nameof(Width)) { Source = this });
            pi.SetBinding(HeightProperty, new Binding(nameof(Height)) { Source = this });
            pi.SetBinding(ForegroundProperty, new Binding(nameof(Foreground)) { Source = this });
            pi.SetBinding(FontSizeProperty, new Binding(nameof(FontSize)) { Source = this });
        }
    }
}