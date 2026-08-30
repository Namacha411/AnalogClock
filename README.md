# AnalogClock

![image](https://github.com/Namacha411/AnalogClock/assets/52745962/19bcaa62-c5be-4f91-b27a-b6e78498a186)

Analog clock on your windows taskbar

## Installation

Access to the [Releases](https://github.com/Namacha411/AnalogClock/releases) page and download one of the following:

- `AnalogClock-<version>-win-x64-setup.exe` — installer. Installs to your user profile (no admin rights required) and adds a Start Menu shortcut and uninstaller.
- `AnalogClock-<version>-win-x64-portable.zip` — portable. Unzip and run `AnalogClock.exe` directly, no installation needed.

Both are self-contained (the .NET runtime is bundled, no separate runtime install required).

The binaries are not code-signed, so Windows SmartScreen may show a warning on first run ("Windows protected your PC"). Click "More info" → "Run anyway" to proceed.

## Build

```pwsh
dotnet publish -r win-x64 -c release
```

## Inspiration

- [RunCat](https://github.com/Kyome22/RunCat_for_windows/)
