namespace DesktopClock.Core;

public sealed record ClockReading(
    string City,
    string TimeText,
    string DateText,
    string TimeZoneId);
