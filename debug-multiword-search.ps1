Write-Host "=== Debugging Multi-Word Search Issue ===" -ForegroundColor Green

# Test 1: Check which files contain "John Doe" on disk
Write-Host "`n1. Files containing 'John Doe' on disk:" -ForegroundColor Yellow
Get-ChildItem MockServer/mockupdata/*.txt | ForEach-Object { 
    $content = Get-Content $_.FullName -Raw
    if ($content -match "John Doe") { 
        Write-Host "✅ $($_.Name) contains 'John Doe'" -ForegroundColor Green
        Write-Host "   Context: $($content -split "`n" | Where-Object { $_ -match "John Doe" } | Select-Object -First 1)" -ForegroundColor Gray
    }
}

# Test 2: Check which files contain "John" on disk  
Write-Host "`n2. Files containing 'John' on disk:" -ForegroundColor Yellow
Get-ChildItem MockServer/mockupdata/*.txt | ForEach-Object { 
    $content = Get-Content $_.FullName -Raw
    if ($content -match "John") { 
        Write-Host "✅ $($_.Name) contains 'John'" -ForegroundColor Green
    }
}

# Test 3: Test exact search variations
Write-Host "`n3. Testing exact search phrases:" -ForegroundColor Yellow

# Test "john doe" (lowercase)
Write-Host "  Testing 'john doe' (lowercase):" -ForegroundColor Cyan
$response1 = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"john doe","system":"111","uuid":"222"}'
$data1 = $response1.Content | ConvertFrom-Json
Write-Host "  Results: $($data1.hits.Count)" -ForegroundColor White

# Test "John Doe" (original case)
Write-Host "  Testing 'John Doe' (original case):" -ForegroundColor Cyan
$response2 = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"John Doe","system":"111","uuid":"222"}'
$data2 = $response2.Content | ConvertFrom-Json
Write-Host "  Results: $($data2.hits.Count)" -ForegroundColor White

# Test "JOHN DOE" (uppercase)
Write-Host "  Testing 'JOHN DOE' (uppercase):" -ForegroundColor Cyan
$response3 = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"JOHN DOE","system":"111","uuid":"222"}'
$data3 = $response3.Content | ConvertFrom-Json
Write-Host "  Results: $($data3.hits.Count)" -ForegroundColor White

# Test 4: Check if it's a space issue
Write-Host "`n4. Testing space variations:" -ForegroundColor Yellow

# Test with different spacing
Write-Host "  Testing 'John  Doe' (double space):" -ForegroundColor Cyan
$response4 = Invoke-WebRequest -Uri "http://localhost:5002/api/files" -Method POST -Headers @{"Content-Type"="application/json"} -Body '{"q":"John  Doe","system":"111","uuid":"222"}'
$data4 = $response4.Content | ConvertFrom-Json
Write-Host "  Results: $($data4.hits.Count)" -ForegroundColor White

Write-Host "`n=== Summary ===" -ForegroundColor Magenta
Write-Host "Expected: 2 files should contain 'John Doe'" -ForegroundColor Yellow
Write-Host "All case variations should return the same results" -ForegroundColor Yellow 