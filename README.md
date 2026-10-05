# YouTubeWindows.Metro

[![Build](https://github.com/SylvesterFox/YouTubeWindows.Metro/actions/workflows/build.yml/badge.svg)](https://github.com/SylvesterFox/YouTubeWindows.Metro/actions/workflows/build.yml)

Windows 8.1 friendly Start Screen launcher for [TGSAN/YouTubeWindows](https://github.com/TGSAN/YouTubeWindows). **YouTubeWindows.Metro is not a new YouTube client.** It provides a TV sized desktop start screen and starts the existing client.

## Features

- Keyboard and remote friendly Start, Settings, and Exit controls.
- Win32 desktop shortcut suitable for pinning to the Windows 8.1 Start Screen.
- Separate launcher process, relative runtime paths, configurable client arguments, and a local log.
- x86 and x64 packaging of AnyCPU .NET Framework 4.5.2 code.

## Architecture

Windows 8.1 does not provide the later Desktop Bridge/MSIX model for wrapping a desktop program in a Store app. A Store app also cannot simply start an arbitrary Win32 executable. This project therefore uses a regular WinForms desktop application and a Start Menu shortcut (which can be pinned as a desktop tile):

```text
Windows 8.1 Start Screen desktop tile
  -> YouTubeWindows.Metro.exe (WinForms TV-style start screen)
  -> YouTubeWindows.Metro.Launcher.exe
  -> YouTubeWindows/YouTubeWindows.exe (original WinForms/WebView2 client)
```

The Metro name is retained as the requested project identity; it is not an AppX/UWP package and does not claim immersive Store-app behavior.

## Requirements

- Windows 8.1 x86 or x64 for the target runtime.
- Visual Studio 2019 with .NET Framework 4.5.2 targeting pack for development.
- TGSAN YouTubeWindows files and a compatible WebView2 Runtime. The upstream README lists .NET Framework 4.5.2+, Windows 7/8.1/10/11, x86/x64/ARM64, and WebView2 as requirements.

## Installation

1. Download or build the ZIP and extract it to a user-writable folder.
2. Obtain the client separately from [TGSAN/YouTubeWindows](https://github.com/TGSAN/YouTubeWindows) and copy its runtime contents to `YouTubeWindows/` beside the launcher.
3. Run `scripts/install-dev.ps1` from an elevated PowerShell only if installing the Start Menu shortcut for all users; without elevation it creates it for the current user.
4. Pin **YouTube TV** from the Start Screen's Apps view. Windows controls the actual pin operation.

## Build

On a Visual Studio 2019 developer machine:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/build.ps1
```

The script cleans, restores, builds Release x64 and x86, runs the lightweight project checks, packages both architectures, and writes ZIPs under `dist/`. See [docs/BUILD.md](docs/BUILD.md).

## Development

Open `YouTubeWindows.Metro.sln` in Visual Studio 2019. The launcher intentionally references no WebView2 assembly. The original client's WebView2 runtime, browser flags, fullscreen behavior and TV interaction remain the upstream client's responsibility.

## Configuration

`config.json` lives beside the two executables and is created with defaults on first save. `YouTubeWindowsPath` may be absolute or relative to the package root. `Arguments` is passed verbatim to the upstream client. `Fullscreen` controls the Metro-named WinForms start screen. `CloseLauncherAfterStart` hides that screen while the child client runs. `LogLevel` labels entries in `logs/launcher.log`.

## Windows 8.1 compatibility

The application targets .NET Framework 4.5.2 and uses WinForms, ProcessStartInfo, and DataContractJsonSerializer. Both x86 and x64 are built as AnyCPU assemblies and packaged separately. WebView2 compatibility depends on the browser runtime available on the target machine; see [docs/WINDOWS81.md](docs/WINDOWS81.md).

## TGSAN YouTubeWindows integration

The integration is file based. Place the upstream distribution under `YouTubeWindows/` or select `YouTubeWindows.exe` in Settings. The launcher sets the client's directory as its working directory, so upstream relative runtime files remain discoverable. It does not rewrite, bundle, or patch upstream sources.

## Automated Builds and Releases

GitHub Actions builds and uploads ZIP artifacts on pushes, pull requests, and manual dispatch. A `v*` tag builds and creates a GitHub Release with architecture-specific archives. See [docs/CI-CD.md](docs/CI-CD.md).

## Troubleshooting

- **Client not found:** restore the upstream folder or browse to `YouTubeWindows.exe` in Settings.
- **WebView2 missing:** install a WebView2 Runtime supported by the Windows 8.1 machine, or use an upstream-supported fixed runtime.
- **No Metro/Store experience:** expected. Windows 8.1 uses a desktop Start tile that launches this Win32 frontend.
- **Start tile not pinned:** locate the shortcut in Apps and choose Pin to Start; the installer creates a shortcut but Windows owns pinning.

## Known limitations

- No AppX manifest, Store deployment, immersive lifecycle, or live tile; those cannot serve as a generic Win32 launcher on Windows 8.1.
- `Fullscreen` applies to the frontend. The upstream client controls its own fullscreen behavior.
- `YouTubeWindows.exe`, its runtime, and WebView2 are not included in this repository or release archives.
- Windows 8.1 and its WebView2 support are legacy; validate runtime compatibility on the exact machine.

## Roadmap

- [x] Project structure, launcher, configuration, packaging, CI and documentation
- [x] Windows 8.1 compatible desktop Start Screen tile architecture
- [ ] TV controller support and process monitoring/automatic restart
- [ ] Tested Windows 8.1 x86 and x64 hardware coverage.
