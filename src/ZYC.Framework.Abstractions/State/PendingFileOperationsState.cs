using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.State;

#pragma warning disable CS1591

public class PendingFileOperationsState : IState
{
    public string[] FilesToDelete { get; set; } = [];

    public string[] ZipArchivesToExtract { get; set; } = [];
}