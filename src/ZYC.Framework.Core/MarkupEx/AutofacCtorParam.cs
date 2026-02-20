using Autofac;
using Autofac.Core;

namespace ZYC.Framework.Core.MarkupEx;

#pragma warning disable CS8618
public sealed class AutofacCtorParam
{
    public string? Name { get; set; }

    public Type? Type { get; set; }

    public object? Value { get; set; }

    internal Parameter ToAutofacParameter()
    {
        if (!string.IsNullOrWhiteSpace(Name))
        {
            return new NamedParameter(Name!, Value);
        }

        var t = Type ?? Value?.GetType();
        if (t is null)
        {
            throw new InvalidOperationException("When Value is null, you must set Type.");
        }

        return new TypedParameter(t, Value);
    }
}