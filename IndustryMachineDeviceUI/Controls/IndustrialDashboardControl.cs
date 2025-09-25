using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using System;

namespace IndustryMachineDeviceUI.Controls;

public sealed class IndustrialDashboardControl : TemplatedControl
{
    public static readonly StyledProperty<string> CurrentTimeProperty =
        AvaloniaProperty.Register<IndustrialDashboardControl, string>(nameof(CurrentTime));

    public static readonly StyledProperty<string> CurrentDateProperty =
        AvaloniaProperty.Register<IndustrialDashboardControl, string>(nameof(CurrentDate));

    public static readonly StyledProperty<string> DayOfWeekProperty =
        AvaloniaProperty.Register<IndustrialDashboardControl, string>(nameof(DayOfWeek));

    private DispatcherTimer? _timer;

    public string CurrentTime
    {
        get => GetValue(CurrentTimeProperty);
        set => SetValue(CurrentTimeProperty, value);
    }

    public string CurrentDate
    {
        get => GetValue(CurrentDateProperty);
        set => SetValue(CurrentDateProperty, value);
    }

    public string DayOfWeek
    {
        get => GetValue(DayOfWeekProperty);
        set => SetValue(DayOfWeekProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        StartTimer();
    }

    private void StartTimer()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += OnTimerTick;
        _timer.Start();

        // 초기값 설정
        UpdateTime();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        var now = DateTime.Now;
        CurrentTime = now.ToString("HH:mm:ss");
        CurrentDate = now.ToString("yyyy-MM-dd");
        DayOfWeek = now.ToString("dddd");
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _timer?.Stop();
        _timer = null;
    }
}