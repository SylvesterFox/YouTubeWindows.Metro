param([ValidateSet('x64','x86')][string]$Architecture = 'x64')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$version = (Get-Content -LiteralPath (Join-Path $root 'VERSION') -Raw).Trim()
if ($version -notmatch '^\d+\.\d+\.\d+$') { throw "Invalid VERSION: $version" }
$stage = Join-Path $root "dist\YouTubeWindows.Metro-win81-$Architecture"
if (Test-Path -LiteralPath $stage) { Remove-Item -LiteralPath $stage -Recurse -Force }
New-Item -ItemType Directory -Path $stage | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'src\YouTubeWindows.Metro\bin\Release\YouTubeWindows.Metro.exe') -Destination $stage
Copy-Item -LiteralPath (Join-Path $root 'src\YouTubeWindows.Metro.Launcher\bin\Release\YouTubeWindows.Metro.Launcher.exe') -Destination $stage
Copy-Item -LiteralPath (Join-Path $root 'src\YouTubeWindows.Metro\Assets') -Destination $stage -Recurse
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'install-dev.ps1') -Destination $stage
New-Item -ItemType Directory -Path (Join-Path $stage 'YouTubeWindows') | Out-Null
Set-Content -LiteralPath (Join-Path $stage 'config.json') -Encoding UTF8 -Value '{"YouTubeWindowsPath":"YouTubeWindows\\YouTubeWindows.exe","Arguments":"","Fullscreen":true,"CloseLauncherAfterStart":true,"LogLevel":"Information"}'
Set-Content -LiteralPath (Join-Path $stage 'YouTubeWindows\README.txt') -Encoding UTF8 -Value 'Place the TGSAN YouTubeWindows runtime files in this folder. The executable must be YouTubeWindows.exe.'
$zip = Join-Path $root "dist\YouTubeWindows.Metro-v$version-win81-$Architecture.zip"
if (Test-Path -LiteralPath $zip) { Remove-Item -LiteralPath $zip -Force }
Compress-Archive -Path (Join-Path $stage '*') -DestinationPath $zip -CompressionLevel Optimal
if ((Get-Item -LiteralPath $zip).Length -le 0) { throw "Package archive is empty: $zip" }
Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::OpenRead($zip)
try {
    $entryNames = @($archive.Entries | ForEach-Object { $_.FullName })
    foreach ($required in @('YouTubeWindows.Metro.exe', 'YouTubeWindows.Metro.Launcher.exe', 'config.json')) {
        if ($entryNames -notcontains $required) { throw "Required package file is missing from ZIP: $required" }
    }
}
finally { $archive.Dispose() }
Write-Output $zip
