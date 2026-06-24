param (
    [string[]]$Targets = @("series3", "series4")
)

Write-Host "Looking for .usp files..."

$workspaceFolder = Resolve-Path "$PSScriptRoot\.."
Write-Host "Workspace folder: $workspaceFolder"

$uspFiles = Get-ChildItem -Path $workspaceFolder -Recurse -Filter *.usp -File | Select-Object -ExpandProperty FullName

if (-not $uspFiles -or $uspFiles.Count -eq 0) {
    Write-Host "No .usp files found in workspace."
    exit 1
}

Write-Host "Found .usp files:"
$quotedUspFiles = $uspFiles | ForEach-Object { '"{0}"' -f $_ }
$appArgs = @("\build") + $quotedUspFiles + @("\target") + $Targets


Write-Host "Running SPlusCC.exe with arguments:"
Write-Host $appArgs

Start-Process -FilePath "C:\Program Files (x86)\Crestron\SIMPL\SPlusCC.exe" `
              -ArgumentList $appArgs `
              -NoNewWindow `
              -Wait
