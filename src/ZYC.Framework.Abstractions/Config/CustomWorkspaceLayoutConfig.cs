using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Abstractions.Config;

[AddINotifyPropertyChangedInterface]
public class CustomWorkspaceLayoutConfig : IConfig
{
    public CustomWorkspaceLayout[] Layout { get; set; } = [];
}

public class CustomWorkspaceLayout
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public WorkspaceNode WorkspaceNode { get; set; } = null!;
}