using System.ComponentModel;
using System.Windows;

namespace ZYC.Framework.Core;

public static class DesignMode
{
    public static bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());
}