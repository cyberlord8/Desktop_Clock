using System.Text.Json;

namespace DesktopClock.Core;

public sealed class JsonClockSettingsStore : ISettingsStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly string _filePath;

    public JsonClockSettingsStore(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("A settings file path is required.", nameof(filePath));
        }

        _filePath = filePath;
    }

    public ClockSettings Load()
    {
        if (!File.Exists(_filePath)) return ClockSettings.Default;

        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<ClockSettings>(json, SerializerOptions) ?? ClockSettings.Default;
        }
        catch (JsonException)
        {
            return ClockSettings.Default;
        }
        catch (IOException)
        {
            return ClockSettings.Default;
        }
        catch (UnauthorizedAccessException)
        {
            return ClockSettings.Default;
        }
    }

    public void Save(ClockSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(settings, SerializerOptions);
        var temporaryPath = $"{_filePath}.{Guid.NewGuid():N}.tmp";

        try
        {
            File.WriteAllText(temporaryPath, json);
            File.Move(temporaryPath, _filePath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
    }
}
