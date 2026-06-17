using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Aspire;

internal partial class AspireService : IDisposable
{
    public bool IsDisposed { get; set; }

    /// <summary>
    ///     !WARNING This Dispose may be a time-consuming operation.
    /// </summary>
    public void Dispose()
    {
        lock (this)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            Gate.Dispose();
            CompositeDisposable.Dispose();

            try
            {
                DistributedApplication.Dispose();
            }
            catch(Exception ex)
            {
                //!WARNING For capture this Exception
                // Could not load file or assembly 'C:\d\ZYC.Framework\src\_bin\1.3.6\StreamJsonRpc.dll'.
                // The located assembly's manifest definition does not match the assembly reference. (0x80131040)'
                Logger.Error(ex);
            }
        }
    }
}