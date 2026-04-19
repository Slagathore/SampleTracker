@echo off
echo Starting SampleTracker...

start "Backend" cmd /k "cd /d %~dp0backend\SampleTracker.API && dotnet run --launch-profile http"
start "Frontend" cmd /k "cd /d %~dp0frontend\sample-tracker-ui && npm run dev"

echo Backend: http://localhost:5294/swagger
echo Frontend: http://localhost:5173
