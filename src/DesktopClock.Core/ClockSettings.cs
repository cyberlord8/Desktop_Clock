namespace DesktopClock.Core;

public sealed record ClockSettings(
    bool ShowDate = true,
    bool AlwaysOnTop = true,
    double? Left = null,
    double? Top = null)
{
    public static ClockSettings Default { get; } = new();
}
