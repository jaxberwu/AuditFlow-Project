# API 测试指南 / API Testing Guide

## 如何测试 API / How to Test APIs

### 前提条件 / Prerequisites

确保服务已启动：/ Ensure services are running:
1. **Simulator** (Port 5001) - 威胁数据服务 / Threat Data Service
2. **Engine** (Port 5002) - 审计引擎服务 / Audit Engine Service

---

## 方法 1: 使用 PowerShell (Windows) / Using PowerShell (Windows)

### 测试 Simulator API / Test Simulator API

#### 1. 获取威胁摘要 / Get Threat Summary

```powershell
# 方法 1: 使用 Invoke-WebRequest (PowerShell 原生)
Invoke-WebRequest -Uri "http://localhost:5001/api/v1/threats/summary" -UseBasicParsing | Select-Object -ExpandProperty Content

# 方法 2: 使用 curl (如果已安装)
curl http://localhost:5001/api/v1/threats/summary

# 方法 3: 格式化 JSON 输出
Invoke-WebRequest -Uri "http://localhost:5001/api/v1/threats/summary" -UseBasicParsing | ConvertFrom-Json | ConvertTo-Json -Depth 10
```

#### 2. 获取指定主机的威胁详情 / Get Threat Details for Specific Hostname

```powershell
# 替换 {hostname} 为实际主机名，例如 DX35GB8
$hostname = "DX35GB8"
Invoke-WebRequest -Uri "http://localhost:5001/api/v1/threats/details/$hostname" -UseBasicParsing | Select-Object -ExpandProperty Content

# 格式化输出
Invoke-WebRequest -Uri "http://localhost:5001/api/v1/threats/details/$hostname" -UseBasicParsing | ConvertFrom-Json | ConvertTo-Json -Depth 10
```

### 测试 Engine API / Test Engine API

#### 获取审计摘要 / Get Audit Summary

```powershell
# 方法 1: 使用 Invoke-WebRequest
Invoke-WebRequest -Uri "http://localhost:5002/api/audit/summary" -UseBasicParsing | Select-Object -ExpandProperty Content

# 方法 2: 格式化 JSON 输出
Invoke-WebRequest -Uri "http://localhost:5002/api/audit/summary" -UseBasicParsing | ConvertFrom-Json | ConvertTo-Json -Depth 10

# 方法 3: 使用 curl
curl http://localhost:5002/api/audit/summary
```

---

## 方法 2: 使用浏览器 / Using Browser

直接在浏览器地址栏输入以下 URL：

### Simulator API

1. **威胁摘要 / Threat Summary**:
   ```
   http://localhost:5001/api/v1/threats/summary
   ```

2. **威胁详情 / Threat Details** (替换 `DX35GB8` 为实际主机名):
   ```
   http://localhost:5001/api/v1/threats/details/DX35GB8
   ```

### Engine API

1. **审计摘要 / Audit Summary**:
   ```
   http://localhost:5002/api/audit/summary
   ```

**注意 / Note**: 浏览器会直接显示 JSON 数据，但格式可能不够美观。  
The browser will display JSON data directly, but the format may not be very readable.

---

## 方法 3: 使用 Postman / Using Postman

### 安装 Postman

1. 下载: https://www.postman.com/downloads/
2. 安装并打开 Postman

### 测试步骤 / Test Steps

#### 测试 Simulator - 威胁摘要 / Test Simulator - Threat Summary

1. 打开 Postman
2. 选择 **GET** 方法
3. 输入 URL: `http://localhost:5001/api/v1/threats/summary`
4. 点击 **Send**
5. 查看响应结果

#### 测试 Simulator - 威胁详情 / Test Simulator - Threat Details

1. 选择 **GET** 方法
2. 输入 URL: `http://localhost:5001/api/v1/threats/details/DX35GB8`
   - 替换 `DX35GB8` 为实际主机名
3. 点击 **Send**

#### 测试 Engine - 审计摘要 / Test Engine - Audit Summary

1. 选择 **GET** 方法
2. 输入 URL: `http://localhost:5002/api/audit/summary`
3. 点击 **Send**

---

## 方法 4: 使用 Visual Studio Code REST Client / Using VS Code REST Client

### 安装扩展

1. 打开 VS Code
2. 安装扩展: **REST Client** (by Huachao Mao)

### 创建测试文件

创建文件 `api-test.http`:

```http
### 测试 Simulator - 威胁摘要
GET http://localhost:5001/api/v1/threats/summary

### 测试 Simulator - 威胁详情
GET http://localhost:5001/api/v1/threats/details/DX35GB8

### 测试 Engine - 审计摘要
GET http://localhost:5002/api/audit/summary
```

点击每个请求上方的 **Send Request** 按钮即可测试。

---

## 快速测试脚本 / Quick Test Script

### PowerShell 测试脚本

创建文件 `test-apis.ps1`:

```powershell
Write-Host "=== Testing AuditFlow APIs ===" -ForegroundColor Cyan

Write-Host "`n1. Testing Simulator - Threat Summary..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5001/api/v1/threats/summary" -UseBasicParsing
    $json = $response.Content | ConvertFrom-Json
    Write-Host "✓ Success!" -ForegroundColor Green
    Write-Host "Total Threats: $($json.totalThreats)" -ForegroundColor White
    Write-Host "Total Hosts: $($json.totalHosts)" -ForegroundColor White
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
}

Write-Host "`n2. Testing Engine - Audit Summary..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5002/api/audit/summary" -UseBasicParsing
    $json = $response.Content | ConvertFrom-Json
    Write-Host "✓ Success!" -ForegroundColor Green
    Write-Host "Total Assets: $($json.totalAssets)" -ForegroundColor White
    Write-Host "Compliant: $($json.compliantCount)" -ForegroundColor White
    Write-Host "Non-Compliant: $($json.nonCompliantCount)" -ForegroundColor White
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
}

Write-Host "`n=== Test Complete ===" -ForegroundColor Cyan
```

运行脚本：
```powershell
.\test-apis.ps1
```

---

## 常见问题 / Common Issues

### 问题 1: 连接被拒绝 / Connection Refused

**错误信息 / Error**: `无法连接到远程服务器` 或 `Connection refused`

**解决方法 / Solution**:
1. 确保 Simulator 和 Engine 服务已启动
2. 检查端口是否正确（5001 和 5002）
3. 检查防火墙设置

### 问题 2: 404 Not Found

**错误信息 / Error**: `404 Not Found`

**解决方法 / Solution**:
1. 检查 URL 是否正确
2. 确保 API 端点路径正确：
   - Simulator: `/api/v1/threats/summary` 或 `/api/v1/threats/details/{hostname}`
   - Engine: `/api/audit/summary`

### 问题 3: PowerShell 中 curl 不可用

**解决方法 / Solution**:
- 使用 `Invoke-WebRequest` 代替 `curl`
- 或安装 Git for Windows（包含 curl）
- 或使用 Windows 10/11 自带的 curl（在 PowerShell 中可能需要使用 `curl.exe`）

---

## API 端点总结 / API Endpoints Summary

### Simulator (Port 5001)

| 端点 / Endpoint | 方法 / Method | 说明 / Description |
|----------------|--------------|-------------------|
| `/api/v1/threats/summary` | GET | 获取所有主机的威胁摘要 / Get threat summary for all hosts |
| `/api/v1/threats/details/{hostname}` | GET | 获取指定主机的详细威胁信息 / Get detailed threat info for specific hostname |

### Engine (Port 5002)

| 端点 / Endpoint | 方法 / Method | 说明 / Description |
|----------------|--------------|-------------------|
| `/api/audit/summary` | GET | 获取审计摘要（包含合规性状态） / Get audit summary (including compliance status) |

---

## 测试示例输出 / Example Test Output

### Simulator - Threat Summary 示例 / Example

```json
{
  "hostThreatCounts": [
    {
      "hostname": "DX35GB8",
      "threatCount": 5,
      "criticalCount": 2,
      "highCount": 1,
      "mediumCount": 1,
      "lowCount": 1
    }
  ],
  "totalThreats": 84,
  "totalHosts": 12
}
```

### Engine - Audit Summary 示例 / Example

```json
{
  "complianceItems": [
    {
      "sn": "X35GB8",
      "hostname": "DX35GB8",
      "purchaseDate": "2018-06-15T00:00:00",
      "ageYears": 6,
      "status": "Non-Compliant",
      "action": "Replace",
      "threatCount": 5,
      "reason": "Device is 6 years old and has 5 threat(s)"
    }
  ],
  "totalAssets": 8,
  "compliantCount": 5,
  "nonCompliantCount": 3
}
```

---

## 推荐测试流程 / Recommended Testing Flow

1. **启动服务 / Start Services**
   - 启动 Simulator (Port 5001)
   - 启动 Engine (Port 5002)

2. **测试 Simulator / Test Simulator**
   - 先测试 `/api/v1/threats/summary` 获取所有主机
   - 选择一个主机名，测试 `/api/v1/threats/details/{hostname}`

3. **测试 Engine / Test Engine**
   - 测试 `/api/audit/summary` 获取审计结果

4. **验证数据一致性 / Verify Data Consistency**
   - 检查 Engine 返回的主机名是否与 Simulator 匹配
   - 检查威胁数量是否正确

---

**提示 / Tip**: 最简单的方法是直接在浏览器中打开 URL，快速查看 API 返回的数据！  
The simplest method is to directly open the URL in a browser to quickly view the API response data!
