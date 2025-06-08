@echo off
echo Starting Tier 2 Management System...
echo.

echo Starting Mock Collaboration Server on port 5001/5002...
start "Mock Server" powershell -NoExit -Command "cd MockServer; dotnet run"

echo Waiting 5 seconds...
timeout /t 5 /nobreak > nul

echo Starting Main API Server on port 5000...
start "Main API" powershell -NoExit -Command "dotnet run"

echo Waiting 5 seconds...
timeout /t 5 /nobreak > nul

echo Starting Angular Frontend on port 4200...
start "Angular Frontend" powershell -NoExit -Command "cd tier2-management-ui; npm install; npm start"

echo.
echo All servers are starting...
echo.
echo URLs:
echo - Mock Server: http://localhost:5002 (HTTP) / https://localhost:5001 (HTTPS)
echo - Main API: http://localhost:5000
echo - Angular Frontend: http://localhost:4200
echo.
echo Press any key to exit...
pause > nul 