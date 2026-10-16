using DesktopClock.Core;

var tests = new (string Name, Action Test)[]
{
    ("formats a reading in the supplied time zone", FormatsReading),
    ("keeps the city and time-zone identity", PreservesIdentity),
    ("fails when the time-zone resolver cannot resolve a zone", FailsForUnknownZone),
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

static void AssertEqual<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"Expected '{expected}', got '{actual}'.");
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
