namespace DesktopClock.Core;

public static class ClockCatalog
{
    public static IReadOnlyList<ClockDefinition> Default { get; } =
    [
        new("phoenix", "Phoenix", "US Mountain Standard Time"),
        new("london", "London", "GMT Standard Time"),
        new("tokyo", "Tokyo", "Tokyo Standard Time"),
        new("new-york", "New York", "Eastern Standard Time"),
        new("sydney", "Sydney", "AUS Eastern Standard Time"),
    ];

    public static ClockDefinition? Find(string id) =>
        Default.FirstOrDefault(clock => string.Equals(clock.Id, id, StringComparison.OrdinalIgnoreCase));
}
