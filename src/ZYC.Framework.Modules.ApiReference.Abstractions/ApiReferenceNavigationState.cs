using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.ApiReference.Abstractions;

public class ApiReferenceNavigationState : IState
{
    public string Uri { get; set; } = "";
}