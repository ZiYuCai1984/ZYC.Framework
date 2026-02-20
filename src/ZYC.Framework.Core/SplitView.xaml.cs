using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ZYC.Framework.Core;

public partial class SplitView
{
    private const double RatioMin = 0.05;
    private const double RatioMax = 0.95;


    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(
            nameof(Orientation), typeof(Orientation), typeof(SplitView),
            new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    public SplitView()
    {
        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private ColumnDefinition? ColLeft { get; set; }

    private ColumnDefinition? ColRight { get; set; }

    private RowDefinition? RowBottom { get; set; }

    private RowDefinition? RowTop { get; set; }

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        BuildLayout();

        if (Splitter != null)
        {
            Splitter.DragCompleted += OnSplitterDragCompleted;
        }

        if (AutoPersist && !string.IsNullOrEmpty(PersistKey) && TryLoadRatio(out var r))
        {
            Ratio = r;
        }

        ApplyRatio(Ratio);
    }

    private void BuildLayout()
    {
        Root.RowDefinitions.Clear();
        Root.ColumnDefinitions.Clear();

        if (Orientation == Orientation.Horizontal)
        {
            var left = new ColumnDefinition { MinWidth = MinLeftWidth, Width = new GridLength(1, GridUnitType.Star) };
            var mid = new ColumnDefinition { Width = new GridLength(SplitterWidth, GridUnitType.Pixel) };
            var right = new ColumnDefinition { MinWidth = MinRightWidth, Width = new GridLength(1, GridUnitType.Star) };

            Root.ColumnDefinitions.Add(left);
            Root.ColumnDefinitions.Add(mid);
            Root.ColumnDefinitions.Add(right);

            Grid.SetColumn(Splitter, 1);
            Grid.SetRow(Splitter, 0);

            Splitter.ResizeDirection = GridResizeDirection.Columns;
            Splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
            Splitter.VerticalAlignment = VerticalAlignment.Stretch;

            Grid.SetColumn(LeftPresenter, 0);
            Grid.SetRow(LeftPresenter, 0);
            Grid.SetColumn(RightPresenter, 2);
            Grid.SetRow(RightPresenter, 0);
        }
        else
        {
            var top = new RowDefinition { MinHeight = MinLeftWidth, Height = new GridLength(1, GridUnitType.Star) };
            var middle = new RowDefinition { Height = new GridLength(SplitterWidth, GridUnitType.Pixel) };
            var bottom = new RowDefinition { MinHeight = MinRightWidth, Height = new GridLength(1, GridUnitType.Star) };

            Root.RowDefinitions.Add(top);
            Root.RowDefinitions.Add(middle);
            Root.RowDefinitions.Add(bottom);

            Grid.SetRow(Splitter, 1);
            Grid.SetColumn(Splitter, 0);

            Splitter.ResizeDirection = GridResizeDirection.Rows;
            Splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
            Splitter.VerticalAlignment = VerticalAlignment.Stretch;

            Grid.SetRow(LeftPresenter, 0);
            Grid.SetColumn(LeftPresenter, 0);
            Grid.SetRow(RightPresenter, 2);
            Grid.SetColumn(RightPresenter, 0);
        }

        ColLeft = Orientation == Orientation.Horizontal ? Root.ColumnDefinitions[0] : null;
        ColRight = Orientation == Orientation.Horizontal ? Root.ColumnDefinitions[2] : null;

        RowTop = Orientation == Orientation.Vertical ? Root.RowDefinitions[0] : null;
        RowBottom = Orientation == Orientation.Vertical ? Root.RowDefinitions[2] : null;
    }

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SplitView sv && sv.IsLoaded)
        {
            sv.BuildLayout();
            sv.ApplyRatio(sv.Ratio);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (Splitter != null)
        {
            Splitter.DragCompleted -= OnSplitterDragCompleted;
        }
    }

    private static void OnRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SplitView sv)
        {
            sv.ApplyRatio((double)e.NewValue);
        }
    }

    private static object CoerceRatioCallback(DependencyObject d, object baseValue)
    {
        return CoerceRatio((double)baseValue);
    }

    private static double CoerceRatio(double r)
    {
        if (double.IsNaN(r) || double.IsInfinity(r))
        {
            return 0.5;
        }

        if (r < RatioMin)
        {
            return RatioMin;
        }

        if (r > RatioMax)
        {
            return RatioMax;
        }

        return r;
    }

    private void ApplyRatio(double ratio)
    {
        if (Orientation == Orientation.Horizontal)
        {
            if (ColLeft == null || ColRight == null)
            {
                return;
            }

            ColLeft.Width = new GridLength(ratio, GridUnitType.Star);
            ColRight.Width = new GridLength(1 - ratio, GridUnitType.Star);
        }
        else
        {
            if (RowTop == null || RowBottom == null)
            {
                return;
            }

            RowTop.Height = new GridLength(ratio, GridUnitType.Star);
            RowBottom.Height = new GridLength(1 - ratio, GridUnitType.Star);
        }
    }


    private void OnSplitterDragCompleted(object? sender, DragCompletedEventArgs e)
    {
        if (Orientation == Orientation.Horizontal)
        {
            if (ColLeft == null || ColRight == null)
            {
                return;
            }

            var left = ColLeft.ActualWidth;
            var right = ColRight.ActualWidth;
            var total = left + right;
            if (total > 1)
            {
                Ratio = CoerceRatio(left / total);
                if (AutoPersist && !string.IsNullOrEmpty(PersistKey))
                {
                    SaveRatio(Ratio);
                }
            }
        }
        else
        {
            if (RowTop == null || RowBottom == null)
            {
                return;
            }

            var top = RowTop.ActualHeight;
            var bottom = RowBottom.ActualHeight;
            var total = top + bottom;
            if (total > 1)
            {
                Ratio = CoerceRatio(top / total);
                if (AutoPersist && !string.IsNullOrEmpty(PersistKey))
                {
                    SaveRatio(Ratio);
                }
            }
        }
    }

    private void SaveRatio(double ratio)
    {
        try
        {
            Application.Current.Properties[PersistKey!] = ratio;
        }
        catch
        {
            /* Log as needed. */
        }
    }

    private bool TryLoadRatio(out double ratio)
    {
        ratio = 0.5;
        try
        {
            if (!string.IsNullOrEmpty(PersistKey) &&
                Application.Current.Properties.Contains(PersistKey) &&
                Application.Current.Properties[PersistKey] is double r)
            {
                ratio = CoerceRatio(r);
                return true;
            }
        }
        catch
        {
            /* ignore */
        }

        return false;
    }

    #region Dependency Properties

    public object? LeftContent
    {
        get => GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
    }

    public static readonly DependencyProperty LeftContentProperty =
        DependencyProperty.Register(nameof(LeftContent), typeof(object), typeof(SplitView),
            new PropertyMetadata(null));

    public object? RightContent
    {
        get => GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }

    public static readonly DependencyProperty RightContentProperty =
        DependencyProperty.Register(nameof(RightContent), typeof(object), typeof(SplitView),
            new PropertyMetadata(null));

    public double Ratio
    {
        get => (double)GetValue(RatioProperty);
        set => SetValue(RatioProperty, CoerceRatio(value)); // Clamp locally first, then enter the DP pipeline.
    }

    public static readonly DependencyProperty RatioProperty =
        DependencyProperty.Register(nameof(Ratio), typeof(double), typeof(SplitView),
            new FrameworkPropertyMetadata(0.5,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnRatioChanged, CoerceRatioCallback));

    public double SplitterWidth
    {
        get => (double)GetValue(SplitterWidthProperty);
        set => SetValue(SplitterWidthProperty, value);
    }

    public static readonly DependencyProperty SplitterWidthProperty =
        DependencyProperty.Register(nameof(SplitterWidth), typeof(double), typeof(SplitView),
            new PropertyMetadata(4.0));

    public double MinLeftWidth
    {
        get => (double)GetValue(MinLeftWidthProperty);
        set => SetValue(MinLeftWidthProperty, value);
    }

    public static readonly DependencyProperty MinLeftWidthProperty =
        DependencyProperty.Register(nameof(MinLeftWidth), typeof(double), typeof(SplitView),
            new PropertyMetadata(120.0));

    public double MinRightWidth
    {
        get => (double)GetValue(MinRightWidthProperty);
        set => SetValue(MinRightWidthProperty, value);
    }

    public static readonly DependencyProperty MinRightWidthProperty =
        DependencyProperty.Register(nameof(MinRightWidth), typeof(double), typeof(SplitView),
            new PropertyMetadata(120.0));

    public string? PersistKey
    {
        get => (string?)GetValue(PersistKeyProperty);
        set => SetValue(PersistKeyProperty, value);
    }

    public static readonly DependencyProperty PersistKeyProperty =
        DependencyProperty.Register(nameof(PersistKey), typeof(string), typeof(SplitView),
            new PropertyMetadata(null));

    public bool AutoPersist
    {
        get => (bool)GetValue(AutoPersistProperty);
        set => SetValue(AutoPersistProperty, value);
    }

    public static readonly DependencyProperty AutoPersistProperty =
        DependencyProperty.Register(nameof(AutoPersist), typeof(bool), typeof(SplitView),
            new PropertyMetadata(true));

    #endregion
}