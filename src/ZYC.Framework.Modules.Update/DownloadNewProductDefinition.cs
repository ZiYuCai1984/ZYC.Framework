using System.Text.Json;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.TaskManager.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions;

namespace ZYC.Framework.Modules.Update;

[Register]
internal class DownloadNewProductDefinition : IManagedTaskDefinition
{
    private const double FakeProgressCeiling = 0.95;

    private static readonly TimeSpan FakeProgressInterval = TimeSpan.FromMilliseconds(500);

    public DownloadNewProductDefinition(
        ILifetimeScope lifetimeScope,
        TaskDefinitionCreateContext taskDefinitionCreateContext)
    {
        LifetimeScope = lifetimeScope;

        NewProduct = JsonSerializer.Deserialize<NewProduct>(taskDefinitionCreateContext.PayloadJson)
                     ?? throw new InvalidOperationException("Invalid payload JSON.");
    }


    public static string DefinitionId => $"{UpdateTaskProvider.ProviderId}/{TaskType}";

    public static string TaskType => "download-new-product";

    private IUpdateManager UpdateManager => LifetimeScope.Resolve<IUpdateManager>();

    private ILifetimeScope LifetimeScope { get; }

    private NewProduct? NewProduct { get; }

    string IManagedTaskDefinition.TaskType => TaskType;

    public string DisplayName
    {
        get
        {
            if (NewProduct == null)
            {
                return "Download new product";
            }

            return $"Download new product({NewProduct.Version})";
        }
    }

    public string Description => "Download and install the new version of the product.";

    public async Task ExecuteAsync(TaskExecutionContext context, CancellationToken ct)
    {
        var updateManager = UpdateManager;

        context.Progress?.Report(0);
        context.StatusText?.Report("Downloading update package...");

        using var fakeProgressCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var fakeProgressTask = RunFakeProgressAsync(context, updateManager, fakeProgressCts.Token);

        try
        {
            await updateManager.DownloadProductAsync(NewProduct!, ct);

            context.Progress?.Report(1);
            context.StatusText?.Report("Download completed.");
        }
        finally
        {
            fakeProgressCts.Cancel();

            try
            {
                await fakeProgressTask;
            }
            catch (OperationCanceledException)
            {
                // Expected when the real download finishes before the fake progress loop.
            }
        }
    }

    private static async Task RunFakeProgressAsync(
        TaskExecutionContext context,
        IUpdateManager updateManager,
        CancellationToken ct)
    {
        var progress = 0d;

        while (progress < FakeProgressCeiling)
        {
            await context.Pause.WaitIfPausedAsync(ct);
            await Task.Delay(FakeProgressInterval, ct);

            progress = Math.Min(FakeProgressCeiling, progress + GetFakeProgressStep(progress));
            context.Progress?.Report(progress);
            await updateManager.ReportDownloadProgressAsync(progress, token: ct);
        }
    }

    private static double GetFakeProgressStep(double progress)
    {
        if (progress < 0.4)
        {
            return 0.035;
        }

        if (progress < 0.7)
        {
            return 0.02;
        }

        if (progress < 0.9)
        {
            return 0.01;
        }

        return 0.003;
    }
}
