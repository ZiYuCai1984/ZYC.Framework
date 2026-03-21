using System.Windows;
using System.Windows.Controls;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Secrets;

internal class SecretsValueTemplateSelector : DataTemplateSelector
{
    //public DataTemplate? BooleanTemplate { get; set; }
    public DataTemplate? StringTemplate { get; set; }
    public DataTemplate? Int32Template { get; set; }
    //public DataTemplate? DoubleTemplate { get; set; }
    //public DataTemplate? ArrayTemplate { get; set; }
    //public DataTemplate? EnumTemplate { get; set; }
    public DataTemplate? MultilineTextTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        return item switch
        {
            //bool => BooleanTemplate,
            string => StringTemplate,
            int => Int32Template,
            //double => DoubleTemplate,
            //Array => ArrayTemplate,
            //Enum => EnumTemplate,
            MultilineText => MultilineTextTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}