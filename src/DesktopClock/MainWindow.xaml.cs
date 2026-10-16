using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DesktopClock.Core;

namespace DesktopClock;

public partial class MainWindow : Window
{
    private static readonly Brush PrimaryText = new SolidColorBrush(Color.FromRgb(245, 247, 251));
    private static readonly Brush SecondaryText = new SolidColorBrush(Color.FromRgb(166, 176, 194));
    private static readonly Brush AccentText = new SolidColorBrush(Color.FromRgb(131, 229, 209));

    private readonly ObservableCollection<ClockDefinition> _clocks = [];
    private readonly ClockTimeService _timeService = new();
    private readonly ISettingsStore _settingsStore = new JsonClockSettingsStore(GetSettingsPath());
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private MenuItem _showDateMenuItem = null!;
    private MenuItem _alwaysOnTopMenuItem = null!;
    private bool _showDate = true;
    private ClockSettings _settings = ClockSettings.Default;

    public MainWindow()
    {
        InitializeComponent();
        _settings = _settingsStore.Load();
        _showDate = _settings.ShowDate;
        Topmost = _settings.AlwaysOnTop;
        foreach (var clockId in _settings.ClockIds ?? [])
        {
            var clock = ClockCatalog.Find(clockId);
            if (clock is not null && !_clocks.Contains(clock)) _clocks.Add(clock);
        }

        if (_clocks.Count == 0) _clocks.Add(ClockCatalog.Default[0]);
        BuildContextMenu();
        _timer.Tick += Timer_Tick;
        RenderClocks();
    }

    private void BuildContextMenu()
    {
        var menu = new ContextMenu();

        var manageClocks = new MenuItem { Header = "Manage clocks..." };
        manageClocks.Click += (_, _) => ManageClocks();

        _showDateMenuItem = new MenuItem { Header = "Show date", IsCheckable = true, IsChecked = _showDate };
        _showDateMenuItem.Click += (_, _) =>
        {
            _showDate = _showDateMenuItem.IsChecked;
            RenderClocks();
            SaveSettings();
        };

        _alwaysOnTopMenuItem = new MenuItem { Header = "Always on top", IsCheckable = true, IsChecked = Topmost };
        _alwaysOnTopMenuItem.Click += (_, _) =>
        {
            Topmost = _alwaysOnTopMenuItem.IsChecked;
            SaveSettings();
        };

        var exit = new MenuItem { Header = "Exit" };
        exit.Click += (_, _) => Close();

        menu.Items.Add(manageClocks);
        menu.Items.Add(_showDateMenuItem);
        menu.Items.Add(_alwaysOnTopMenuItem);
        menu.Items.Add(new Separator());
        menu.Items.Add(exit);
        ContextMenu = menu;
    }

    private void Timer_Tick(object? sender, EventArgs e) => RenderClocks();

    private void RenderClocks()
    {
        ClockStack.Children.Clear();

        foreach (var clock in _clocks)
        {
            var reading = _timeService.GetReading(clock, DateTimeOffset.Now);
            var row = new Border
            {
                Padding = new Thickness(0, 2, 0, 2),
                Background = Brushes.Transparent,
                ToolTip = $"{reading.City} · {reading.TimeZoneId}",
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var city = new TextBlock
            {
                Text = reading.City.ToUpperInvariant(),
                Foreground = SecondaryText,
                FontSize = 10,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 12, 0),
            };

            var readingStack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
            var timeText = new TextBlock
            {
                Text = reading.TimeText,
                Foreground = PrimaryText,
                FontSize = 25,
                FontWeight = FontWeights.Light,
                FontFamily = new FontFamily("Segoe UI"),
                LineHeight = 25,
            };
            TextOptions.SetTextFormattingMode(timeText, TextFormattingMode.Display);
            readingStack.Children.Add(timeText);

            if (_showDate)
            {
                readingStack.Children.Add(new TextBlock
                {
                    Text = reading.DateText,
                    Foreground = AccentText,
                    FontSize = 9,
                    HorizontalAlignment = HorizontalAlignment.Right,
                });
            }

            Grid.SetColumn(city, 0);
            Grid.SetColumn(readingStack, 1);
            grid.Children.Add(city);
            grid.Children.Add(readingStack);
            row.Child = grid;
            row.MouseLeftButtonDown += Window_MouseLeftButtonDown;
            ClockStack.Children.Add(row);
        }

        _showDateMenuItem.IsChecked = _showDate;
        _alwaysOnTopMenuItem.IsChecked = Topmost;
    }

    private void ManageClocks()
    {
        var dialog = new ManageClocksWindow(_clocks.ToArray())
        {
            Owner = this,
        };

        if (dialog.ShowDialog() != true) return;

        _clocks.Clear();
        foreach (var clock in dialog.SelectedClocks) _clocks.Add(clock);
        RenderClocks();
        SaveSettings();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (HasVisibleSavedPosition())
        {
            Left = _settings.Left!.Value;
            Top = _settings.Top!.Value;
        }
        else
        {
            Left = SystemParameters.WorkArea.Right - ActualWidth - 24;
            Top = SystemParameters.WorkArea.Bottom - ActualHeight - 24;
        }

        _timer.Start();
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        _timer.Stop();
        SaveSettings();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left) return;
        try
        {
            DragMove();
            SaveSettings();
        }
        catch (InvalidOperationException)
        {
            // The mouse can be released before DragMove takes ownership.
        }
    }

    private bool HasVisibleSavedPosition()
    {
        if (!_settings.Left.HasValue || !_settings.Top.HasValue) return false;

        var left = _settings.Left.Value;
        var top = _settings.Top.Value;
        var right = SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth;
        var bottom = SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight;

        return left < right && left + ActualWidth > SystemParameters.VirtualScreenLeft
            && top < bottom && top + ActualHeight > SystemParameters.VirtualScreenTop;
    }

    private void SaveSettings()
    {
        double? left = double.IsNaN(Left) ? null : Left;
        double? top = double.IsNaN(Top) ? null : Top;
        _settings = new ClockSettings(_showDate, Topmost, left, top, _clocks.Select(clock => clock.Id).ToArray());
        _settingsStore.Save(_settings);
    }

    private static string GetSettingsPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "DesktopClock", "settings.json");
    }
}
