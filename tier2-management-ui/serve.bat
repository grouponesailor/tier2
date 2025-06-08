@echo off
echo Installing Node.js dependencies...
npm install

echo.
echo Starting Angular development server on port 4202...
echo Open your browser to: http://localhost:4202
echo.
echo The application will run with Hebrew RTL interface
echo Available routes:
echo - http://localhost:4202/dashboard (Hebrew Dashboard)
echo - http://localhost:4202/files (File Management - planned)
echo - http://localhost:4202/users (User Management - planned)
echo - http://localhost:4202/queues (Queue Management - planned)
echo.

ng serve --port 4202 --host 0.0.0.0 