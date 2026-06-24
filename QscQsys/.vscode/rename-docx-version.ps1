param(
    [string]$SIMPLPath = "${PSScriptRoot}\..\SIMPL"
)

Write-Host "Rename .docx files with new version" -ForegroundColor Green
Write-Host "====================================" -ForegroundColor Green

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
Write-Host "Looking for .docx files with version '$oldVersion' in: $SIMPLPath" -ForegroundColor Yellow

$docxFiles = Get-ChildItem -Path $SIMPLPath -Filter "*.docx" | Where-Object { $_.Name -like "*$oldVersion*" }

if ($docxFiles.Count -eq 0) {
    Write-Host "No .docx files found with version '$oldVersion'" -ForegroundColor Red
    exit 1
}

Write-Host "Found $($docxFiles.Count) .docx file(s) with version '$oldVersion':" -ForegroundColor Yellow
foreach ($file in $docxFiles) {
    Write-Host "  - $($file.Name)" -ForegroundColor White
}

Write-Host ""
$confirm = Read-Host "Are you sure you want to rename these files? (y/N)"
if ($confirm -ne 'y' -and $confirm -ne 'Y') {
    Write-Host "Rename operation cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
foreach ($file in $docxFiles) {
    $newName = $file.Name -replace [regex]::Escape($oldVersion), $newVersion
    
    try {
        Rename-Item $file.FullName -NewName $newName
        Write-Host "Renamed '$($file.Name)' to '$newName'" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to rename '$($file.Name)': $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Rename operation completed!" -ForegroundColor Green