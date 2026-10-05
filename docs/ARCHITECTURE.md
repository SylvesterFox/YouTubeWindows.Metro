# Architecture

## Upstream research

The researched upstream repository has a Visual Studio solution containing a classic WinForms `WinExe`. `YouTubeWindows.csproj` targets .NET Framework 4.5.2, has AnyCPU Release configuration, and references WebView2 SDK 1.0.1518.46 through NuGet. `Program.Main(string[] args)` passes arguments to `MainForm`. `MainForm` initializes WebView2, loads `https://www.youtube.com/tv`, has a fullscreen property implemented by removing the border and maximizing, and recognizes `--allow-auto-hdr`; other arguments are passed to WebView2 startup options. Its README lists Windows 8.1 and .NET 4.5.2+, and identifies `F11` as fullscreen.

Sources: [upstream README](https://github.com/TGSAN/YouTubeWindows/blob/master/README.md), [project](https://github.com/TGSAN/YouTubeWindows/blob/master/YouTubeWindows/YouTubeWindows.csproj), [entry point](https://github.com/TGSAN/YouTubeWindows/blob/master/YouTubeWindows/Program.cs), [MainForm](https://github.com/TGSAN/YouTubeWindows/blob/master/YouTubeWindows/MainForm.cs).

## Components

- `YouTubeWindows.Metro`: TV-oriented WinForms start screen, settings dialog, and frontend process control.
- `YouTubeWindows.Metro.Launcher`: separate WinExe that loads configuration, resolves the client path, validates it, logs, starts the client with `ProcessStartInfo`, then monitors its exit.
- `config.json`: settings beside the launcher; relative paths resolve from the package root.
- `scripts`: build, packaging, and per-user shortcut installation.
- `external/YouTubeWindows/README.md`: upstream acquisition and placement instructions; no upstream code is copied.

## Request flow

1. Windows starts the shortcut target `YouTubeWindows.Metro.exe`.
2. The frontend loads `config.json`, showing a useful settings error if malformed.
3. Start runs `YouTubeWindows.Metro.Launcher.exe` from the same directory.
4. The helper resolves the configured path against its own base directory, checks the executable exists, and launches it with its folder as working directory.
5. The helper waits for the client to exit; the frontend either stays visible or hides and closes when the helper exits.

No shell, command prompt, PowerShell, WebView2 API, or registry lookup participates in starting the upstream client.
