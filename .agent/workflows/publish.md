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
Write-Host "Publishing to $publishDir..."
dotnet publish -c Release -o $publishDir
```
