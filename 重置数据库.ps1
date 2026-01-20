# Script to reset AuditFlowDB database
# This will clear existing data and let services recreate with new seed data

Write-Host "=== Resetting AuditFlowDB Database ===" -ForegroundColor Cyan
Write-Host "`nWarning: This will delete all existing data!" -ForegroundColor Yellow
Write-Host "Press Ctrl+C to cancel, or any key to continue..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

try {
    Write-Host "`nConnecting to database..." -ForegroundColor Yellow
    $connection = New-Object System.Data.SqlClient.SqlConnection("Server=(localdb)\mssqllocaldb;Database=AuditFlowDB;Trusted_Connection=True;")
    $connection.Open()
    
    Write-Host "Clearing existing data..." -ForegroundColor Yellow
    $command = $connection.CreateCommand()
    
    # Delete all records
    $command.CommandText = "DELETE FROM ThreatAlerts"
    $command.ExecuteNonQuery()
    Write-Host "  ✓ Cleared ThreatAlerts" -ForegroundColor Green
    
    $command.CommandText = "DELETE FROM HardwareAssets"
    $command.ExecuteNonQuery()
    Write-Host "  ✓ Cleared HardwareAssets" -ForegroundColor Green
    
    $connection.Close()
    
    Write-Host "`n✓ Database reset complete!" -ForegroundColor Green
    Write-Host "`nNext steps:" -ForegroundColor Cyan
    Write-Host "  1. Restart AuditFlow.Engine service" -ForegroundColor White
    Write-Host "  2. Restart AuditFlow.Simulator service" -ForegroundColor White
    Write-Host "  3. New seed data will be generated automatically" -ForegroundColor White
    
} catch {
    Write-Host "`n✗ Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Make sure services are stopped before running this script." -ForegroundColor Yellow
}
