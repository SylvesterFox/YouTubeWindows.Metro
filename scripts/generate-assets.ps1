$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$folder = Join-Path $root 'src\YouTubeWindows.Metro\Assets'
New-Item -ItemType Directory -Path $folder -Force | Out-Null
Add-Type -AssemblyName System.Drawing
function Draw-Asset([string]$Name, [int]$Width, [int]$Height, [bool]$Text) {
    $bitmap = New-Object -TypeName System.Drawing.Bitmap -ArgumentList @($Width, $Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.Clear([System.Drawing.Color]::FromArgb(24,24,24))
    $size = [Math]::Min($Width, $Height) * 0.32
    $cx = $Width * 0.5; $cy = $Height * 0.5
    $rect = New-Object -TypeName System.Drawing.RectangleF -ArgumentList @(($cx - $size), ($cy - $size), ($size * 2), ($size * 2))
    $brush = New-Object -TypeName System.Drawing.SolidBrush -ArgumentList ([System.Drawing.Color]::FromArgb(238,0,0))
    $graphics.FillEllipse($brush, $rect)
    $points = [System.Drawing.PointF[]]@(
      (New-Object -TypeName System.Drawing.PointF -ArgumentList @(($cx - $size * 0.25), ($cy - $size * 0.45))),
      (New-Object -TypeName System.Drawing.PointF -ArgumentList @(($cx - $size * 0.25), ($cy + $size * 0.45))),
      (New-Object -TypeName System.Drawing.PointF -ArgumentList @(($cx + $size * 0.48), $cy))
    )
    $white = New-Object -TypeName System.Drawing.SolidBrush -ArgumentList ([System.Drawing.Color]::White)
    $graphics.FillPolygon($white, $points)
    if ($Text) {
      $fontSize = [Math]::Max(16, [Math]::Min($Width, $Height) * 0.085)
      $font = New-Object -TypeName System.Drawing.Font -ArgumentList @('Segoe UI', $fontSize, [System.Drawing.FontStyle]::Regular)
      $format = New-Object -TypeName System.Drawing.StringFormat
      $format.Alignment = [System.Drawing.StringAlignment]::Center; $format.LineAlignment = [System.Drawing.StringAlignment]::Far
      $textRect = New-Object -TypeName System.Drawing.RectangleF -ArgumentList @(0, 0, $Width, ($Height * 0.94))
      $graphics.DrawString('YouTube TV', $font, $white, $textRect, $format)
      $font.Dispose(); $format.Dispose()
    }
    $bitmap.Save((Join-Path $folder $Name), [System.Drawing.Imaging.ImageFormat]::Png)
    $white.Dispose(); $brush.Dispose(); $graphics.Dispose(); $bitmap.Dispose()
}
Draw-Asset 'Logo.png' 150 150 $true
Draw-Asset 'SmallLogo.png' 30 30 $false
Draw-Asset 'WideLogo.png' 310 150 $true
Draw-Asset 'SplashScreen.png' 620 300 $true
$iconBitmap = New-Object -TypeName System.Drawing.Bitmap -ArgumentList @(256, 256)
$iconGraphics = [System.Drawing.Graphics]::FromImage($iconBitmap)
$iconGraphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$iconGraphics.Clear([System.Drawing.Color]::FromArgb(24,24,24))
$redBrush = New-Object -TypeName System.Drawing.SolidBrush -ArgumentList ([System.Drawing.Color]::FromArgb(238,0,0))
$whiteBrush = New-Object -TypeName System.Drawing.SolidBrush -ArgumentList ([System.Drawing.Color]::White)
$iconGraphics.FillEllipse($redBrush, 28, 28, 200, 200)
$triangle = [System.Drawing.Point[]]@((New-Object -TypeName System.Drawing.Point -ArgumentList @(100,76)), (New-Object -TypeName System.Drawing.Point -ArgumentList @(100,180)), (New-Object -TypeName System.Drawing.Point -ArgumentList @(184,128)))
$iconGraphics.FillPolygon($whiteBrush, $triangle)
$nativeIcon = [System.Drawing.Icon]::FromHandle($iconBitmap.GetHicon())
$stream = [System.IO.File]::Create((Join-Path $folder 'YouTubeWindows.Metro.ico'))
$nativeIcon.Save($stream); $stream.Dispose(); $nativeIcon.Dispose(); $iconGraphics.Dispose(); $iconBitmap.Dispose()
$redBrush.Dispose(); $whiteBrush.Dispose()
