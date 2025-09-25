using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using System;

namespace IndustryMachineDeviceUI
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer? _adaptiveTimer;

        public MainWindow()
        {
            InitializeComponent();
            SetupResponsiveDesign();
            SetupKeyboardShortcuts();
        }

        private void SetupResponsiveDesign()
        {
            // 창 크기 변경에 따른 반응형 처리
            SizeChanged += OnWindowSizeChanged;

            // 주기적으로 화면 크기 체크
            _adaptiveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _adaptiveTimer.Tick += OnAdaptiveCheck;
            _adaptiveTimer.Start();
        }

        private void OnWindowSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            AdaptLayout();
        }

        private void OnAdaptiveCheck(object? sender, EventArgs e)
        {
            AdaptLayout();
        }

        private void AdaptLayout()
        {
            var width = ClientSize.Width;
            var height = ClientSize.Height;

            // 각 시계 컨트롤 찾기 및 적응형 클래스 적용
            if (this.FindControl<Grid>("MainGrid") is { } mainGrid)
            {
                foreach (var child in mainGrid.GetLogicalDescendants())
                {
                    if (child is Controls.IndustrialDashboardControl clockControl)
                    {
                        // 화면 크기에 따른 클래스 동적 변경
                        clockControl.Classes.Clear();

                        if (width < 600 || height < 400)
                        {
                            clockControl.Classes.Add("compact");
                        }
                        else if (width > 1200 && height > 800)
                        {
                            clockControl.Classes.Add("large");
                        }
                        else
                        {
                            // 기본 스타일 (클래스 없음)
                        }
                    }
                }
            }
        }

        private void SetupKeyboardShortcuts()
        {
            // F11 - 전체화면 토글
            KeyDown += (sender, e) =>
            {
                if (e.Key == Key.F11)
                {
                    WindowState = WindowState == WindowState.FullScreen
                        ? WindowState.Maximized
                        : WindowState.FullScreen;
                }
                // ESC - 전체화면 해제
                else if (e.Key == Key.Escape && WindowState == WindowState.FullScreen)
                {
                    WindowState = WindowState.Maximized;
                }
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            _adaptiveTimer?.Stop();
            _adaptiveTimer = null;
            base.OnClosed(e);
        }
    }
}