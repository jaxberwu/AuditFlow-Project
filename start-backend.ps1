# PowerShell script to start both backend services
# Usage: .\start-backend.ps1

Write-Host "Starting AuditFlow Backend Services..." -ForegroundColor Green

# Start Simulator in a new window
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\AuditFlow.Simulator'; dotnet run"

# Wait a bit
Start-Sleep -Seconds 2

# Start Engine in a new window
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\AuditFlow.Engine'; dotnet run"

Write-Host "Backend services started in separate windows." -ForegroundColor Green
Write-Host "Simulator: http://localhost:5001" -ForegroundColor Cyan
Write-Host "Engine: http://localhost:5002" -ForegroundColor Cyan
Write-Host ""
Write-Host "To start the frontend, run:" -ForegroundColor Yellow
Write-Host "  cd AuditFlow.UI" -ForegroundColor White
Write-Host "  npm run dev" -ForegroundColor White
