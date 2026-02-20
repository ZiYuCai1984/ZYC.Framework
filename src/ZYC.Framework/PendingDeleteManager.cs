using ZYC.CoreToolkit.Abstractions.Attributes;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.State;

namespace ZYC.Framework;

[RegisterSingleInstanceAs(typeof(IPendingDeleteManager))]
[TempCode]
public class PendingDeleteManager : IPendingDeleteManager
{
    public PendingDeleteManager(PendingFileOperationsState pendingFileOperationsState)
    {
        PendingFileOperationsState = pendingFileOperationsState;
    }

    private PendingFileOperationsState PendingFileOperationsState { get; }

    public void Add(string fileName)
    {
        var pendingRemove = PendingFileOperationsState.FilesToDelete.ToList();
        pendingRemove.Add(fileName);
        PendingFileOperationsState.FilesToDelete = pendingRemove.ToArray();
    }

    public bool Contains(string fileName)
    {
        return PendingFileOperationsState.FilesToDelete.Contains(fileName);
    }

    public string[] GetFiles()
    {
        return PendingFileOperationsState.FilesToDelete;
    }
}