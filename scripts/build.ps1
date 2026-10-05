param([switch]$SkipClean)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root 'YouTubeWindows.Metro.sln'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$msbuild = $null
$msbuildPrefix = @()
$frameworkOverride = $null
if (Test-Path -LiteralPath $vswhere) { $msbuild = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1 }
if ($msbuild) { $msbuildPrefix = @() }
else {
  $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
  if (-not $dotnet) { throw 'Visual Studio 2019 MSBuild or the .NET SDK is required.' }
  $msbuild = $dotnet.Source
  $msbuildPrefix = @('msbuild')
}
$referencePack = Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\mscorlib.dll'
if (-not (Test-Path -LiteralPath $referencePack)) {
  $runtimeRefs = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319'
  if (-not (Test-Path -LiteralPath (Join-Path $runtimeRefs 'mscorlib.dll'))) { throw 'The .NET Framework 4.5.2 targeting pack is missing; install it or build on the Windows 2019 CI image.' }
  $frameworkOverride = "/p:FrameworkPathOverride=$runtimeRefs"
  Write-Warning 'Using installed CLR 4 runtime assemblies for local compile checks. CI uses the Windows 2019 .NET Framework targeting pack.'
}
& (Join-Path $PSScriptRoot 'generate-assets.ps1')
if ($LASTEXITCODE -ne 0) { throw 'Asset generation failed.' }
if (-not $SkipClean) { & $msbuild @msbuildPrefix $solution /t:Clean /p:Configuration=Release /v:minimal $frameworkOverride; if ($LASTEXITCODE -ne 0) { throw 'Clean failed.' } }
foreach ($architecture in @('x64', 'x86')) {
  & $msbuild @msbuildPrefix $solution /restore /t:Build "/p:Configuration=Release" "/p:Platform=$architecture" /v:minimal $frameworkOverride
  if ($LASTEXITCODE -ne 0) { throw "Release $architecture build failed." }
}
& $msbuild @msbuildPrefix (Join-Path $root 'tests\YouTubeWindows.Metro.Tests.csproj') /restore /t:Build /p:Configuration=Release /p:Platform='AnyCPU' /v:minimal $frameworkOverride
if ($LASTEXITCODE -ne 0) { throw 'Tests failed to build.' }
& (Join-Path $root 'tests\bin\Release\YouTubeWindows.Metro.Tests.exe')
if ($LASTEXITCODE -ne 0) { throw 'Runtime checks failed.' }
# Lightweight runtime-independent validation of source/project contract.
$checks = @(
  (Test-Path (Join-Path $root 'src\YouTubeWindows.Metro\bin\Release\YouTubeWindows.Metro.exe')),
  (Test-Path (Join-Path $root 'src\YouTubeWindows.Metro.Launcher\bin\Release\YouTubeWindows.Metro.Launcher.exe')),
  ((Get-Content -Raw (Join-Path $root 'VERSION')).Trim() -match '^\d+\.\d+\.\d+$')
)
if ($checks -contains $false) { throw 'Validation checks failed.' }
& (Join-Path $PSScriptRoot 'package.ps1') -Architecture x64
if ($LASTEXITCODE -ne 0) { throw 'x64 packaging failed.' }
& (Join-Path $PSScriptRoot 'package.ps1') -Architecture x86
if ($LASTEXITCODE -ne 0) { throw 'x86 packaging failed.' }
Write-Output "YouTubeWindows.Metro build`n--------------------------`nVersion:       $((Get-Content (Join-Path $root 'VERSION')).Trim())`nConfiguration: Release`nArchitectures: x64, x86 (AnyCPU managed frontend)`nTarget:        Windows 8.1 / .NET Framework 4.5.2`nBuild:         SUCCESS`nTests:         SUCCESS`nPackage:       SUCCESS`nArtifacts:     dist\YouTubeWindows.Metro-v$((Get-Content (Join-Path $root 'VERSION')).Trim())-win81-x64.zip, dist\YouTubeWindows.Metro-v$((Get-Content (Join-Path $root 'VERSION')).Trim())-win81-x86.zip"
