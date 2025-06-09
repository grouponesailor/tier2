Write-Host "Testing search for 'John Doe'..." -ForegroundColor Green

# Test MockServer directly
$response = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"John Doe","system":"111","uuid":"222"}'
$data = $response.Content | ConvertFrom-Json
$count = $data.hits.Count

Write-Host "Results: $count files found" -ForegroundColor Cyan

Write-Host "`nFiles that match:" -ForegroundColor Yellow
$data.hits | ForEach-Object { 
    Write-Host "- $($_.metadata.name) ($($_.metadata.fullNamePath))" -ForegroundColor White
}

Write-Host "`nExpected: 2 files (deployment_guide.txt and meeting_notes.txt)" -ForegroundColor Magenta

if ($count -eq 2) {
    Write-Host "✅ Search is working correctly!" -ForegroundColor Green
} elseif ($count -eq 1) {
    Write-Host "❌ Only returning 1 result instead of 2" -ForegroundColor Red
} else {
    Write-Host "❌ Unexpected result count: $count" -ForegroundColor Red
} 