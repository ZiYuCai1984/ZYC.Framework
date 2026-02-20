using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Notification.Banner;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.TaskManager.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions;
using ZYC.Framework.Modules.Update.UI.Toast;

namespace ZYC.Framework.Modules.Update;

[RegisterSingleInstance]
internal class CheckUpdateTaskDefinition : IManagedTaskDefinition
{
    public CheckUpdateTaskDefinition(
        ILifetimeScope lifetimeScope,
        TaskDefinitionCreateContext taskDefinitionCreateContext)
    {
        LifetimeScope = lifetimeScope;
        TaskDefinitionCreateContext = taskDefinitionCreateContext;
    }

    private ILifetimeScope LifetimeScope { get; }

    public static string DefinitionId => $"{UpdateTaskProvider.ProviderId}/{TaskType}";

    private TaskDefinitionCreateContext TaskDefinitionCreateContext { get; }

    private IUpdateManager UpdateManager => LifetimeScope.Resolve<IUpdateManager>();

    private IBannerManager BannerManager => LifetimeScope.Resolve<IBannerManager>();

    private IToastManager ToastManager => LifetimeScope.Resolve<IToastManager>();

    public static string TaskType => "check-update";

    string IManagedTaskDefinition.TaskType => TaskType;

    public string DisplayName => "Check update";

    public string Description => "Check if a new version exists by NuGet.";

    public async Task ExecuteAsync(TaskExecutionContext context, CancellationToken ct)
    {
        var newProductContext = await UpdateManager.FetchNewProductInfoAsync(ct);
        if (newProductContext.NewProduct != null)
        {
            ToastManager.Prompt<PromptNewProductToastView>(newProductContext.NewProduct);
        }
    }
}