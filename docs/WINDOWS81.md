# Windows 8.1 compatibility

## Chosen app model

This is a classic Win32 desktop application targeting .NET Framework 4.5.2. Windows 8.1 has Windows Store apps and separate desktop applications. A Store app cannot act as a general-purpose launcher for an arbitrary desktop executable, and the modern Desktop Bridge/MSIX packaging route is not the Windows 8.1 solution. The compatible entry point is a desktop Start Menu shortcut, represented on Start as a desktop tile. The frontend is deliberately not described as UWP/Metro packaging, despite the product name.

Microsoft describes Windows 8 desktop tiles as desktop app shortcuts and documents desktop application tiles for Start: [Desktop App Tiles on the Start Screen](https://learn.microsoft.com/en-us/windows/win32/shell/desktop-app-tiles-on-the-start-screen).

## Toolchain and frameworks

- Visual Studio 2019 / MSBuild 16.
- .NET Framework 4.5.2 targeting pack.
- Windows 8.1 SDK is not required; the project uses managed WinForms APIs available in the target framework.
- AnyCPU builds are packaged separately as x86 and x64 launch bundles. The launcher itself makes no architecture-specific P/Invoke calls.

## WebView2 limits

The wrapper has no WebView2 package reference. WebView2 belongs only to upstream `YouTubeWindows.exe`. Although upstream advertises Windows 8.1 and a WebView2 runtime requirement, browser runtime support evolves independently; install a runtime version that still supports the target OS or use the upstream documented fixed runtime arrangement. The wrapper neither downloads nor selects WebView2.

## Installation limits

The installer creates a Start Menu shortcut for the current user. It cannot silently pin a tile to the user's Start Screen; users can pin the shortcut from Apps. There is no AppX registration, Store identity, splash screen package contract, or UWP full-screen lifecycle. The WinForms frontend can open maximized/borderless, but the original client owns its own fullscreen state.
