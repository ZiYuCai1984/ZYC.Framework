using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Banner;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core;

namespace ZYC.Framework.Dialog;

[Register]
public partial class OverrideMutexIdDialog : INotifyPropertyChanged
{
    private string _mutexOverrideId = "";

    public OverrideMutexIdDialog(
        ILogger<OverrideMutexIdDialog> logger,
        IToastManager toastManager,
        IBannerManager bannerManager)
    {
        Logger = logger;
        ToastManager = toastManager;
        BannerManager = bannerManager;

        InitializeComponent();
        LoadMutexOverrideId();
    }

    public string MutexOverrideId
    {
        get => _mutexOverrideId;
        set
        {
            if (_mutexOverrideId == value)
            {
                return;
            }

            _mutexOverrideId = value;
            OnPropertyChanged();
        }
    }

    private ILogger<OverrideMutexIdDialog> Logger { get; }

    private IToastManager ToastManager { get; }

    private IBannerManager BannerManager { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnDialogLoaded(object sender, RoutedEventArgs e)
    {
        MutexOverrideIdTextBox.Focus();
        MutexOverrideIdTextBox.SelectAll();
    }

    private void LoadMutexOverrideId()
    {
        try
        {
            var path = MutexTools.GetMutexOverridePath();
            if (File.Exists(path))
            {
                MutexOverrideId = File.ReadAllText(path).Trim();
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}