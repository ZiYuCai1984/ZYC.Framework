using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Secrets.Abstractions;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Secrets.UI;

[RegisterSingleInstanceAs(typeof(ISecretsManager), typeof(SecretsManagerView))]
internal sealed partial class SecretsManagerView
{
    private string? _filterText;

    public SecretsManagerView(
        ISecrets[] secrets,
        ISettingsManager settingsManager)
    {
        Secrets = secrets;
        SettingsManager = settingsManager;

        SecretGroupsCollectionViewSource.Source = SecretGroups;
        SecretGroupsCollectionViewSource.Filter += OnFilter;


        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => PropertyChanged += h,
                h => PropertyChanged -= h)
            .Where(e => e.EventArgs.PropertyName == nameof(FilterText))
            .Select(_ => FilterText)
            .Throttle(TimeSpan.FromMilliseconds(250))
            .DistinctUntilChanged()
            .ObserveOnUI()
            .Subscribe(_ =>
                {
                    SecretGroupsCollectionView.Refresh();
                    OnPropertyChanged(nameof(CollectionViewCount));
                }
            ).DisposeWith(CompositeDisposable);
    }


    public string? FilterText
    {
        get => _filterText;
        set
        {
            if (_filterText == value)
            {
                return;
            }

            _filterText = value;
            OnPropertyChanged();
        }
    }

    private CollectionViewSource SecretGroupsCollectionViewSource { get; } = new();

    public ICollectionView SecretGroupsCollectionView => SecretGroupsCollectionViewSource.View;

    public int CollectionViewCount => SecretGroupsCollectionView.Cast<object>().Count();

    private ISettingsManager SettingsManager { get; }

    public ObservableCollection<SettingGroup> SecretGroups { get; set; } = [];

    private bool FirstRending { get; set; } = true;

    private void OnFilter(object sender, FilterEventArgs e)
    {
        if (e.Item is not SettingGroup settingGroup)
        {
            e.Accepted = false;
            return;
        }

        var t = FilterText;
        if (string.IsNullOrWhiteSpace(t))
        {
            e.Accepted = true;
            return;
        }

        if (settingGroup.Name.Contains(t, StringComparison.CurrentCultureIgnoreCase))
        {
            e.Accepted = true;
            return;
        }

        foreach (var settingItem in settingGroup.SettingItems)
        {
            if (!settingItem.Name.Contains(t, StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }

            e.Accepted = true;
            return;
        }

        e.Accepted = false;
    }

    private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        var textBox = (TextBox)sender;
        var item = (SettingItem)textBox.Tag;

        item.ValueChangedCallbackAction.Invoke(item, textBox.Text);
        e.Handled = true;
    }

    private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var item = (SettingItem)textBox.Tag;

        item.ValueChangedCallbackAction.Invoke(item, textBox.Text);
    }

    private void OnSecretsManagerViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!FirstRending)
        {
            return;
        }

        FirstRending = false;

        var secretGroups = SettingsManager.CreateSettingGroups(GetSecretsConfigs());
        foreach (var secretGroup in secretGroups)
        {
            SecretGroups.Add(secretGroup);
        }

        LoadedTaskCompletionSource.SetResult();
        OnPropertyChanged(nameof(CollectionViewCount));
    }
}