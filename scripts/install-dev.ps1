param([string]$InstallDirectory)
$ErrorActionPreference = 'Stop'
if ([String]::IsNullOrWhiteSpace($InstallDirectory)) {
    if (Test-Path -LiteralPath (Join-Path $PSScriptRoot 'YouTubeWindows.Metro.exe')) { $InstallDirectory = $PSScriptRoot }
    else {
        $repoRoot = Split-Path -Parent $PSScriptRoot
        $InstallDirectory = Join-Path $repoRoot 'dist\YouTubeWindows.Metro-win81-x64'
    }
}
$os = [Environment]::OSVersion.Version
if ($os.Major -lt 6 -or ($os.Major -eq 6 -and $os.Minor -lt 3)) { throw 'This installer targets Windows 8.1 or later.' }
$front = Join-Path $InstallDirectory 'YouTubeWindows.Metro.exe'
$launcher = Join-Path $InstallDirectory 'YouTubeWindows.Metro.Launcher.exe'
$client = Join-Path $InstallDirectory 'YouTubeWindows\YouTubeWindows.exe'
if (-not (Test-Path -LiteralPath $front)) { $front = Join-Path $InstallDirectory 'src\YouTubeWindows.Metro\bin\Release\YouTubeWindows.Metro.exe' }
if (-not (Test-Path -LiteralPath $front)) { throw 'YouTubeWindows.Metro.exe was not found. Build or extract the runtime package first.' }
if (-not (Test-Path -LiteralPath $launcher) -and -not (Test-Path (Join-Path $InstallDirectory 'src\YouTubeWindows.Metro.Launcher\bin\Release\YouTubeWindows.Metro.Launcher.exe'))) { throw 'YouTubeWindows.Metro.Launcher.exe was not found.' }
if (-not (Test-Path -LiteralPath $client)) { Write-Warning 'YouTubeWindows.exe is missing; install the upstream client before launching YouTube TV.' }
if (Test-Path -LiteralPath $client) {
    $stream = [System.IO.File]::OpenRead($client)
    try {
        $reader = New-Object System.IO.BinaryReader -ArgumentList $stream
        $stream.Position = 0x3c
        $peOffset = $reader.ReadInt32()
        $stream.Position = $peOffset
        if ($reader.ReadUInt32() -ne 0x00004550) { throw 'YouTubeWindows.exe has an invalid PE signature.' }
        $machineId = $reader.ReadUInt16()
        $clientArch = switch ($machineId) { 0x014c { 'x86' } 0x8664 { 'x64' } 0xaa64 { 'ARM64' } default { 'unknown' } }
        if ($clientArch -eq 'unknown') { throw "Unsupported client executable architecture: 0x$('{0:X4}' -f $machineId)" }
        if ($clientArch -eq 'x64' -and -not [Environment]::Is64BitOperatingSystem) { throw 'An x64 YouTubeWindows client cannot run on 32-bit Windows.' }
        Write-Output "Detected upstream client architecture: $clientArch"
    }
    finally { $stream.Dispose() }
}
$machine = if ([Environment]::Is64BitOperatingSystem) { 'x64 OS' } else { 'x86 OS' }
Write-Output "Detected $machine. The managed frontend and launcher are AnyCPU."
$programs = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs'
New-Item -ItemType Directory -Path $programs -Force | Out-Null
$shortcutPath = Join-Path $programs 'YouTube TV.lnk'
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $front
$shortcut.WorkingDirectory = Split-Path -Parent $front
$shortcut.Description = 'Start YouTube TV'
$shortcut.IconLocation = "$front,0"
$shortcut.Save()
Write-Output "Created current-user shortcut: $shortcutPath"
Write-Output 'To add a Start Screen tile, locate YouTube TV in Apps and choose Pin to Start.'
