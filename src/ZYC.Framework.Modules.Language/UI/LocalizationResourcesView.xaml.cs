using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Language.Abstractions;
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language.UI;

[Register]
internal sealed partial class LocalizationResourcesView
{
    private string? _filterText;
    private LanguageType _languageType;

    public LocalizationResourcesView(
        ILanguageManager languageManager,
        ILanguageResourcesManager languageResourcesManager,
        IToastManager toastManager)
    {
        LanguageResourcesManager = languageResourcesManager;
        ToastManager = toastManager;
        _languageType = languageManager.GetCurrentLanguageType();
        OnPropertyChanged(nameof(LanguageType));

        ResourceEntriesCollectionViewSource.Source = ResourceEntries;
        ResourceEntriesCollectionViewSource.Filter += OnFilter;

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
                ResourceEntriesCollectionView.Refresh();
                OnPropertyChanged(nameof(CollectionViewCount));
            }).DisposeWith(CompositeDisposable);
    }

    public LanguageType[] LanguageTypes { get; } = Enum.GetValues<LanguageType>();

    public LanguageType LanguageType
    {
        get => _languageType;
        set
        {
            if (_languageType == value)
            {
                return;
            }

            _languageType = value;
            OnPropertyChanged();
            LoadResourceEntries();
        }
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

    public int CollectionViewCount => ResourceEntriesCollectionView.Cast<object>().Count();

    private CollectionViewSource ResourceEntriesCollectionViewSource { get; } = new();

    public ICollectionView ResourceEntriesCollectionView => ResourceEntriesCollectionViewSource.View;

    public ObservableCollection<LanguageResourceEntryRow> ResourceEntries { get; } = new();

    private ILanguageResourcesManager LanguageResourcesManager { get; }

    private IToastManager ToastManager { get; }

    protected override void InternalOnLoaded()
    {
        LoadResourceEntries();
    }

    public override void Dispose()
    {
        base.Dispose();

        ResourceEntriesCollectionViewSource.Filter -= OnFilter;
    }

    private void LoadResourceEntries()
    {
        ResourceEntries.Clear();

        foreach (var entry in LanguageResourcesManager.GetLanguageResourceEntries(LanguageType))
        {
            ResourceEntries.Add(new LanguageResourceEntryRow(entry));
        }

        ResourceEntriesCollectionView.Refresh();
        OnPropertyChanged(nameof(CollectionViewCount));
    }

    private void OnFilter(object sender, FilterEventArgs e)
    {
        if (e.Item is not LanguageResourceEntryRow entry)
        {
            e.Accepted = false;
            return;
        }

        var text = FilterText;
        if (string.IsNullOrWhiteSpace(text))
        {
            e.Accepted = true;
            return;
        }

        e.Accepted = Contains(entry.LanguageDisplayName, text)
                     || Contains(entry.Key, text)
                     || Contains(entry.Value, text)
                     || Contains(entry.SourceName, text);
    }

    private static bool Contains(string? value, string text)
    {
        return !string.IsNullOrWhiteSpace(value)
               && value.Contains(text, StringComparison.CurrentCultureIgnoreCase);
    }

    private void OnValueTextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        SaveEntry(textBox.DataContext as LanguageResourceEntryRow);
    }

    private void OnValueTextBoxKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        var textBox = (TextBox)sender;
        SaveEntry(textBox.DataContext as LanguageResourceEntryRow);
        e.Handled = true;
    }

    private void SaveEntry(LanguageResourceEntryRow? entry)
    {
        if (entry == null || !entry.CanEdit || !entry.IsDirty)
        {
            return;
        }

        try
        {
            var updated = LanguageResourcesManager.UpdateLanguageResourceEntry(
                entry.LanguageType,
                entry.Key,
                entry.Value);

            entry.Apply(updated);
            ToastManager.PromptMessage(ToastMessage.Info("Localization updated.", false));
            ResourceEntriesCollectionView.Refresh();
            OnPropertyChanged(nameof(CollectionViewCount));
        }
        catch (Exception ex)
        {
            ToastManager.PromptException(ex);
        }
    }
}

internal sealed class LanguageResourceEntryRow : INotifyPropertyChanged
{
    private string _sourceName;
    private string _value;

    public LanguageResourceEntryRow(LanguageResourceEntry entry)
    {
        LanguageType = entry.LanguageType;
        Key = entry.Key;
        _value = entry.Value;
        _sourceName = entry.SourceName;
        OriginalValue = entry.Value;
        Exists = entry.Exists;
        CanEdit = entry.CanEdit;
    }

    public LanguageType LanguageType { get; }

    public string LanguageDisplayName => LanguageType.ToDisplayName();

    public string Key { get; }

    public string Value
    {
        get => _value;
        set
        {
            if (_value == value)
            {
                return;
            }

            _value = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsDirty));
        }
    }

    public string SourceName
    {
        get => _sourceName;
        private set
        {
            if (_sourceName == value)
            {
                return;
            }

            _sourceName = value;
            OnPropertyChanged();
        }
    }

    public bool Exists { get; private set; }

    public bool CanEdit { get; private set; }

    public bool IsValueReadOnly => !CanEdit;

    public bool IsDirty => Value != OriginalValue;

    private string OriginalValue { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Apply(LanguageResourceEntry entry)
    {
        SourceName = entry.SourceName;
        Exists = entry.Exists;
        CanEdit = entry.CanEdit;
        OriginalValue = entry.Value;
        Value = entry.Value;


        OnPropertyChanged(nameof(Exists));
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(IsValueReadOnly));
        OnPropertyChanged(nameof(IsDirty));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}