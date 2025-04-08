@echo off
cd /d "%~dp0"
start "" "https://localhost:7279/swagger/index.html"
dotnet run --urls "https://localhost:7279;http://localhost:5030"
pause
