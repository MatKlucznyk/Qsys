param (
    [string]$SIMPLPath = "${PSScriptRoot}\..\SIMPL",
    [string[]]$Targets = @("series3", "series4")
)

Write-Host "Looking for .usp files..."

$simplFolder = Resolve-Path $SIMPLPath
Write-Host "SIMPL folder: $simplFolder"

$uspFiles = Get-ChildItem -Path $simplFolder -Filter *.usp -File | Select-Object -ExpandProperty FullName

if (-not $uspFiles -or $uspFiles.Count -eq 0) {
    Write-Host "No .usp files found in SIMPL folder."
    exit 1
}

Write-Host "Found .usp files:"
$quotedUspFiles = $uspFiles | ForEach-Object { '"{0}"' -f $_ }
$targetsString = $Targets -join " "
$appArgs = @("\build") + $quotedUspFiles + "\target $targetsString"

Write-Host "Running SPlusCC.exe with arguments:"
Write-Host $appArgs

Start-Process -FilePath "C:\Program Files (x86)\Crestron\SIMPL\SPlusCC.exe" `
              -ArgumentList $appArgs `
              -NoNewWindow `
              -Wait
