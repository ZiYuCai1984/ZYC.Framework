using System.Text;
using System.Windows;
using ZYC.Automation.Abstractions.Notification.Banner;
using ZYC.Automation.Abstractions.Notification.Toast;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Mock.UI;

[Register]
public partial class TestNotificationView
{
    public TestNotificationView(IToastManager toastManager, IBannerManager bannerManager)
    {
        ToastManager = toastManager;
        BannerManager = bannerManager;

        InitializeComponent();
    }

    private IToastManager ToastManager { get; }

    private IBannerManager BannerManager { get; }

    private void OnPromptRestartBtnClick(object sender, RoutedEventArgs e)
    {
        BannerManager.PromptRestart();
    }

    private void OnPromptToastInfoBtnClick(object sender, RoutedEventArgs e)
    {
        ToastManager.PromptMessage(ToastMessage.Info("Hello World"));
    }

    private void OnPromptToastLongInfoBtnClick(object sender, RoutedEventArgs e)
    {
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < 100; i++)
        {
            stringBuilder.Append("HelloWorld");
        }
        ToastManager.PromptMessage(ToastMessage.Info(stringBuilder.ToString()));
    }

    private void OnPromptToastWarnBtnClick(object sender, RoutedEventArgs e)
    {
        ToastManager.PromptMessage(ToastMessage.Warn("Hello World"));
    }

    private void OnPromptToastErrorBtnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var arr = Array.Empty<int>();
            _ = arr[0];
        }
        catch (Exception exception)
        {
            ToastManager.PromptException(exception);
        }
    }

    // ReSharper disable once AsyncVoidEventHandlerMethod
    private async void OnDelayPromptToastBtnClick(object sender, RoutedEventArgs e)
    {
        await Task.Delay(1000);
        ToastManager.PromptMessage(ToastMessage.Warn("Hello World"));
    }
}