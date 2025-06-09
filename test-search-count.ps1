Write-Host "Testing search endpoints for multiple results..." -ForegroundColor Green

Write-Host "`n1. Testing MockServer directly:" -ForegroundColor Yellow
$mockResponse = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"project","system":"111","uuid":"222"}'
$mockData = $mockResponse.Content | ConvertFrom-Json
$mockCount = $mockData.hits.Count
Write-Host "MockServer returned $mockCount results" -ForegroundColor Cyan

Write-Host "`n2. Testing Main API (should call MockServer):" -ForegroundColor Yellow
$mainResponse = Invoke-WebRequest -Uri "http://localhost:5000/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"project","system":"111","uuid":"222"}'
$mainData = $mainResponse.Content | ConvertFrom-Json
$mainCount = $mainData.hits.Count
Write-Host "Main API returned $mainCount results" -ForegroundColor Cyan

Write-Host "`n3. Testing empty query (should return all files):" -ForegroundColor Yellow
$allResponse = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"","system":"111","uuid":"222"}'
$allData = $allResponse.Content | ConvertFrom-Json
$allCount = $allData.hits.Count
Write-Host "MockServer returned $allCount files for empty query" -ForegroundColor Cyan

Write-Host "`n4. Results Summary:" -ForegroundColor Green
Write-Host "- MockServer 'project' search: $mockCount files"
Write-Host "- Main API 'project' search: $mainCount files"
Write-Host "- MockServer all files: $allCount files"

if ($mockCount -gt 1) {
    Write-Host "`n✅ MockServer is correctly returning multiple results!" -ForegroundColor Green
} else {
    Write-Host "`n❌ MockServer is only returning $mockCount result(s)" -ForegroundColor Red
}

if ($mainCount -eq $mockCount) {
    Write-Host "✅ Main API is correctly forwarding to MockServer!" -ForegroundColor Green
} else {
    Write-Host "❌ Main API returned different count than MockServer" -ForegroundColor Red
} 