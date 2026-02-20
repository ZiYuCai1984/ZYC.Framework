using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ZYC.Framework.Abstractions.Notification;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Core.Notification;

public class NotificationBase : UserControl, INotification
{
    private bool _started;

    public NotificationBase()
    {
        Loaded += OnNotificationBaseLoaded;

        DataContext = this;

        CloseBannerCommand = new RelayCommand(_ => true, _ => CloseNow());
    }

    private Storyboard? FadeOutStoryboard { get; set; }

    public ICommand CloseBannerCommand { get; }

    public event EventHandler? Closed;

    public object GetVisibility()
    {
        return Visibility;
    }

    private void OnNotificationBaseLoaded(object sender, RoutedEventArgs e)
    {
        if (_started)
        {
            return;
        }

        _started = true;

        var fadeOutAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromSeconds(1)),
            BeginTime = TimeSpan.FromSeconds(5)
        };

        FadeOutStoryboard = new Storyboard();
        FadeOutStoryboard.Children.Add(fadeOutAnimation);

        Storyboard.SetTarget(fadeOutAnimation, this);
        Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(nameof(Opacity)));

        FadeOutStoryboard.Completed += OnFadeOutStoryboardCompleted;
        FadeOutStoryboard.Begin();
    }

    private void CloseNow()
    {
        if (FadeOutStoryboard == null)
        {
            Visibility = Visibility.Collapsed;
            Closed?.Invoke(this, EventArgs.Empty);
            return;
        }

        FadeOutStoryboard.SkipToFill();
    }

    private void OnFadeOutStoryboardCompleted(object? sender, EventArgs e)
    {
        Debug.Assert(FadeOutStoryboard != null);
        FadeOutStoryboard.Completed -= OnFadeOutStoryboardCompleted;

        Visibility = Visibility.Collapsed;
        Closed?.Invoke(this, EventArgs.Empty);
    }
}