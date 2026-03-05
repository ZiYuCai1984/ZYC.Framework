using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.State;

#pragma warning disable CS1591

public class ErrorViewState : IState
{
    public bool IsTextWrap { get; set; } = true;
}