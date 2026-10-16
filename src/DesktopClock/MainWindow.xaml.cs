using System.Collections.ObjectModel;
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
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private MenuItem _showDateMenuItem = null!;
    private MenuItem _alwaysOnTopMenuItem = null!;
    private MenuItem _removeCityMenuItem = null!;
    private bool _showDate = true;
    private int _nextCatalogIndex = 1;

    public MainWindow()
    {
        InitializeComponent();
        _clocks.Add(ClockCatalog.Default[0]);
        BuildContextMenu();
        _timer.Tick += Timer_Tick;
        RenderClocks();
    }

    private void BuildContextMenu()
    {
        var menu = new ContextMenu();

        var addCity = new MenuItem { Header = "Add city" };
        addCity.Click += (_, _) => AddNextCity();

        _showDateMenuItem = new MenuItem { Header = "Show date", IsCheckable = true, IsChecked = _showDate };
        _showDateMenuItem.Click += (_, _) =>
        {
            _showDate = _showDateMenuItem.IsChecked;
            RenderClocks();
        };

        _alwaysOnTopMenuItem = new MenuItem { Header = "Always on top", IsCheckable = true, IsChecked = Topmost };
        _alwaysOnTopMenuItem.Click += (_, _) => Topmost = _alwaysOnTopMenuItem.IsChecked;

        _removeCityMenuItem = new MenuItem { Header = "Remove last city" };
        _removeCityMenuItem.Click += (_, _) => RemoveLastCity();

        var exit = new MenuItem { Header = "Exit" };
        exit.Click += (_, _) => Close();

        menu.Items.Add(addCity);
        menu.Items.Add(_showDateMenuItem);
        menu.Items.Add(_alwaysOnTopMenuItem);
        menu.Items.Add(new Separator());
        menu.Items.Add(_removeCityMenuItem);
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

        _removeCityMenuItem.IsEnabled = _clocks.Count > 1;
        _showDateMenuItem.IsChecked = _showDate;
        _alwaysOnTopMenuItem.IsChecked = Topmost;
    }

    private void AddNextCity()
    {
        while (_nextCatalogIndex < ClockCatalog.Default.Count && _clocks.Contains(ClockCatalog.Default[_nextCatalogIndex]))
        {
            _nextCatalogIndex++;
        }

        if (_nextCatalogIndex >= ClockCatalog.Default.Count) return;
        _clocks.Add(ClockCatalog.Default[_nextCatalogIndex]);
        _nextCatalogIndex++;
        RenderClocks();
    }

    private void RemoveLastCity()
    {
        if (_clocks.Count <= 1) return;
        _clocks.RemoveAt(_clocks.Count - 1);
        RenderClocks();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Left = SystemParameters.WorkArea.Right - ActualWidth - 24;
        Top = SystemParameters.WorkArea.Bottom - ActualHeight - 24;
        _timer.Start();
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        _timer.Stop();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left) return;
        try
        {
            DragMove();
        }
        catch (InvalidOperationException)
        {
            // The mouse can be released before DragMove takes ownership.
        }
    }
}
