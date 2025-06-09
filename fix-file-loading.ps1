Write-Host "=== Investigating File Loading Issue ===" -ForegroundColor Green

# Get all files that should be loaded
Write-Host "`n1. Files in mockupdata directory:" -ForegroundColor Yellow
$diskFiles = Get-ChildItem MockServer/mockupdata/*.txt | Sort-Object Name
$diskFiles | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor White }
Write-Host "  Total: $($diskFiles.Count) files" -ForegroundColor Cyan

# Test the search system to see what files are actually loaded
Write-Host "`n2. Files found by search system (empty query):" -ForegroundColor Yellow
$response = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"","system":"111","uuid":"222"}'
$data = $response.Content | ConvertFrom-Json
$searchFiles = $data.hits | Sort-Object { $_.metadata.name }
$searchFiles | ForEach-Object { Write-Host "  - $($_.metadata.name).txt" -ForegroundColor White }
Write-Host "  Total: $($searchFiles.Count) files" -ForegroundColor Cyan

# Find missing files
Write-Host "`n3. Comparison:" -ForegroundColor Yellow
$diskFileNames = $diskFiles | ForEach-Object { $_.BaseName }
$searchFileNames = $searchFiles | ForEach-Object { $_.metadata.name }

$missingFiles = $diskFileNames | Where-Object { $_ -notin $searchFileNames }
if ($missingFiles) {
    Write-Host "❌ Missing files from search system:" -ForegroundColor Red
    $missingFiles | ForEach-Object { Write-Host "  - $_.txt" -ForegroundColor Red }
} else {
    Write-Host "✅ All files are loaded correctly" -ForegroundColor Green
}

# Check files that contain "John"
Write-Host "`n4. Files containing 'John':" -ForegroundColor Yellow
Write-Host "  On disk:" -ForegroundColor Cyan
$diskFiles | ForEach-Object { 
    $content = Get-Content $_.FullName -Raw
    if ($content -match "John") { 
        Write-Host "    - $($_.Name)" -ForegroundColor White
    }
}

Write-Host "  Found by search:" -ForegroundColor Cyan
$johnResponse = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"John","system":"111","uuid":"222"}'
$johnData = $johnResponse.Content | ConvertFrom-Json
$johnData.hits | ForEach-Object { Write-Host "    - $($_.metadata.name).txt" -ForegroundColor White } 