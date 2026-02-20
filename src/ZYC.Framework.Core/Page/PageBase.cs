using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using ZYC.Framework.Abstractions;
using TextBlock = Emoji.Wpf.TextBlock;

namespace ZYC.Framework.Core.Page;

[ContentProperty(nameof(BodyContent))]
public abstract class PageBase : UserControl, IDisposable, INotifyPropertyChanged
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string),
        typeof(PageBase)
    );

    public static readonly DependencyProperty BodyContentProperty = DependencyProperty.Register(
        nameof(BodyContent), typeof(object),
        typeof(PageBase)
    );

    public static readonly DependencyProperty HeaderAdditionContentProperty = DependencyProperty.Register(
        nameof(HeaderAdditionContent), typeof(object),
        typeof(PageBase)
    );

    protected PageBase()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        if (!SuppressInitializeComponent)
        {
            ComponentTools.TryCallInitializeComponent(this);
        }

        Loaded += OnLoaded;

        var grid = new Grid();
        grid.Margin = new Thickness(16, 0, 16, 0);

        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        grid.Children.Add(GetHeader());
        var separator = new Separator();
        separator.SetValue(Grid.RowProperty, 1);
        grid.Children.Add(separator);

        var contentPresenter = new ContentPresenter();
        contentPresenter.SetValue(Grid.RowProperty, 2);
        contentPresenter.SetBinding(
            ContentPresenter.ContentProperty,
            new Binding(nameof(BodyContent))
            {
                Source = this
            });

        grid.Children.Add(contentPresenter);

        Content = grid;
    }

    public virtual bool SuppressInitializeComponent => false;


    public object HeaderAdditionContent
    {
        get => GetValue(HeaderAdditionContentProperty);
        set => SetValue(HeaderAdditionContentProperty, value);
    }

    public object BodyContent
    {
        get => GetValue(BodyContentProperty);
        set => SetValue(BodyContentProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    private bool FirstRending { get; set; } = true;

    protected CompositeDisposable CompositeDisposable { get; } = new();

    public virtual void Dispose()
    {
        Loaded -= OnLoaded;
        CompositeDisposable.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private UIElement GetHeader()
    {
        var grid = new Grid();
        grid.Height = 48;

        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var textBlock = new TextBlock
        {
            FontWeight = FontWeights.Bold,
            FontSize = 16,
            VerticalAlignment = VerticalAlignment.Center
        };

        textBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Title))
        {
            Source = this
        });

        grid.Children.Add(textBlock);


        var contentPresenter = new ContentPresenter();
        contentPresenter.SetValue(Grid.ColumnProperty, 1);
        contentPresenter.SetBinding(
            ContentPresenter.ContentProperty,
            new Binding(nameof(HeaderAdditionContent))
            {
                Source = this
            });

        grid.Children.Add(contentPresenter);

        return grid;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!FirstRending)
        {
            return;
        }

        FirstRending = false;
        InternalOnLoaded();
    }

    protected virtual void InternalOnLoaded()
    {
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}