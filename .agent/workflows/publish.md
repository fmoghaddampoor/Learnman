---
description: Publishes the application to a new versioned folder (publish_vX).
---
1. Determine the next version number based on existing directories.
2. Publish the application to the new directory.

```powershell
$directories = Get-ChildItem -Path . -Directory -Filter "publish_v*"
$maxVersion = 0
foreach ($dir in $directories) {
    if ($dir.Name -match "publish_v(\d+)") {
        $version = [int]$matches[1]
        if ($version -gt $maxVersion) {
            $maxVersion = $version
        }
    }
}
$nextVersion = $maxVersion + 1
$publishDir = "publish_v$nextVersion"

# Close App
Write-Host "Closing running applications..."
Stop-Process -Name "Learnman.TrayApp" -Force -ErrorAction SilentlyContinue
Stop-Process -Name "Learnman" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

Write-Host "Publishing to $publishDir..."
dotnet publish -c Release -o $publishDir

# Run App
Write-Host "Starting published application..."
$trayAppPath = Join-Path $publishDir "Learnman.TrayApp.exe"
if (Test-Path $trayAppPath) {
    Start-Process $trayAppPath
} else {
    Write-Warning "Could not find Learnman.TrayApp.exe in $publishDir"
}
```
