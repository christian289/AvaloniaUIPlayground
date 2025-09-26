using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace IndustryMachineDeviceUI.Controls;

public sealed class AnimatedBackgroundControl : ContentControl
{
    public static readonly StyledProperty<double> AnimationProgressProperty =
        AvaloniaProperty.Register<AnimatedBackgroundControl, double>(nameof(AnimationProgress));

    public static readonly StyledProperty<IBrush> BackgroundBrushProperty =
        AvaloniaProperty.Register<AnimatedBackgroundControl, IBrush>(nameof(BackgroundBrush));

    private DispatcherTimer? _animationTimer;
    private double _currentOffset = 0;
    private bool _movingRight = true;

    public double AnimationProgress
    {
        get => GetValue(AnimationProgressProperty);
        set => SetValue(AnimationProgressProperty, value);
    }

    public IBrush BackgroundBrush
    {
        get => GetValue(BackgroundBrushProperty);
        set => SetValue(BackgroundBrushProperty, value);
    }

    public AnimatedBackgroundControl()
    {
        StartAnimation();
    }

    private void StartAnimation()
    {
        CreateAnimatedBrush();

        _animationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50) // 20 FPS
        };
        _animationTimer.Tick += OnAnimationTick;
        _animationTimer.Start();
    }

    private void OnAnimationTick(object? sender, EventArgs e)
    {
        // 왔다갔다 움직이는 효과
        if (_movingRight)
        {
            _currentOffset += 0.005; // 속도 조절
            if (_currentOffset >= 1.0)
            {
                _currentOffset = 1.0;
                _movingRight = false;
            }
        }
        else
        {
            _currentOffset -= 0.005;
            if (_currentOffset <= 0.0)
            {
                _currentOffset = 0.0;
                _movingRight = true;
            }
        }

        AnimationProgress = _currentOffset;
        CreateAnimatedBrush();
    }

    private void CreateAnimatedBrush()
    {
        // 더 큰 그라디언트를 만들고 뷰포트(화면)가 그 일부를 보는 효과
        // 그라디언트는 실제로는 더 넓은 영역에 걸쳐 있고, 
        // 현재 보이는 부분은 그 중 일부입니다
        var viewportOffset = _currentOffset * 0.6; // 이동 범위 조절

        var gradient = new LinearGradientBrush
        {
            // 더 넓은 그라디언트 영역을 정의하고 뷰포트가 이동
            StartPoint = new RelativePoint(-0.8 + viewportOffset, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1.8 + viewportOffset, 1, RelativeUnit.Relative)
        };

        // 더 풍부한 그라디언트 패턴
        gradient.GradientStops.Add(new GradientStop(Color.FromRgb(0x6B, 0x73, 0xFF), 0.0));
        gradient.GradientStops.Add(new GradientStop(Color.FromRgb(0x9D, 0x50, 0xBB), 0.15));
        gradient.GradientStops.Add(new GradientStop(Color.FromRgb(0x00, 0x94, 0xFF), 0.3));
        gradient.GradientStops.Add(new GradientStop(Color.FromRgb(0x6B, 0x73, 0xFF), 0.45));
        gradient.GradientStops.Add(new GradientStop(Color.FromRgb(0x9D, 0x50, 0xBB), 0.6));
        gradient.GradientStops.Add(new GradientStop(Color.FromRgb(0x00, 0x94, 0xFF), 0.75));
        gradient.GradientStops.Add(new GradientStop(Color.FromRgb(0x6B, 0x73, 0xFF), 0.9));
        gradient.GradientStops.Add(new GradientStop(Color.FromRgb(0x9D, 0x50, 0xBB), 1.0));

        BackgroundBrush = gradient;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _animationTimer?.Stop();
        _animationTimer = null;
    }
}