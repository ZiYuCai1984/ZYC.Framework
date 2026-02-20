using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.State;

public class PendingFileOperationsState : IState
{
    public string[] FilesToDelete { get; set; } = [];

    public string[] ZipArchivesToExtract { get; set; } = [];
}