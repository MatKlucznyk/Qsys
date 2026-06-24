param(
    [string]$SIMPLPath = "${PSScriptRoot}\..\SIMPL"
)

Write-Host "Delete files with specific version" -ForegroundColor Red
Write-Host "==================================" -ForegroundColor Red

$version = Read-Host "Enter version to delete (e.g., v1.0.0)"
if ([string]::IsNullOrWhiteSpace($version)) {
    Write-Host "No version entered. Exiting." -ForegroundColor Red
    exit 1
}

$extensions = @("*.umc", "*.usp", "*.ush", "*.um2", "*.umc.ASV", "*_archive.zip", "*.pdf")

Write-Host ""
Write-Host "Looking for files with version '$version' in: $SIMPLPath" -ForegroundColor Yellow

$filesToDelete = @()
foreach ($ext in $extensions) {
    $pattern = "*$version$ext"
    $files = Get-ChildItem -Path $SIMPLPath -Filter $pattern
    $filesToDelete += $files
}

if ($filesToDelete.Count -eq 0) {
    Write-Host "No files found with version '$version'" -ForegroundColor Yellow
    exit 0
}

Write-Host "Found $($filesToDelete.Count) file(s) to delete:" -ForegroundColor Yellow
foreach ($file in $filesToDelete) {
    Write-Host "  - $($file.Name)" -ForegroundColor White
}

Write-Host ""
$confirm = Read-Host "Are you sure you want to delete these files? (y/N)"
if ($confirm -ne 'y' -and $confirm -ne 'Y') {
    Write-Host "Delete operation cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
foreach ($file in $filesToDelete) {
    try {
        Remove-Item $file.FullName -Force
        Write-Host "Deleted '$($file.Name)'" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to delete '$($file.Name)': $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Delete operation completed!" -ForegroundColor Green