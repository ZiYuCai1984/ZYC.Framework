using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.State;

public class ErrorViewState : IState
{
    public bool IsTextWrap { get; set; } = true;
}