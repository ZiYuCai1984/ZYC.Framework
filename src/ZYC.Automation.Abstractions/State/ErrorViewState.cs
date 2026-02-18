using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Automation.Abstractions.State;

public class ErrorViewState : IState
{
    public bool IsTextWrap { get; set; }
}