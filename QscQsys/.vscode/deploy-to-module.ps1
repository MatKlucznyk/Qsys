param(
    [string]$SIMPLPath = "${PSScriptRoot}\..\SIMPL",
    [string]$ModulePath = "${PSScriptRoot}\..\Module"
)

Write-Host "Deploy compiled program to Module folder" -ForegroundColor Green
Write-Host "=======================================" -ForegroundColor Green

# Step 1: Find all compiled zip files
Write-Host "Looking for compiled zip files in: $SIMPLPath" -ForegroundColor Yellow
$compiledZips = Get-ChildItem -Path $SIMPLPath -Filter "*_compiled.zip"

if ($compiledZips.Count -eq 0) {
    Write-Host "No compiled zip files found in SIMPL folder" -ForegroundColor Red
    exit 1
}

Write-Host "Found $($compiledZips.Count) compiled zip file(s):" -ForegroundColor Yellow
for ($i = 0; $i -lt $compiledZips.Count; $i++) {
    Write-Host "  [$($i + 1)] $($compiledZips[$i].Name)" -ForegroundColor White
}
Write-Host "  [A] ALL programs" -ForegroundColor White

# Step 2: Ask user to select which program to deploy
Write-Host ""
do {
    $selection = Read-Host "Enter the number of the program to deploy (1-$($compiledZips.Count)) or 'A' for ALL"
    if ($selection.ToUpper() -eq "A") {
        $deployAll = $true
        break
    }
    $selectionNum = $null
    if ([int]::TryParse($selection, [ref]$selectionNum) -and $selectionNum -ge 1 -and $selectionNum -le $compiledZips.Count) {
        $deployAll = $false
        break
    }
    Write-Host "Invalid selection. Please enter a number between 1 and $($compiledZips.Count) or 'A' for ALL" -ForegroundColor Red
} while ($true)

if ($deployAll) {
    Write-Host ""
    Write-Host "Selected: ALL programs" -ForegroundColor Green
    Write-Host "Will deploy $($compiledZips.Count) compiled zip files" -ForegroundColor Green
    
    # Step 3: Empty Module folder except .gitkeep
    Write-Host ""
    Write-Host "Emptying Module folder (keeping .gitkeep)..." -ForegroundColor Yellow

    $moduleFiles = Get-ChildItem -Path $ModulePath | Where-Object { $_.Name -ne ".gitkeep" }
    if ($moduleFiles.Count -gt 0) {
        Write-Host "Removing $($moduleFiles.Count) file(s) from Module folder..." -ForegroundColor Yellow
        foreach ($file in $moduleFiles) {
            try {
                Remove-Item $file.FullName -Force -Recurse
                Write-Host "  Removed: $($file.Name)" -ForegroundColor Gray
            }
            catch {
                Write-Host "  Failed to remove '$($file.Name)': $_" -ForegroundColor Red
            }
        }
    } else {
        Write-Host "Module folder is already empty (except .gitkeep)" -ForegroundColor Gray
    }

    # Step 4: Copy and extract all compiled zips to Module folder
    Write-Host ""
    Write-Host "Deploying all compiled programs..." -ForegroundColor Yellow
    
    foreach ($zipFile in $compiledZips) {
        Write-Host ""
        Write-Host "Processing: $($zipFile.Name)" -ForegroundColor Cyan
        
        # Copy zip to Module folder
        try {
            Copy-Item $zipFile.FullName -Destination $ModulePath -Force
            Write-Host "  Copied: $($zipFile.Name)" -ForegroundColor Green
        }
        catch {
            Write-Host "  Failed to copy '$($zipFile.Name)': $_" -ForegroundColor Red
            continue
        }

        # Extract zip file
        $zipPath = Join-Path $ModulePath $zipFile.Name
        try {
            Expand-Archive -Path $zipPath -DestinationPath $ModulePath -Force
            Write-Host "  Successfully extracted zip contents" -ForegroundColor Green
        }
        catch {
            Write-Host "  Failed to extract zip file: $_" -ForegroundColor Red
            continue
        }

        # Clean up files with program name in filename (except the _compiled.zip)
        $programName = $zipFile.BaseName -replace "_compiled$", ""
        $filesToDelete = Get-ChildItem -Path $ModulePath | Where-Object { 
            $_.Name -like "*$programName*" -and 
            $_.Name -ne $zipFile.Name -and 
            $_.Name -ne ".gitkeep"
        }

        if ($filesToDelete.Count -gt 0) {
            Write-Host "  Removing $($filesToDelete.Count) file(s) with program name..." -ForegroundColor Yellow
            foreach ($file in $filesToDelete) {
                try {
                    Remove-Item $file.FullName -Force
                    Write-Host "    Removed: $($file.Name)" -ForegroundColor Gray
                }
                catch {
                    Write-Host "    Failed to remove '$($file.Name)': $_" -ForegroundColor Red
                }
            }
        }
    }
} else {
    $selectedZip = $compiledZips[$selectionNum - 1]
    $programName = $selectedZip.BaseName -replace "_compiled$", ""

    Write-Host ""
    Write-Host "Selected: $($selectedZip.Name)" -ForegroundColor Green
    Write-Host "Program name: $programName" -ForegroundColor Green

    # Step 3: Empty Module folder except .gitkeep
    Write-Host ""
    Write-Host "Emptying Module folder (keeping .gitkeep)..." -ForegroundColor Yellow

    $moduleFiles = Get-ChildItem -Path $ModulePath | Where-Object { $_.Name -ne ".gitkeep" }
    if ($moduleFiles.Count -gt 0) {
        Write-Host "Removing $($moduleFiles.Count) file(s) from Module folder..." -ForegroundColor Yellow
        foreach ($file in $moduleFiles) {
            try {
                Remove-Item $file.FullName -Force -Recurse
                Write-Host "  Removed: $($file.Name)" -ForegroundColor Gray
            }
            catch {
                Write-Host "  Failed to remove '$($file.Name)': $_" -ForegroundColor Red
            }
        }
    } else {
        Write-Host "Module folder is already empty (except .gitkeep)" -ForegroundColor Gray
    }

    # Step 4: Copy compiled zip to Module folder
    Write-Host ""
    Write-Host "Copying compiled zip to Module folder..." -ForegroundColor Yellow
    try {
        Copy-Item $selectedZip.FullName -Destination $ModulePath -Force
        Write-Host "Copied: $($selectedZip.Name)" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to copy '$($selectedZip.Name)': $_" -ForegroundColor Red
        exit 1
    }

    # Step 5: Unzip the file in Module folder
    Write-Host ""
    Write-Host "Extracting zip file..." -ForegroundColor Yellow
    $zipPath = Join-Path $ModulePath $selectedZip.Name
    try {
        Expand-Archive -Path $zipPath -DestinationPath $ModulePath -Force
        Write-Host "Successfully extracted zip contents" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to extract zip file: $_" -ForegroundColor Red
        exit 1
    }

    # Step 6: Delete any files with program name in filename (except the _compiled.zip)
    Write-Host ""
    Write-Host "Cleaning up files with program name in filename (except _compiled.zip)..." -ForegroundColor Yellow

    $filesToDelete = Get-ChildItem -Path $ModulePath | Where-Object { 
        $_.Name -like "*$programName*" -and 
        $_.Name -ne $selectedZip.Name -and 
        $_.Name -ne ".gitkeep"
    }

    if ($filesToDelete.Count -gt 0) {
        Write-Host "Removing $($filesToDelete.Count) file(s) with program name..." -ForegroundColor Yellow
        foreach ($file in $filesToDelete) {
            try {
                Remove-Item $file.FullName -Force
                Write-Host "  Removed: $($file.Name)" -ForegroundColor Gray
            }
            catch {
                Write-Host "  Failed to remove '$($file.Name)': $_" -ForegroundColor Red
            }
        }
    } else {
        Write-Host "No files with program name found to clean up" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "Deployment completed successfully!" -ForegroundColor Green
Write-Host "Module folder now contains:" -ForegroundColor Yellow
$finalFiles = Get-ChildItem -Path $ModulePath
foreach ($file in $finalFiles) {
    Write-Host "  - $($file.Name)" -ForegroundColor White
}