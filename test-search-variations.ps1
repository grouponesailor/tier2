Write-Host "=== Testing Search Variations for 'John Doe' ===" -ForegroundColor Green

# Test 1: Search for "John Doe" (full phrase)
Write-Host "`n1. Testing 'John Doe' (full phrase):" -ForegroundColor Yellow
$response1 = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"John Doe","system":"111","uuid":"222"}'
$data1 = $response1.Content | ConvertFrom-Json
Write-Host "Results: $($data1.hits.Count)" -ForegroundColor Cyan
$data1.hits | ForEach-Object { Write-Host "- $($_.metadata.name)" -ForegroundColor White }

# Test 2: Search for "John" (single word)
Write-Host "`n2. Testing 'John' (single word):" -ForegroundColor Yellow
$response2 = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"John","system":"111","uuid":"222"}'
$data2 = $response2.Content | ConvertFrom-Json
Write-Host "Results: $($data2.hits.Count)" -ForegroundColor Cyan
$data2.hits | ForEach-Object { Write-Host "- $($_.metadata.name)" -ForegroundColor White }

# Test 3: Search for "Doe" (single word)
Write-Host "`n3. Testing 'Doe' (single word):" -ForegroundColor Yellow
$response3 = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"Doe","system":"111","uuid":"222"}'
$data3 = $response3.Content | ConvertFrom-Json
Write-Host "Results: $($data3.hits.Count)" -ForegroundColor Cyan
$data3.hits | ForEach-Object { Write-Host "- $($_.metadata.name)" -ForegroundColor White }

# Test 4: Check exact content of both files
Write-Host "`n4. Checking file contents:" -ForegroundColor Yellow
Write-Host "meeting_notes.txt first line:" -ForegroundColor Cyan
Get-Content "MockServer/mockupdata/meeting_notes.txt" | Select-Object -First 1

Write-Host "`ndeployment_guide.txt grep for 'John':" -ForegroundColor Cyan
Get-Content "MockServer/mockupdata/deployment_guide.txt" | Select-String "John"

Write-Host "`n=== Analysis ===" -ForegroundColor Magenta
if ($data2.hits.Count -gt $data1.hits.Count) {
    Write-Host "Issue confirmed: Single word 'John' returns more results than 'John Doe'" -ForegroundColor Red
    Write-Host "This suggests the search doesn't handle multi-word queries properly" -ForegroundColor Yellow
} else {
    Write-Host "Multi-word search seems to work correctly" -ForegroundColor Green
} 