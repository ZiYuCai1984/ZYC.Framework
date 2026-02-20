using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class CopyCommand : CommandBase
{
    protected override void InternalExecute(object? parameter)
    {
        var value = parameter?.ToString();
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        ClipboardTools.SetText(value);
    }

    /// <summary>
    ///     !WARNING Although there is only one instance, there is no problem in [CanExecute]
    /// </summary>
    public override bool CanExecute(object? parameter)
    {
        var value = parameter?.ToString();
        return !string.IsNullOrEmpty(value);
    }
}