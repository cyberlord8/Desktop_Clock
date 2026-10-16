namespace DesktopClock.Core;

public static class ClockCatalog
{
    public static IReadOnlyList<ClockDefinition> Default { get; } =
    [
        new("phoenix", "Phoenix", "US Mountain Standard Time"),
        new("los-angeles", "Los Angeles", "Pacific Standard Time"),
        new("denver", "Denver", "Mountain Standard Time"),
        new("chicago", "Chicago", "Central Standard Time"),
        new("new-york", "New York", "Eastern Standard Time"),
        new("toronto", "Toronto", "Eastern Standard Time"),
        new("sao-paulo", "São Paulo", "E. South America Standard Time"),
        new("london", "London", "GMT Standard Time"),
        new("paris", "Paris", "Romance Standard Time"),
        new("berlin", "Berlin", "W. Europe Standard Time"),
        new("moscow", "Moscow", "Russian Standard Time"),
        new("dubai", "Dubai", "Arabian Standard Time"),
        new("mumbai", "Mumbai", "India Standard Time"),
        new("singapore", "Singapore", "Singapore Standard Time"),
        new("beijing", "Beijing", "China Standard Time"),
        new("tokyo", "Tokyo", "Tokyo Standard Time"),
        new("seoul", "Seoul", "Korea Standard Time"),
        new("sydney", "Sydney", "AUS Eastern Standard Time"),
        new("auckland", "Auckland", "New Zealand Standard Time"),
        new("anchorage", "Anchorage", "Alaskan Standard Time"),
        new("honolulu", "Honolulu", "Hawaiian Standard Time"),
    ];

    public static ClockDefinition? Find(string id) =>
        Default.FirstOrDefault(clock => string.Equals(clock.Id, id, StringComparison.OrdinalIgnoreCase));
}
