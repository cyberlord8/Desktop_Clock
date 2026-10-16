# Desktop Clock

A small native WPF desktop widget for displaying local time and a configurable stack of city clocks.

## Features

- Floating, draggable clock widget
- Always-on-top behavior
- Multiple city clocks with configurable order
- Optional date display
- Per-user settings persisted between launches
- A framework-independent core library for time-zone and settings logic

This project is currently focused on Windows because the desktop host uses WPF.

## Requirements

- Windows
- .NET 10 SDK to build and run from source

The published self-contained executable does not require a separate .NET installation.

## Run from source

From the repository root:

```powershell
dotnet run --project src/DesktopClock/DesktopClock.csproj
```

## Run the tests

The test project is a small console executable rather than a test-framework project. Run it with:

```powershell
dotnet run --project tests/DesktopClock.Core.Tests/DesktopClock.Core.Tests.csproj
```

## Create a self-contained Windows release

```powershell
dotnet publish src/DesktopClock/DesktopClock.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  --output publish/DesktopClock-win-x64
```

The `publish/` directory is intentionally ignored by Git. Upload release packages through GitHub Releases instead of committing generated binaries to the source repository.

## Settings

The application stores its settings per user at:

```text
%APPDATA%\DesktopClock\settings.json
```

## Repository layout

```text
src/DesktopClock.Core/       Core clock, time-zone, and settings logic
src/DesktopClock/            WPF desktop application
tests/                       Executable tests for the core library
prototype/                   Exploratory browser prototypes
docs/adr/                    Architecture decision records
GLOSSARY.md                  Project terminology
```

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
