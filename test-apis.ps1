# AuditFlow API 测试脚本 / AuditFlow API Test Script
# 功能: 测试 Simulator 和 Engine 的 API 端点 / Function: Test API endpoints for Simulator and Engine

Write-Host "`n=== AuditFlow API 测试 ===" -ForegroundColor Cyan
Write-Host "=== AuditFlow API Testing ===" -ForegroundColor Cyan

# 测试 Simulator - 威胁摘要 / Test Simulator - Threat Summary
Write-Host "`n1. 测试 Simulator - 威胁摘要 / Testing Simulator - Threat Summary..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5001/api/v1/threats/summary" -UseBasicParsing -ErrorAction Stop
    $json = $response.Content | ConvertFrom-Json
    Write-Host "✓ 成功 / Success!" -ForegroundColor Green
    Write-Host "  总威胁数 / Total Threats: $($json.totalThreats)" -ForegroundColor White
    Write-Host "  总主机数 / Total Hosts: $($json.totalHosts)" -ForegroundColor White
    if ($json.hostThreatCounts -and $json.hostThreatCounts.Count -gt 0) {
        Write-Host "  示例主机 / Example Host: $($json.hostThreatCounts[0].hostname) - $($json.hostThreatCounts[0].threatCount) 威胁 / threats" -ForegroundColor Gray
    }
} catch {
    Write-Host "✗ 错误 / Error: $_" -ForegroundColor Red
    Write-Host "  请确保 Simulator 服务已启动 (Port 5001) / Please ensure Simulator service is running (Port 5001)" -ForegroundColor Yellow
}

# 测试 Simulator - 威胁详情（使用第一个主机名）/ Test Simulator - Threat Details (using first hostname)
Write-Host "`n2. 测试 Simulator - 威胁详情 / Testing Simulator - Threat Details..." -ForegroundColor Yellow
try {
    # 先获取主机列表 / First get host list
    $summaryResponse = Invoke-WebRequest -Uri "http://localhost:5001/api/v1/threats/summary" -UseBasicParsing -ErrorAction Stop
    $summaryJson = $summaryResponse.Content | ConvertFrom-Json
    if ($summaryJson.hostThreatCounts -and $summaryJson.hostThreatCounts.Count -gt 0) {
        $testHostname = $summaryJson.hostThreatCounts[0].hostname
        Write-Host "  测试主机名 / Test Hostname: $testHostname" -ForegroundColor Gray
        $detailsResponse = Invoke-WebRequest -Uri "http://localhost:5001/api/v1/threats/details/$testHostname" -UseBasicParsing -ErrorAction Stop
        $detailsJson = $detailsResponse.Content | ConvertFrom-Json
        Write-Host "✓ 成功 / Success!" -ForegroundColor Green
        Write-Host "  主机名 / Hostname: $($detailsJson.hostname)" -ForegroundColor White
        Write-Host "  CVE 总数 / Total CVEs: $($detailsJson.totalCount)" -ForegroundColor White
    } else {
        Write-Host "⚠ 没有可用的主机名进行测试 / No available hostnames for testing" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ 错误 / Error: $_" -ForegroundColor Red
}

# 测试 Engine - 审计摘要 / Test Engine - Audit Summary
Write-Host "`n3. 测试 Engine - 审计摘要 / Testing Engine - Audit Summary..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5002/api/audit/summary" -UseBasicParsing -ErrorAction Stop
    $json = $response.Content | ConvertFrom-Json
    Write-Host "✓ 成功 / Success!" -ForegroundColor Green
    Write-Host "  总资产数 / Total Assets: $($json.totalAssets)" -ForegroundColor White
    Write-Host "  合规设备 / Compliant: $($json.compliantCount)" -ForegroundColor Green
    Write-Host "  不合规设备 / Non-Compliant: $($json.nonCompliantCount)" -ForegroundColor Red
    if ($json.complianceItems -and $json.complianceItems.Count -gt 0) {
        Write-Host "  示例设备 / Example Device: $($json.complianceItems[0].hostname) - $($json.complianceItems[0].status)" -ForegroundColor Gray
    }
} catch {
    Write-Host "✗ 错误 / Error: $_" -ForegroundColor Red
    Write-Host "  请确保 Engine 服务已启动 (Port 5002) / Please ensure Engine service is running (Port 5002)" -ForegroundColor Yellow
}

Write-Host "`n=== 测试完成 ===" -ForegroundColor Cyan
Write-Host "=== Test Complete ===" -ForegroundColor Cyan
Write-Host "`n提示 / Tip: 你也可以在浏览器中直接访问这些 URL 查看 JSON 数据" -ForegroundColor Gray
Write-Host "Tip: You can also directly access these URLs in a browser to view JSON data" -ForegroundColor Gray
