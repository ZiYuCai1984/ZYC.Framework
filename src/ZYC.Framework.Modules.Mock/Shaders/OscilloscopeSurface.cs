using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace ZYC.Framework.Modules.Mock.Shaders;

internal sealed class OscilloscopeSurface : FrameworkElement
{
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly Queue<TrailFrame> _history = new();

    private double[] _samples = Array.Empty<double>();

    public OscilloscopeSurface()
    {
        Loaded += (_, _) => CompositionTarget.Rendering += OnRendering;
        Unloaded += (_, _) => CompositionTarget.Rendering -= OnRendering;

        SnapsToDevicePixels = true;
    }

    public int SampleCount { get; set; } = 512;

    public int TrailCount { get; set; } = 10;

    private void OnRendering(object? sender, EventArgs e)
    {
        if (ActualWidth <= 0 || ActualHeight <= 0)
        {
            return;
        }

        var t = _clock.Elapsed.TotalSeconds;

        _samples = CreateDemoSamples(SampleCount, t);

        var geometry = BuildGeometry(_samples, RenderSize);
        _history.Enqueue(new TrailFrame(geometry, t));

        while (_history.Count > TrailCount)
        {
            _history.Dequeue();
        }

        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);

        var rect = new Rect(RenderSize);

        DrawBackground(dc, rect);
        DrawGrid(dc, rect);
        DrawTrails(dc);
        DrawSweep(dc, rect);
    }

    private static void DrawBackground(DrawingContext dc, Rect rect)
    {
        var background = new SolidColorBrush(Color.FromRgb(3, 10, 3));
        background.Freeze();

        dc.DrawRectangle(background, null, rect);
    }

    private static void DrawGrid(DrawingContext dc, Rect rect)
    {
        const int majorX = 10;
        const int majorY = 8;
        const int minor = 5;

        var minorPen = new Pen(
            new SolidColorBrush(Color.FromArgb(24, 80, 180, 80)), 1);
        var majorPen = new Pen(
            new SolidColorBrush(Color.FromArgb(50, 100, 220, 100)), 1);

        minorPen.Freeze();
        majorPen.Freeze();

        // Minor vertical
        for (var i = 0; i <= majorX * minor; i++)
        {
            var x = rect.Left + rect.Width * i / (majorX * minor);
            dc.DrawLine(minorPen, new Point(x, rect.Top), new Point(x, rect.Bottom));
        }

        // Minor horizontal
        for (var i = 0; i <= majorY * minor; i++)
        {
            var y = rect.Top + rect.Height * i / (majorY * minor);
            dc.DrawLine(minorPen, new Point(rect.Left, y), new Point(rect.Right, y));
        }

        // Major vertical
        for (var i = 0; i <= majorX; i++)
        {
            var x = rect.Left + rect.Width * i / majorX;
            dc.DrawLine(majorPen, new Point(x, rect.Top), new Point(x, rect.Bottom));
        }

        // Major horizontal
        for (var i = 0; i <= majorY; i++)
        {
            var y = rect.Top + rect.Height * i / majorY;
            dc.DrawLine(majorPen, new Point(rect.Left, y), new Point(rect.Right, y));
        }

        var centerPen = new Pen(
            new SolidColorBrush(Color.FromArgb(80, 120, 255, 120)), 1.2);
        centerPen.Freeze();

        var centerX = rect.Left + rect.Width / 2;
        var centerY = rect.Top + rect.Height / 2;

        dc.DrawLine(centerPen, new Point(centerX, rect.Top), new Point(centerX, rect.Bottom));
        dc.DrawLine(centerPen, new Point(rect.Left, centerY), new Point(rect.Right, centerY));
    }

    private void DrawTrails(DrawingContext dc)
    {
        if (_history.Count == 0)
        {
            return;
        }

        var frames = _history.ToArray();

        for (var i = 0; i < frames.Length; i++)
        {
            var k = (i + 1.0) / frames.Length;

            var alpha = (byte)(12 + k * 70);
            var thickness = 1.0 + k * 1.6;

            var pen = new Pen(
                new SolidColorBrush(Color.FromArgb(alpha, 80, 255, 80)),
                thickness);

            pen.Freeze();

            dc.DrawGeometry(null, pen, frames[i].Geometry);
        }

        var current = frames[^1].Geometry;

        var glow1 = new Pen(
            new SolidColorBrush(Color.FromArgb(35, 120, 255, 120)), 7);
        glow1.Freeze();

        var glow2 = new Pen(
            new SolidColorBrush(Color.FromArgb(90, 150, 255, 150)), 3);
        glow2.Freeze();

        var core = new Pen(
            new SolidColorBrush(Color.FromArgb(255, 210, 255, 210)), 1.2);
        core.Freeze();

        dc.DrawGeometry(null, glow1, current);
        dc.DrawGeometry(null, glow2, current);
        dc.DrawGeometry(null, core, current);
    }

    private void DrawSweep(DrawingContext dc, Rect rect)
    {
        var t = _clock.Elapsed.TotalSeconds;

        var x = t * rect.Width * 0.45 % rect.Width;
        var sweepRect = new Rect(x - 12, rect.Top, 24, rect.Height);

        var brush = new LinearGradientBrush();
        brush.StartPoint = new Point(0, 0);
        brush.EndPoint = new Point(1, 0);
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 100, 255, 100), 0));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(40, 140, 255, 140), 0.5));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 100, 255, 100), 1));
        brush.Freeze();

        dc.DrawRectangle(brush, null, sweepRect);
    }

    private static StreamGeometry BuildGeometry(double[] samples, Size size)
    {
        var geometry = new StreamGeometry();

        if (samples.Length == 0 || size.Width <= 0 || size.Height <= 0)
        {
            return geometry;
        }

        var midY = size.Height / 2;
        var amplitude = size.Height * 0.35;

        using var ctx = geometry.Open();

        for (var i = 0; i < samples.Length; i++)
        {
            var x = size.Width * i / (samples.Length - 1.0);
            var y = midY - samples[i] * amplitude;

            var point = new Point(x, y);

            if (i == 0)
            {
                ctx.BeginFigure(point, false, false);
            }
            else
            {
                ctx.LineTo(point, true, false);
            }
        }

        geometry.Freeze();
        return geometry;
    }

    private static double[] CreateDemoSamples(int count, double t)
    {
        var data = new double[count];

        for (var i = 0; i < count; i++)
        {
            var x = i / (double)(count - 1);

            var y =
                0.65 * Math.Sin((x * 4.0 + t * 0.35) * Math.PI * 2.0) +
                0.20 * Math.Sin((x * 17.0 - t * 1.20) * Math.PI * 2.0) +
                0.08 * Math.Sin((x * 41.0 + t * 2.80) * Math.PI * 2.0);

            y += 0.015 * Math.Sin((x * 211.0 + t * 33.0) * Math.PI * 2.0);

            data[i] = Math.Clamp(y, -1.0, 1.0);
        }

        return data;
    }

    private sealed record TrailFrame(Geometry Geometry, double Time);
}