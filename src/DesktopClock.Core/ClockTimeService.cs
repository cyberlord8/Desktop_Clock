using System.Globalization;

namespace DesktopClock.Core;

public sealed class ClockTimeService
{
    private readonly Func<string, TimeZoneInfo> _timeZoneResolver;

    public ClockTimeService(Func<string, TimeZoneInfo>? timeZoneResolver = null)
    {
        _timeZoneResolver = timeZoneResolver ?? TimeZoneInfo.FindSystemTimeZoneById;
    }

    public ClockReading GetReading(ClockDefinition clock, DateTimeOffset instant)
    {
        ArgumentNullException.ThrowIfNull(clock);

        var timeZone = _timeZoneResolver(clock.TimeZoneId);
        var localTime = TimeZoneInfo.ConvertTime(instant, timeZone);

        return new ClockReading(
            clock.City,
            localTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture),
            localTime.ToString("ddd, MMM d", CultureInfo.CurrentCulture),
            clock.TimeZoneId);
    }
}
