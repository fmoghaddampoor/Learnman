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
