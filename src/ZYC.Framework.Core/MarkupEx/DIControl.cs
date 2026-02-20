using System.Diagnostics;
using System.Windows.Markup;
using System.Xaml;
using Autofac;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core.MarkupEx;

/// <summary>
///     !WARNING The reason why core:Component is not replaced by markupEx:DIControl is that it is impossible to set
///     Additional property in markupEx:DIControl(like Grid.Column="1") !!
/// </summary>
public class DIControl : MarkupExtension
{
    public Type? Type { get; set; }

    public ILifetimeScope? LifetimeScope { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        //!WARNING When xaml is initialized, get the current xaml root element
        var rootObjectProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

        var lifetimeScope = LifetimeScope;

        if (rootObjectProvider?.RootObject is ILifetimeScopeHolder holder)
        {
            lifetimeScope = (ILifetimeScope)holder.GetLifetimeScope();
        }
        else
        {
            Debugger.Break();
        }


        Debug.Assert(lifetimeScope != null);
        Debug.Assert(Type != null);

        return lifetimeScope.Resolve(Type);
    }
}