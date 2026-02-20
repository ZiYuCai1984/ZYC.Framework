using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Runtime.CompilerServices;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Core.Menu;
using ZYC.Framework.Modules.Language.Abstractions;
using ZYC.Framework.Modules.Language.Abstractions.Event;
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language;

[RegisterSingleInstance]
internal class LanguageMainMenuItem : MainMenuItemsProvider
{
    public LanguageMainMenuItem(
        ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = LanguageModuleConstants.Title,
            Icon = LanguageModuleConstants.DefaultIcon
        };

        var languages = Enum.GetValues<LanguageType>();
        foreach (var language in languages)
        {
            RegisterSubItem(
                LifetimeScope.Resolve<SetLanguageOptionMainMenuItem>(new TypedParameter(typeof(LanguageType),
                    language)));
        }

        //RegisterSubItem(new MainMenuItem("Detail",
        //    "ListBoxOutline",
        //    lifetimeScope.CreateNavigateCommand(LanguageTabItem.Constants.Uri)));
    }

    public override MenuItemInfo Info { get; }
}

[Register]
internal class SetLanguageOptionMainMenuItem : MainMenuItem, IDisposable, INotifyPropertyChanged
{
    public SetLanguageOptionMainMenuItem(
        IEventAggregator eventAggregator,
        LanguageType languageType,
        ILanguageManager languageManager,
        LanguageConfig languageConfig)
    {
        TargetLanguageType = languageType;
        LanguageConfig = languageConfig;

        Info = new MenuItemInfo
        {
            Title = languageType.ToDisplayName(),
            Icon = Base64IconResources.Empty1x1,
            Localization = false
        };

        Command = new RelayCommand(_ => languageManager.GetCurrentLanguageType() != TargetLanguageType, _ =>
        {
            languageManager.SetCurrentLanguageType(TargetLanguageType);
        });

        eventAggregator.Subscribe<LanguageChangedEvent>(OnLanguageChanged)
            .DisposeWith(CompositeDisposable);
    }


    private CompositeDisposable CompositeDisposable { get; } = new();

    private LanguageType TargetLanguageType { get; }

    private LanguageConfig LanguageConfig { get; }

    public override string Title
    {
        get
        {
            if (LanguageConfig.CurrentLanguage != TargetLanguageType)
            {
                return Info.Title;
            }

            return $"{Info.Title} ✔️";
        }
    }

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnLanguageChanged(LanguageChangedEvent obj)
    {
        OnPropertyChanged(nameof(Title));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}