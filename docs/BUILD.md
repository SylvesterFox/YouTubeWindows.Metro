# Build

## Prerequisites

Visual Studio 2019 Build Tools with MSBuild and .NET Framework 4.5.2 targeting pack, PowerShell, and Git. Network access is not needed to build the wrapper after tool installation; it does not restore external NuGet packages.

## Build and package

```powershell
powershell -ExecutionPolicy Bypass -File scripts/build.ps1
```

This generates the tile artwork, cleans generated outputs, restores the solution, builds the Release managed binaries, runs the configuration/path smoke checks, stages x64 and x86-labelled runtime bundles, and creates ZIPs in `dist/`. The frontend and helper are AnyCPU managed executables; both archives contain the same binaries and can run on either OS architecture. Build only: `msbuild YouTubeWindows.Metro.sln /restore /p:Configuration=Release /p:Platform="Any CPU"`.

Artifacts intentionally omit the upstream client and WebView2 runtime. Add the client under the staged `YouTubeWindows/` folder after packaging if preparing a local installation.
