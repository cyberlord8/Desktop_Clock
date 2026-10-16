namespace DesktopClock.Core;

public sealed record ClockSettings
{
    public bool ShowDate { get; init; } = true;

    public bool AlwaysOnTop { get; init; } = true;

    public double? Left { get; init; }

    public double? Top { get; init; }

    public IReadOnlyList<string> ClockIds { get; init; } = ["phoenix"];

    public ClockSettings()
    {
    }

    public ClockSettings(
        bool showDate,
        bool alwaysOnTop,
        double? left,
        double? top,
        IReadOnlyList<string>? clockIds = null)
    {
        ShowDate = showDate;
        AlwaysOnTop = alwaysOnTop;
        Left = left;
        Top = top;
        ClockIds = clockIds ?? ["phoenix"];
    }

    public static ClockSettings Default { get; } = new();
}
