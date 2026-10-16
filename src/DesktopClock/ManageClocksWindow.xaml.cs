using System.Collections.ObjectModel;
using System.Windows;
using DesktopClock.Core;

namespace DesktopClock;

public partial class ManageClocksWindow : Window
{
    private readonly ObservableCollection<ClockDefinition> _clocks = [];

    public IReadOnlyList<ClockDefinition> SelectedClocks => _clocks;

    public ManageClocksWindow(IEnumerable<ClockDefinition> clocks)
    {
        InitializeComponent();

        foreach (var clock in clocks) _clocks.Add(clock);
        ClockList.ItemsSource = _clocks;
        RefreshCityPicker();
        ClockList.SelectedIndex = _clocks.Count > 0 ? 0 : -1;
        UpdateActionState();
    }

    private void RefreshCityPicker()
    {
        var selectedIds = _clocks.Select(clock => clock.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        CityPicker.ItemsSource = ClockCatalog.Default.Where(clock => !selectedIds.Contains(clock.Id)).ToList();
        CityPicker.SelectedIndex = CityPicker.Items.Count > 0 ? 0 : -1;
        UpdateActionState();
    }

    private void AddCity_Click(object sender, RoutedEventArgs e)
    {
        if (CityPicker.SelectedItem is not ClockDefinition city) return;

        _clocks.Add(city);
        ClockList.SelectedIndex = _clocks.Count - 1;
        RefreshCityPicker();
    }

    private void MoveUp_Click(object sender, RoutedEventArgs e)
    {
        MoveSelected(-1);
    }

    private void MoveDown_Click(object sender, RoutedEventArgs e)
    {
        MoveSelected(1);
    }

    private void MoveSelected(int direction)
    {
        if (ClockList.SelectedItem is not ClockDefinition selected) return;

        var index = _clocks.IndexOf(selected);
        var nextIndex = index + direction;
        if (nextIndex < 0 || nextIndex >= _clocks.Count) return;

        _clocks.Move(index, nextIndex);
        ClockList.SelectedIndex = nextIndex;
        UpdateActionState();
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (_clocks.Count <= 1 || ClockList.SelectedItem is not ClockDefinition selected) return;

        var index = _clocks.IndexOf(selected);
        _clocks.Remove(selected);
        ClockList.SelectedIndex = Math.Min(index, _clocks.Count - 1);
        RefreshCityPicker();
    }

    private void ClockList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        UpdateActionState();
    }

    private void CityPicker_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        UpdateActionState();
    }

    private void UpdateActionState()
    {
        var index = ClockList.SelectedIndex;
        MoveUpButton.IsEnabled = index > 0;
        MoveDownButton.IsEnabled = index >= 0 && index < _clocks.Count - 1;
        RemoveButton.IsEnabled = _clocks.Count > 1 && index >= 0;
        AddCityButton.IsEnabled = CityPicker.SelectedItem is ClockDefinition;
    }

    private void Done_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
