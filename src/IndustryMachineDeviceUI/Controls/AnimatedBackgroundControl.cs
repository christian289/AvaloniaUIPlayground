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

    // 애니메이션 속도 제어 속성들
    public static readonly StyledProperty<double> AnimationSpeedProperty =
        AvaloniaProperty.Register<AnimatedBackgroundControl, double>(nameof(AnimationSpeed), 0.01);

    public static readonly StyledProperty<double> EaseSpeedProperty =
        AvaloniaProperty.Register<AnimatedBackgroundControl, double>(nameof(EaseSpeed), 0.03);

    public static readonly StyledProperty<int> TargetChangeIntervalProperty =
        AvaloniaProperty.Register<AnimatedBackgroundControl, int>(nameof(TargetChangeInterval), 80);

    public static readonly StyledProperty<int> FrameRateProperty =
        AvaloniaProperty.Register<AnimatedBackgroundControl, int>(nameof(FrameRate), 20);

    // 디버그 모드 속성
    public static readonly StyledProperty<bool> IsDebugModeProperty =
        AvaloniaProperty.Register<AnimatedBackgroundControl, bool>(nameof(IsDebugMode), false);

    private DispatcherTimer? _animationTimer;
    private double _currentX = 0.5; // 초기 X 위치 (중앙)
    private double _currentY = 0.5; // 초기 Y 위치 (중앙)
    private double _targetX = 0.5;  // 목표 X 위치
    private double _targetY = 0.5;  // 목표 Y 위치
    private readonly Random _random = new();
    private int _movementCounter = 0;

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

    /// <summary>
    /// 애니메이션 속도 (기본값: 0.01, 범위: 0.005 ~ 0.05)
    /// </summary>
    public double AnimationSpeed
    {
        get => GetValue(AnimationSpeedProperty);
        set => SetValue(AnimationSpeedProperty, value);
    }

    /// <summary>
    /// 움직임의 부드러움 정도 (기본값: 0.03, 범위: 0.01 ~ 0.1)
    /// 작을수록 더 부드럽게 이동
    /// </summary>
    public double EaseSpeed
    {
        get => GetValue(EaseSpeedProperty);
        set => SetValue(EaseSpeedProperty, value);
    }

    /// <summary>
    /// 목표점 변경 간격 (기본값: 80, 50ms 기준으로 약 4초)
    /// </summary>
    public int TargetChangeInterval
    {
        get => GetValue(TargetChangeIntervalProperty);
        set => SetValue(TargetChangeIntervalProperty, value);
    }

    /// <summary>
    /// 프레임 레이트 (기본값: 20 FPS)
    /// </summary>
    public int FrameRate
    {
        get => GetValue(FrameRateProperty);
        set => SetValue(FrameRateProperty, Math.Max(1, Math.Min(60, value)));
    }

    /// <summary>
    /// 디버그 모드 활성화 (격자 패턴으로 움직임 확인)
    /// </summary>
    public bool IsDebugMode
    {
        get => GetValue(IsDebugModeProperty);
        set => SetValue(IsDebugModeProperty, value);
    }

    public AnimatedBackgroundControl()
    {
        SetNewRandomTarget();
        StartAnimation();
        
        // FrameRate 속성 변경 감지
        this.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == FrameRateProperty && _animationTimer != null)
        {
            _animationTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / Math.Max(1, FrameRate));
        }
    }

    private void StartAnimation()
    {
        CreateAnimatedBrush();

        _animationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1000.0 / FrameRate)
        };
        _animationTimer.Tick += OnAnimationTick;
        _animationTimer.Start();
    }

    private void OnAnimationTick(object? sender, EventArgs e)
    {
        _movementCounter++;

        // 일정 간격마다 새로운 랜덤 목표점 설정
        if (_movementCounter >= TargetChangeInterval)
        {
            SetNewRandomTarget();
            _movementCounter = 0;
        }

        // 현재 위치에서 목표점으로 부드럽게 이동 (이징 효과)
        // EaseSpeed 속성 사용하여 부드러움 조절
        _currentX += (_targetX - _currentX) * EaseSpeed;
        _currentY += (_targetY - _currentY) * EaseSpeed;

        // 애니메이션 프로그레스 업데이트 (0~1 범위로 정규화)
        AnimationProgress = Math.Sqrt(_currentX * _currentX + _currentY * _currentY) / Math.Sqrt(2);
        
        CreateAnimatedBrush();
    }

    private void SetNewRandomTarget()
    {
        // 0.0 ~ 1.0 범위에서 랜덤 목표점 생성
        _targetX = _random.NextDouble();
        _targetY = _random.NextDouble();
    }

    private void CreateAnimatedBrush()
    {
        if (IsDebugMode)
        {
            CreateDebugBrush();
        }
        else
        {
            CreateNaturalBrush();
        }
    }

    private void CreateDebugBrush()
    {
        // 디버그 모드: 격자 패턴으로 움직임 확인
        var viewportXOffset = (_currentX - 0.5) * 100;
        var viewportYOffset = (_currentY - 0.5) * 100;

        var drawingGroup = new DrawingGroup();
        
        // 배경색
        var backgroundRect = new RectangleGeometry(new Rect(0, 0, 200, 200));
        var backgroundDrawing = new GeometryDrawing
        {
            Brush = new SolidColorBrush(Colors.DarkBlue),
            Geometry = backgroundRect
        };
        drawingGroup.Children.Add(backgroundDrawing);

        // 체크보드 격자 패턴
        var gridSize = 40;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                var isEven = (x + y) % 2 == 0;
                var color = isEven ? Colors.Red : Colors.Yellow;
                
                // 가장자리 확실히 구분되는 색상
                if (x == 0) color = Colors.Lime;           // 왼쪽 끝
                else if (x == 4) color = Colors.Magenta;    // 오른쪽 끝
                else if (y == 0) color = Colors.Cyan;       // 위쪽 끝
                else if (y == 4) color = Colors.Orange;     // 아래쪽 끝
                
                var rect = new RectangleGeometry(new Rect(x * gridSize, y * gridSize, gridSize, gridSize));
                var drawing = new GeometryDrawing
                {
                    Brush = new SolidColorBrush(color),
                    Geometry = rect
                };
                drawingGroup.Children.Add(drawing);
            }
        }

        var drawingBrush = new DrawingBrush
        {
            Drawing = drawingGroup,
            TileMode = TileMode.Tile,
            Stretch = Stretch.Fill
        };

        var translateTransform = new TranslateTransform
        {
            X = viewportXOffset,
            Y = viewportYOffset
        };
        
        drawingBrush.Transform = translateTransform;
        BackgroundBrush = drawingBrush;
    }

    private void CreateNaturalBrush()
    {
        // 극한 확대: 거대한 색상 블록들 중 극히 일부만 보이게
        var viewportXOffset = (_currentX - 0.5) * 8000; // 극도로 큰 이동 범위
        var viewportYOffset = (_currentY - 0.5) * 8000;

        // 단순한 RadialGradientBrush로 자연스러운 색상 전환
        var colors = new Color[]
        {
            Color.FromRgb(255, 0, 0),    // 빨강
            Color.FromRgb(255, 165, 0),  // 주황  
            Color.FromRgb(255, 255, 0),  // 노랑
            Color.FromRgb(0, 255, 0),    // 초록
            Color.FromRgb(0, 255, 255),  // 시안
            Color.FromRgb(0, 0, 255),    // 파랑
            Color.FromRgb(128, 0, 128),  // 보라
            Color.FromRgb(255, 0, 255)   // 자홍
        };

        // 현재 위치에 기반한 색상 선택
        var primaryColorIndex = (int)((_currentX + _currentY) * colors.Length) % colors.Length;
        var secondaryColorIndex = (primaryColorIndex + 1) % colors.Length;
        var tertiaryColorIndex = (primaryColorIndex + 2) % colors.Length;

        var primaryColor = colors[primaryColorIndex];
        var secondaryColor = colors[secondaryColorIndex];
        var tertiaryColor = colors[tertiaryColorIndex];

        // 매우 큰 RadialGradient로 부드러운 색상 전환
        var gradient = new RadialGradientBrush
        {
            // 중심점을 뷰포트 위치에 따라 이동
            Center = new RelativePoint(
                0.5 + (_currentX - 0.5) * 2.0, 
                0.5 + (_currentY - 0.5) * 2.0, 
                RelativeUnit.Relative),
            // 매우 큰 반지름으로 완전한 색상 덮개 보장
            Radius = 3.0 + Math.Abs(_currentX - 0.5) + Math.Abs(_currentY - 0.5)
        };

        // 매우 자연스러운 색상 전환 (더 많은 스탑으로 부드럽게)
        gradient.GradientStops.Add(new GradientStop(primaryColor, 0.0));
        gradient.GradientStops.Add(new GradientStop(
            Color.FromRgb(
                (byte)((primaryColor.R + secondaryColor.R) / 2),
                (byte)((primaryColor.G + secondaryColor.G) / 2),
                (byte)((primaryColor.B + secondaryColor.B) / 2)), 0.3));
        gradient.GradientStops.Add(new GradientStop(secondaryColor, 0.6));
        gradient.GradientStops.Add(new GradientStop(
            Color.FromRgb(
                (byte)((secondaryColor.R + tertiaryColor.R) / 2),
                (byte)((secondaryColor.G + tertiaryColor.G) / 2),
                (byte)((secondaryColor.B + tertiaryColor.B) / 2)), 0.8));
        gradient.GradientStops.Add(new GradientStop(tertiaryColor, 1.0));

        // 추가 LinearGradient 오버레이로 더 복잡한 패턴
        var linearGradient = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(
                -2.0 + (_currentX - 0.5) * 4.0, 
                -2.0 + (_currentY - 0.5) * 4.0, 
                RelativeUnit.Relative),
            EndPoint = new RelativePoint(
                3.0 + (_currentX - 0.5) * 4.0, 
                3.0 + (_currentY - 0.5) * 4.0, 
                RelativeUnit.Relative)
        };

        // 반투명 오버레이로 더 자연스러운 혼합
        var overlayColor1 = colors[(primaryColorIndex + 3) % colors.Length];
        var overlayColor2 = colors[(primaryColorIndex + 5) % colors.Length];
        
        linearGradient.GradientStops.Add(new GradientStop(Color.FromArgb(0, overlayColor1.R, overlayColor1.G, overlayColor1.B), 0.0));
        linearGradient.GradientStops.Add(new GradientStop(Color.FromArgb(60, overlayColor1.R, overlayColor1.G, overlayColor1.B), 0.2));
        linearGradient.GradientStops.Add(new GradientStop(Color.FromArgb(80, overlayColor2.R, overlayColor2.G, overlayColor2.B), 0.5));
        linearGradient.GradientStops.Add(new GradientStop(Color.FromArgb(60, overlayColor2.R, overlayColor2.G, overlayColor2.B), 0.8));
        linearGradient.GradientStops.Add(new GradientStop(Color.FromArgb(0, overlayColor2.R, overlayColor2.G, overlayColor2.B), 1.0));

        // DrawingGroup으로 두 그라디언트 합성
        var drawingGroup = new DrawingGroup();
        
        // 거대한 배경 영역 (화면을 완전히 덮도록)
        var backgroundRect = new RectangleGeometry(new Rect(-5000, -5000, 15000, 15000));
        
        // 기본 RadialGradient
        var radialDrawing = new GeometryDrawing
        {
            Brush = gradient,
            Geometry = backgroundRect
        };
        drawingGroup.Children.Add(radialDrawing);
        
        // 오버레이 LinearGradient
        var linearDrawing = new GeometryDrawing
        {
            Brush = linearGradient,
            Geometry = backgroundRect
        };
        drawingGroup.Children.Add(linearDrawing);

        var drawingBrush = new DrawingBrush
        {
            Drawing = drawingGroup,
            TileMode = TileMode.None,
            Stretch = Stretch.None
        };

        BackgroundBrush = drawingBrush;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _animationTimer?.Stop();
        _animationTimer = null;
    }
}
