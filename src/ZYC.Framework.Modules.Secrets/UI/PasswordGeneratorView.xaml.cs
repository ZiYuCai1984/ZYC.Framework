using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Secrets.Abstractions;
using ZYC.Framework.Modules.Secrets.Commands;

namespace ZYC.Framework.Modules.Secrets.UI;

[Register]
internal sealed partial class PasswordGeneratorView
{
    public PasswordGeneratorView(
        PasswordGeneratorOptionsState passwordGeneratorOptionsState,
        RandomPasswordCommand randomPasswordCommand,
        CopyAndNotifyCommand copyAndNotifyCommand)
    {
        PasswordGeneratorOptionsState = passwordGeneratorOptionsState;

        RandomPasswordCommand = randomPasswordCommand;
        RandomPasswordCommand.SetView(this);

        CopyAndNotifyCommand = copyAndNotifyCommand;

        InitializeComponent();
    }

    public override bool SuppressInitializeComponent => true;

    public RandomPasswordCommand RandomPasswordCommand { get; }
    public CopyAndNotifyCommand CopyAndNotifyCommand { get; }
    public PasswordGeneratorOptionsState PasswordGeneratorOptionsState { get; }

    public ObservableCollection<string> Passwords { get; } = new();

    protected override void InternalOnLoaded()
    {
        base.InternalOnLoaded();
        RandomPasswordCommand.Execute(PasswordGeneratorOptionsState);
    }

    public void UpdatePasswords(IEnumerable<string> passwords)
    {
        Passwords.Clear();
        foreach (var p in passwords)
        {
            Passwords.Add(p);
        }
    }

    private void OnPasswordTextBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;

        if (sender is FrameworkElement fe && fe.DataContext is string pwd && !string.IsNullOrEmpty(pwd))
        {
            CopyAndNotifyCommand.Execute(pwd);
        }
    }

    private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        RandomPasswordCommand.Execute(PasswordGeneratorOptionsState);
    }

    private async void OnPasswordCharOptionsClick(object sender, RoutedEventArgs e)
    {
        await Task.Delay(200);
        RandomPasswordCommand.Execute(PasswordGeneratorOptionsState);
    }
}