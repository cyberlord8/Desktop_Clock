using DesktopClock.Core;

var tests = new (string Name, Action Test)[]
{
    ("formats a reading in the supplied time zone", FormatsReading),
    ("keeps the city and time-zone identity", PreservesIdentity),
    ("fails when the time-zone resolver cannot resolve a zone", FailsForUnknownZone),
    ("contains unique cities with valid time zones", ValidatesClockCatalog),
    ("round-trips settings through JSON", RoundTripsSettings),
    ("uses defaults when the settings file is missing", UsesDefaultsWhenMissing),
    ("uses defaults when the settings file is malformed", UsesDefaultsWhenMalformed),
};

var failures = 0;
foreach (var (name, test) in tests)
{
    try
    {
        test();
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception error)
    {
        failures++;
        Console.WriteLine($"FAIL {name}: {error.Message}");
    }
}

return failures == 0 ? 0 : 1;

static void FormatsReading()
{
    var zone = TimeZoneInfo.CreateCustomTimeZone("Test +02", TimeSpan.FromHours(2), "Test +02", "Test +02");
    var service = new ClockTimeService(_ => zone);
    var clock = new ClockDefinition("test", "Test City", zone.Id);
    var instant = new DateTimeOffset(2026, 10, 16, 12, 34, 56, TimeSpan.Zero);

    var reading = service.GetReading(clock, instant);

    AssertEqual("14:34:56", reading.TimeText);
    AssertEqual("Fri, Oct 16", reading.DateText);
}

static void PreservesIdentity()
{
    var zone = TimeZoneInfo.Utc;
    var service = new ClockTimeService(_ => zone);
    var clock = new ClockDefinition("utc", "UTC", "UTC");

    var reading = service.GetReading(clock, DateTimeOffset.UnixEpoch);

    AssertEqual("UTC", reading.City);
    AssertEqual("UTC", reading.TimeZoneId);
}

static void FailsForUnknownZone()
{
    var service = new ClockTimeService(_ => throw new TimeZoneNotFoundException("missing"));
    var clock = new ClockDefinition("missing", "Missing", "missing");

    AssertThrows<TimeZoneNotFoundException>(() => service.GetReading(clock, DateTimeOffset.UtcNow));
}

static void ValidatesClockCatalog()
{
    AssertEqual(ClockCatalog.Default.Count, ClockCatalog.Default.Select(clock => clock.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count());

    foreach (var clock in ClockCatalog.Default)
    {
        _ = TimeZoneInfo.FindSystemTimeZoneById(clock.TimeZoneId);
    }
}

static void RoundTripsSettings()
{
    WithTemporarySettingsPath(path =>
    {
        var store = new JsonClockSettingsStore(path);
        var expected = new ClockSettings(false, false, 128.5, 642.25, ["phoenix", "tokyo"], ClockLayout.Horizontal);

        store.Save(expected);

        var actual = store.Load();
        AssertEqual(expected.ShowDate, actual.ShowDate);
        AssertEqual(expected.AlwaysOnTop, actual.AlwaysOnTop);
        AssertEqual(expected.Layout, actual.Layout);
        AssertEqual(expected.Left, actual.Left);
        AssertEqual(expected.Top, actual.Top);
        AssertSequenceEqual(expected.ClockIds, actual.ClockIds);
    });
}

static void UsesDefaultsWhenMissing()
{
    WithTemporarySettingsPath(path =>
    {
        var settings = new JsonClockSettingsStore(path).Load();

        AssertEqual(ClockSettings.Default.ShowDate, settings.ShowDate);
        AssertEqual(ClockSettings.Default.AlwaysOnTop, settings.AlwaysOnTop);
        AssertSequenceEqual(ClockSettings.Default.ClockIds, settings.ClockIds);
    });
}

static void UsesDefaultsWhenMalformed()
{
    WithTemporarySettingsPath(path =>
    {
        File.WriteAllText(path, "{ this is not valid JSON }");

        var settings = new JsonClockSettingsStore(path).Load();

        AssertEqual(ClockSettings.Default.ShowDate, settings.ShowDate);
        AssertEqual(ClockSettings.Default.AlwaysOnTop, settings.AlwaysOnTop);
        AssertSequenceEqual(ClockSettings.Default.ClockIds, settings.ClockIds);
    });
}

static void WithTemporarySettingsPath(Action<string> test)
{
    var directory = Path.Combine(Environment.CurrentDirectory, ".test-data");
    Directory.CreateDirectory(directory);
    var path = Path.Combine(directory, $"desktop-clock-settings-{Guid.NewGuid():N}.json");
    try
    {
        test(path);
    }
    finally
    {
        if (File.Exists(path)) File.Delete(path);
        if (Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any()) Directory.Delete(directory);
    }
}

static void AssertEqual<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"Expected '{expected}', got '{actual}'.");
    }
}

static void AssertSequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
{
    if (!expected.SequenceEqual(actual))
    {
        throw new InvalidOperationException($"Expected '{string.Join(", ", expected)}', got '{string.Join(", ", actual)}'.");
    }
}

static void AssertThrows<TException>(Action action) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
}
