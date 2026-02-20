using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.Mock.UI;

[Register]
public partial class TestTaskManagerView
{
    public TestTaskManagerView(ITaskManager taskManager)
    {
        TaskManager = taskManager;

        InitializeComponent();
    }

    private ITaskManager TaskManager { get; }
}