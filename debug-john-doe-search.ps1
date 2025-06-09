Write-Host "=== Debugging 'John Doe' Search ===" -ForegroundColor Green

# First, let's see all files without search
Write-Host "`n1. Getting all files (no search):" -ForegroundColor Yellow
$allResponse = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"","system":"111","uuid":"222"}'
$allData = $allResponse.Content | ConvertFrom-Json
Write-Host "Total files found: $($allData.hits.Count)" -ForegroundColor Cyan

# Search without classification filter
Write-Host "`n2. Searching 'John Doe' WITHOUT classification filter:" -ForegroundColor Yellow
$noFilterResponse = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"John Doe","system":"111","uuid":"222"}'
$noFilterData = $noFilterResponse.Content | ConvertFrom-Json
Write-Host "Results without filter: $($noFilterData.hits.Count)" -ForegroundColor Cyan
$noFilterData.hits | ForEach-Object { 
    Write-Host "- $($_.metadata.name) (Auth: $($_.metadata.authorizationLevel))" -ForegroundColor White
}

# Search with classification filter (this is what normally happens)
Write-Host "`n3. Searching 'John Doe' WITH hide_classified filter:" -ForegroundColor Yellow
$response = Invoke-WebRequest -Uri "http://localhost:5002/api/files?classification=hide_classified" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"John Doe","system":"111","uuid":"222"}'
$data = $response.Content | ConvertFrom-Json
Write-Host "Results with filter: $($data.hits.Count)" -ForegroundColor Cyan
$data.hits | ForEach-Object { 
    Write-Host "- $($_.metadata.name) (Auth: $($_.metadata.authorizationLevel))" -ForegroundColor White
}

# Check which files contain "John Doe" in the file system
Write-Host "`n4. Files containing 'John Doe' on disk:" -ForegroundColor Yellow
Get-ChildItem MockServer/mockupdata/*.txt | ForEach-Object { 
    $content = Get-Content $_.FullName -Raw
    if ($content -match "John Doe") { 
        Write-Host "- $($_.Name) contains 'John Doe'" -ForegroundColor Green
    }
}

Write-Host "`n=== Analysis ===" -ForegroundColor Magenta
if ($noFilterData.hits.Count -gt $data.hits.Count) {
    Write-Host "Classification filter is hiding some results!" -ForegroundColor Red
    Write-Host "   Files without filter: $($noFilterData.hits.Count)" -ForegroundColor Yellow
    Write-Host "   Files with filter: $($data.hits.Count)" -ForegroundColor Yellow
} else {
    Write-Host "Classification filter is not the issue" -ForegroundColor Green
    if ($data.hits.Count -eq 1) {
        Write-Host "The issue might be in the search logic itself" -ForegroundColor Yellow
    }
} 