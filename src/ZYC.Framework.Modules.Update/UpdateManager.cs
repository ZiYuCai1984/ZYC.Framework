using System.IO;
using NuGet.Versioning;
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
    private readonly SemaphoreSlim _gate = new(1, 1);

    public UpdateManager(
        IEventAggregator eventAggregator,
        INuGetManager nugetManager,
        ITaskManager taskManager,
        IAppLogger<UpdateManager> logger,
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
    private IAppLogger<UpdateManager> Logger { get; }

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

            string extractFolder;

            if (AppContext.IsSelfMain())
            {
                extractFolder = AppContext.GetAlternateAppDirectory();
            }

            else
            {
                extractFolder = AppContext.GetMainAppDirectory();
            }

            await ZipTools.UnpackZipAsync(productZip, extractFolder);
            AppContext.SwitchStartupTarget();

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
        await UpdateUpdateContextAsync(UpdateStatus.Downloading, UpdateContext.NewProduct, token);
        var processedPackages = new HashSet<string>();

        try
        {
            await NuGetManager.DownloadPackageAndDependenciesRecursiveAsync(
                product.PackageId,
                product.Version,
                processedPackages,
                token);

            return await UpdateUpdateContextAsync(UpdateStatus.ApplyPending, product, token);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return await UpdateUpdateContextAsync(UpdateStatus.DownloadFaulted, product, token, e);
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
        Exception? exception = null)
    {
        try
        {
            await _gate.WaitAsync(token).ConfigureAwait(false);

            UpdateContext = new UpdateContext(status, newProduct, exception);
            EventAggregator.Publish(new UpdateContextChangedEvent(UpdateContext));
            return UpdateContext;
        }
        finally
        {
            _gate.Release();
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