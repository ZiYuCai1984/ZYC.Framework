using System.Windows;
using MahApps.Metro.IconPacks;

namespace ZYC.Framework.Core;

public static class PackIconMaterialTools
{
    public static PackIconMaterial CreateIcon(PackIconMaterialKind kind)
    {
        return new PackIconMaterial
        {
            Kind = kind,
            Height = 16,
            Width = 16,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
    }


    public static PackIconMaterial CreateIcon(string kind)
    {
        if (!Enum.TryParse<PackIconMaterialKind>(kind, out var result))
        {
            return CreateIcon(PackIconMaterialKind.Help);
        }

        return CreateIcon(result);
    }
}