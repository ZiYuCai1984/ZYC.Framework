using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using System.IO;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.NuGet.Abstractions;
using ZYC.Framework.Modules.TaskManager.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions.Event;

namespace ZYC.Framework.Modules.Update;

[RegisterSingleInstanceAs(typeof(IUpdateManager))]
internal class UpdateManager : IUpdateManager
{
    private const string DownloadCompletedStatusText = "Download completed.";

    private const string DownloadingStatusText = "Downloading update package...";

    private readonly SemaphoreSlim _gate = new(1, 1);

    public UpdateManager(
        IEventAggregator eventAggregator,
        INuGetManager nugetManager,
        ITaskManager taskManager,
        ILogger<UpdateManager> logger,
        IAppContext appContext,
        IProduct product,
        UpdateConfig updateConfig)
    {
        EventAggregator = eventAggregator;
        NuGetManager = nugetManager;
        TaskManager = taskManager;
        Logger = logger;
        AppContext = appContext;
        CurrentProduct = product;
        UpdateConfig = updateConfig;

        _ = UpdateUpdateContextAsync(UpdateStatus.Free, null, CancellationToken.None);
    }

    private IEventAggregator EventAggregator { get; }
    private INuGetManager NuGetManager { get; }
    private ITaskManager TaskManager { get; }
    private ILogger<UpdateManager> Logger { get; }

    private IAppContext AppContext { get; }

    private IProduct CurrentProduct { get; }

    private UpdateConfig UpdateConfig { get; }

    private UpdateContext UpdateContext { get; set; } = null!;

    public UpdateContext GetCurrentUpdateContext()
    {
        return UpdateContext;
    }


    public async Task<UpdateContext> FetchNewProductInfoAsync(CancellationToken token)
    {
        await UpdateUpdateContextAsync(UpdateStatus.Checking, null, CancellationToken.None);

        try
        {
            var packageId = ProductInfo.PackageId;

            var searchMetadata = await NuGetManager.GetSearchMetadataAsync(
                packageId,
                UpdateConfig.IncludePrerelease,
                token);

            if (!IsNeedUpdate(searchMetadata))
            {
                return await UpdateUpdateContextAsync(UpdateStatus.UpToDate, null, CancellationToken.None);
            }

            var newProduct = await ResolveNewProductFromSearchMetadataAsync(
                packageId,
                searchMetadata,
                token);
            if (newProduct == null)
            {
                throw new FileNotFoundException("Resolve new product from SearchMetadata failed.");
            }

            return await UpdateUpdateContextAsync(UpdateStatus.UpdateAvailable, newProduct, CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            return await UpdateUpdateContextAsync(UpdateStatus.CheckUpdateCanceled, null, CancellationToken.None);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return await UpdateUpdateContextAsync(UpdateStatus.CheckUpdateFaulted, null, CancellationToken.None, e);
        }
    }

    public async Task<UpdateContext> ApplyProductAsync(NewProduct product)
    {
        await UpdateUpdateContextAsync(UpdateStatus.Applying, UpdateContext.NewProduct, CancellationToken.None);

        try
        {
            var productZip = NuGetManager.GetNuGetPackageCacheFilePath(
                product.PackageId,
                product.Version);

            var extractFolder = Path.Combine(AppContext.GetAppRootDirectory(), product.Version);

            await ZipTools.UnpackZipAsync(productZip, extractFolder);
            AppContext.UpdateStartupVersion(product.Version);

            return await UpdateUpdateContextAsync(UpdateStatus.RestartPending, product, CancellationToken.None);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return await UpdateUpdateContextAsync(UpdateStatus.ApplyFaulted, UpdateContext.NewProduct,
                CancellationToken.None, e);
        }
    }


    public async Task<UpdateContext> DownloadProductAsync(NewProduct product, CancellationToken token)
    {
        await UpdateUpdateContextAsync(
            UpdateStatus.Downloading,
            product,
            CancellationToken.None,
            downloadProgress: 0,
            downloadStatusText: DownloadingStatusText);
        var processedPackages = new HashSet<string>();

        try
        {
            await NuGetManager.DownloadPackageAndDependenciesRecursiveAsync(
                product.PackageId,
                product.Version,
                processedPackages,
                token);

            token.ThrowIfCancellationRequested();

            return await UpdateUpdateContextAsync(
                UpdateStatus.ApplyPending,
                product,
                CancellationToken.None,
                downloadProgress: 1,
                downloadStatusText: DownloadCompletedStatusText);
        }
        catch (OperationCanceledException)
        {
            await UpdateUpdateContextAsync(UpdateStatus.UpdateAvailable, product, CancellationToken.None);
            throw;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return await UpdateUpdateContextAsync(UpdateStatus.DownloadFaulted, product, CancellationToken.None, e);
        }
    }

    public async Task<UpdateContext> ReportDownloadProgressAsync(
        double progress,
        string? statusText = null,
        CancellationToken token = default)
    {
        var entered = false;
        try
        {
            await _gate.WaitAsync(token).ConfigureAwait(false);
            entered = true;

            if (UpdateContext.UpdateStatus != UpdateStatus.Downloading)
            {
                return UpdateContext;
            }

            UpdateContext = new UpdateContext(
                UpdateContext.UpdateStatus,
                UpdateContext.NewProduct,
                UpdateContext.Exception,
                Math.Clamp(progress, 0, 1),
                statusText ?? UpdateContext.DownloadStatusText);
            EventAggregator.Publish(new UpdateContextChangedEvent(UpdateContext));
            return UpdateContext;
        }
        finally
        {
            if (entered)
            {
                _gate.Release();
            }
        }
    }

    private bool IsNeedUpdate(NuGetVersion searchMetadata)
    {
#if DEBUG
        return true;
#endif

        // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable CS0162 // Unreachable code detected
        return VersionTools.IsNew(searchMetadata, CurrentProduct.Version);
#pragma warning restore CS0162 // Unreachable code detected
    }


    private async Task<UpdateContext> UpdateUpdateContextAsync(
        UpdateStatus status,
        NewProduct? newProduct,
        CancellationToken token,
        Exception? exception = null,
        double? downloadProgress = null,
        string? downloadStatusText = null)
    {
        var entered = false;
        try
        {
            await _gate.WaitAsync(token).ConfigureAwait(false);
            entered = true;

            UpdateContext = new UpdateContext(status, newProduct, exception, downloadProgress, downloadStatusText);
            EventAggregator.Publish(new UpdateContextChangedEvent(UpdateContext));
            return UpdateContext;
        }
        finally
        {
            if (entered)
            {
                _gate.Release();
            }
        }
    }


    private async Task<NewProduct?> ResolveNewProductFromSearchMetadataAsync(
        string packageId,
        NuGetVersion version,
        CancellationToken token)
    {
        var metadata = await NuGetManager.GetPackageMetadataAsync(
            packageId,
            version,
            token);


        var patchNote = await NuGetManager.FetchReleaseNotesAsync(
            packageId, version.OriginalVersion!);

        return new NewProduct(
            packageId,
            version.ToString(),
            metadata.Authors,
            metadata.Description,
            patchNote ?? "");
    }
}
