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
            // â ũ�� ���濡 ���� ������ ó��
            SizeChanged += OnWindowSizeChanged;

            // �ֱ������� ȭ�� ũ�� üũ
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

            // �� �ð� ��Ʈ�� ã�� �� ������ Ŭ���� ����
            if (this.FindControl<Grid>("MainGrid") is { } mainGrid)
            {
                foreach (var child in mainGrid.GetLogicalDescendants())
                {
                    if (child is Controls.IndustrialDashboardControl clockControl)
                    {
                        // ȭ�� ũ�⿡ ���� Ŭ���� ���� ����
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
                            // �⺻ ��Ÿ�� (Ŭ���� ����)
                        }
                    }
                }
            }
        }

        private void SetupKeyboardShortcuts()
        {
            // F11 - ��üȭ�� ���
            KeyDown += (sender, e) =>
            {
                if (e.Key == Key.F11)
                {
                    WindowState = WindowState == WindowState.FullScreen
                        ? WindowState.Maximized
                        : WindowState.FullScreen;
                }
                // ESC - ��üȭ�� ����
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