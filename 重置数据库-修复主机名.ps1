# Script to reset AuditFlowDB database and fix hostname matching
# This will clear existing data and let services recreate with corrected hostname format
# Hostnames will be stored WITHOUT "D" prefix (e.g., "WKG9FK" instead of "DWKG9FK")

Write-Host "=== Resetting AuditFlowDB Database (Fix Hostname Format) ===" -ForegroundColor Cyan
Write-Host "`nThis will:" -ForegroundColor Yellow
Write-Host "  1. Clear existing HardwareAssets data" -ForegroundColor White
Write-Host "  2. Restart Engine service to reseed with corrected format" -ForegroundColor White
Write-Host "  3. Hostnames will be stored as 'WKG9FK' (without D prefix)" -ForegroundColor White
Write-Host "  4. SN001 will match WKG9FK (Simulator has DWKG9FK)" -ForegroundColor White
Write-Host "`nWarning: This will delete all existing data!" -ForegroundColor Yellow
Write-Host "Press Ctrl+C to cancel, or any key to continue..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

try {
    Write-Host "`nStep 1: Stopping services..." -ForegroundColor Yellow
    Get-Process | Where-Object { $_.ProcessName -like "*dotnet*" } | ForEach-Object {
        Write-Host "  Stopping: $($_.ProcessName) (PID: $($_.Id))" -ForegroundColor Gray
        Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
    }
    Start-Sleep -Seconds 2
    
    Write-Host "`nStep 2: Connecting to database..." -ForegroundColor Yellow
    $connection = New-Object System.Data.SqlClient.SqlConnection("Server=(localdb)\mssqllocaldb;Database=AuditFlowDB;Trusted_Connection=True;")
    $connection.Open()
    
    Write-Host "`nStep 3: Clearing existing HardwareAssets data..." -ForegroundColor Yellow
    $command = $connection.CreateCommand()
    $command.CommandText = "DELETE FROM HardwareAssets"
    $command.ExecuteNonQuery()
    Write-Host "  ✓ Cleared HardwareAssets" -ForegroundColor Green
    
    $connection.Close()
    
    Write-Host "`nStep 4: Starting services to reseed data..." -ForegroundColor Yellow
    Write-Host "  Starting CS_Simulator..." -ForegroundColor Gray
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'D:\github-ClientEngineereing - Web\AuditFlow.Simulator'; dotnet run" -WindowStyle Minimized
    Start-Sleep -Seconds 5
    
    Write-Host "  Starting AuditFlow_Engine..." -ForegroundColor Gray
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'D:\github-ClientEngineereing - Web\AuditFlow.Engine'; dotnet run" -WindowStyle Minimized
    Start-Sleep -Seconds 10
    
    Write-Host "`n✓ Database reset and reseeding complete!" -ForegroundColor Green
    Write-Host "`nNew format:" -ForegroundColor Cyan
    Write-Host "  SN001 -> WKG9FK (matches Simulator's DWKG9FK)" -ForegroundColor White
    Write-Host "  SN002 -> [hostname without D prefix]" -ForegroundColor White
    Write-Host "`nCheck the frontend to verify the new hostname format." -ForegroundColor Yellow
    
} catch {
    Write-Host "`n✗ Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Make sure SQL Server LocalDB is running." -ForegroundColor Yellow
}
