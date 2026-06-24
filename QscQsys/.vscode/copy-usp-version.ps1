param(
    [string]$SIMPLPath = "${PSScriptRoot}\..\SIMPL"
)

Write-Host "Copy .usp files with new version" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

$oldVersion = Read-Host "Enter current version (e.g., v1.0.0)"
if ([string]::IsNullOrWhiteSpace($oldVersion)) {
    Write-Host "No version entered. Exiting." -ForegroundColor Red
    exit 1
}

$newVersion = Read-Host "Enter new version (e.g., v1.0.1)"
if ([string]::IsNullOrWhiteSpace($newVersion)) {
    Write-Host "No version entered. Exiting." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Looking for .usp files in: $SIMPLPath" -ForegroundColor Yellow

$uspFiles = Get-ChildItem -Path $SIMPLPath -Filter "*.usp" | Where-Object { $_.Name -like "*$oldVersion*" }

if ($uspFiles.Count -eq 0) {
    Write-Host "No .usp files found with version '$oldVersion'" -ForegroundColor Red
    exit 1
}

Write-Host "Found $($uspFiles.Count) .usp file(s) with version '$oldVersion'" -ForegroundColor Yellow
Write-Host ""

foreach ($file in $uspFiles) {
    $newName = $file.Name -replace [regex]::Escape($oldVersion), $newVersion
    $destinationPath = Join-Path $file.Directory $newName
    
    try {
        Copy-Item $file.FullName -Destination $destinationPath -Force
        Write-Host "Copied '$($file.Name)' to '$newName'" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to copy '$($file.Name)': $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Copy operation completed!" -ForegroundColor Green