---
description: Updates .gitignore, commits all changes, and pushes to the remote repository.
---
1. Add `publish_v*`, `bin/`, `obj/`, and `.gemini/` to .gitignore if not present.
2. Stage all changes.
3. Commit with a timestamped message.
4. Push to origin.

```powershell
$gitignorePath = ".gitignore"
$entries = @("publish_v*", "bin/", "obj/", ".gemini/")

if (-not (Test-Path $gitignorePath)) {
    New-Item -Path $gitignorePath -ItemType File
}

$currentContent = Get-Content $gitignorePath -ErrorAction SilentlyContinue
foreach ($entry in $entries) {
    if ($currentContent -notcontains $entry) {
        Add-Content -Path $gitignorePath -Value $entry
        Write-Host "Added $entry to .gitignore"
    }
}

git add .
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
git commit -m "Sync: $timestamp"
git push origin HEAD
```
