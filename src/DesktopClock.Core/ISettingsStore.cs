namespace DesktopClock.Core;

public interface ISettingsStore
{
    ClockSettings Load();

    void Save(ClockSettings settings);
}
