using ZYC.Automation.Abstractions.State;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Core.Commands;

[RegisterSingleInstance]
public class ErrorViewWrapSwitchCommand : CommandBase
{
    public ErrorViewWrapSwitchCommand(ErrorViewState errorViewState)
    {
        ErrorViewState = errorViewState;
    }

    private ErrorViewState ErrorViewState { get; }


    public string Text
    {
        get
        {
            if (ErrorViewState.IsTextWrap)
            {
                return "Disable Word Wrap";
            }

            return "Enable Word Wrap";
        }
    }


    public bool IsTextWrap
    {
        get => ErrorViewState.IsTextWrap;
        set => ErrorViewState.IsTextWrap = value;
    }

    protected override void InternalExecute(object? parameter)
    {
        IsTextWrap = !IsTextWrap;

        OnPropertyChanged(nameof(IsTextWrap));
        OnPropertyChanged(nameof(Text));
    }
}