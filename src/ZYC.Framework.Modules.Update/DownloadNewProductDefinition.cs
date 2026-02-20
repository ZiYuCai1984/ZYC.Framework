using System.Text.Json;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.TaskManager.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions;

namespace ZYC.Framework.Modules.Update;

[Register]
internal class DownloadNewProductDefinition : IManagedTaskDefinition
{
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
        await UpdateManager.DownloadProductAsync(NewProduct!, ct);
    }
}