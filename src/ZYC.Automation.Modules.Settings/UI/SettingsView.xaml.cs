using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Settings.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Settings.UI;

[Register]
internal partial class SettingsView : ISettingsView
{
    public static readonly DependencyProperty ArrayDeleteTargetProperty =
        DependencyProperty.RegisterAttached(
            "ArrayDeleteTarget",
            typeof(string),
            typeof(SettingsView),
            new FrameworkPropertyMetadata(string.Empty)
        );

    private string? _filterText;

    public SettingsView(ISettingsManager settingsManager)
    {
        SettingsManager = settingsManager;
        SettingsManager.SetSettingsView(this);

        SettingGroupsCollectionViewSource.Source = SettingGroups;
        SettingGroupsCollectionViewSource.Filter += OnFilter;

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
                    SettingGroupsCollectionView.Refresh();
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

    public int CollectionViewCount => SettingGroupsCollectionView.Cast<object>().Count();

    private CollectionViewSource SettingGroupsCollectionViewSource { get; } = new();

    public ICollectionView SettingGroupsCollectionView => SettingGroupsCollectionViewSource.View;

    private ISettingsManager SettingsManager { get; }

    // ReSharper disable once CollectionNeverQueried.Global
    public ObservableCollection<SettingGroup> SettingGroups { get; } = new();

    private TaskCompletionSource LoadedTaskCompletionSource { get; } = new();

    public void BringIntoView(Type configType)
    {
        Task.Run(async () =>
        {
            await LoadedTaskCompletionSource.Task;
            await Dispatcher.InvokeAsync(() =>
            {
                FilterText = string.Empty;

                ItemsControl.BringIntoView((_, item) =>
                {
                    var group = (SettingGroup)item;
                    return group.Type == configType;
                });
            });
        });
    }

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

    public override void Dispose()
    {
        base.Dispose();

        SettingsManager.SetSettingsView(null);
        SettingGroupsCollectionViewSource.Filter -= OnFilter;
    }

    public static string GetArrayDeleteTarget(UIElement target)
    {
        return (string)target.GetValue(ArrayDeleteTargetProperty);
    }

    public static void SetArrayDeleteTarget(UIElement target, string value)
    {
        target.SetValue(ArrayDeleteTargetProperty, value);
    }


    protected override void InternalOnLoaded()
    {
        var groups = SettingsManager.GetSettingGroupsWithoutHidden();
        foreach (var group in groups)
        {
            SettingGroups.Add(group);
        }

        LoadedTaskCompletionSource.SetResult();
        OnPropertyChanged(nameof(CollectionViewCount));
    }

    private void OnSwitchToggled(object sender, RoutedEventArgs e)
    {
        var toggleSwitch = (ToggleSwitch)sender;
        var item = (SettingItem)toggleSwitch.Tag;

        item.ValueChangedCallbackAction.Invoke(item, toggleSwitch.IsOn);
        e.Handled = true;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender;
        var item = (SettingItem)comboBox.Tag;
        //TODO-zyc Needs to be refactored!!
        if (comboBox.SelectedItem == null || item == null)
        {
            return;
        }

        item.ValueChangedCallbackAction.Invoke(item, comboBox.SelectedItem.ToString());
        e.Handled = true;
    }

    private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var item = (SettingItem)textBox.Tag;

        item.ValueChangedCallbackAction.Invoke(item, textBox.Text);
        //e.Handled = true;
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

    private void OnNewArrayItemTextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;

        CreateArrayItemFromTextBox(textBox);
        //e.Handled = true;
    }


    private void OnDeleteArrayItemButtonClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var item = (SettingItem)button.Tag;

        var list = new List<string>((string[])item.Value!);

        var text = GetArrayDeleteTarget(button);
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        list.Remove(text);
        item.ValueChangedCallbackAction.Invoke(item, list.ToArray());
        e.Handled = true;
    }

    private void OnNewArrayItemTextBoxKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        var textBox = (TextBox)sender;
        CreateArrayItemFromTextBox(textBox);
        e.Handled = true;
    }


    private static void CreateArrayItemFromTextBox(TextBox textBox)
    {
        var item = (SettingItem)textBox.Tag;

        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            return;
        }

        var list = new List<string>((string[])item.Value!);
        list.Insert(0, textBox.Text);

        item.ValueChangedCallbackAction.Invoke(item, list.ToArray());

        textBox.Clear();
    }
}