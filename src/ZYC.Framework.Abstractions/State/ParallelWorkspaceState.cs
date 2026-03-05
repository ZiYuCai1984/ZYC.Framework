using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.State;

#pragma warning disable CS1591

public class ParallelWorkspaceState : IState
{
    public Guid FocusedWorkspaceId { get; set; }
}