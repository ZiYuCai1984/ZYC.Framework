using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.State;

public class ParallelWorkspaceState : IState
{
    public Guid FocusedWorkspaceId { get; set; }
}