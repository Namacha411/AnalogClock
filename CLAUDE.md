# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A Windows tray (notification area) app that draws a live analog clock icon and shows it in the taskbar. Inspired by [RunCat](https://github.com/Kyome22/RunCat_for_windows/).

## Commands

Build/run (requires Windows + .NET 7 SDK):

```pwsh
dotnet build
dotnet run
```

Publish a self-contained single-file win-x64 executable (matches the release artifact):

```pwsh
dotnet publish -r win-x64 -c release
```

There are no tests, lint config, or CI in this repo.

## Architecture

The entire application lives in `Program.cs`. There is no main window — `Form1.cs`/`Form1.Designer.cs` are unused boilerplate left over from the WinForms project template and are not referenced by `Program.cs`.

- `Main()` first acquires a named `Mutex` (`_ANALOG_CLOCK_MUTEX`) to enforce single-instance execution; if another instance already holds it, the process exits immediately.
- `AnalogClockApplicationContext` (an `ApplicationContext`, not a `Form`) drives the whole app via a `NotifyIcon`:
  - `GenerateAnalogClockIcon(DateTime)` rasterizes a 48x48 clock face (hour hand, minute hand, center dot) into a `Bitmap` and converts it to an `Icon` via `GetHicon()`.
  - A `Timer` (`animateTimer`, 1000ms interval) ticks `AnimationTick`, which regenerates the icon and updates the tooltip text (`HH:mm`) every second.
  - The context menu (`ContextMenuStrip`) offers "Startup" (toggle run-at-login), a version label, and "Exit".
  - Run-at-login is implemented by reading/writing the `Software\Microsoft\Windows\CurrentVersion\Run` registry key under `HKEY_CURRENT_USER` (`IsStartupEnabled`/`SetStartup`), keyed by `Application.ProductName`.

Project targets `net7.0-windows` with `UseWindowsForms=true`; publish settings (`PublishSingleFile`, fixed `RuntimeIdentifier=win-x64`, `SelfContained=false`) are set in `AnalogClock.csproj` and back the GitHub Releases distribution described in the README.
