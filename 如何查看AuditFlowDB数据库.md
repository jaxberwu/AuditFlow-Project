# 如何查看 AuditFlowDB 数据库

## ⚠️ 重要提示

**AuditFlowDB 数据库存储在 LocalDB 实例中，不是在你当前连接的 SQL Server 实例中！**

## 📍 数据库位置

- **SQL Server 实例**: `(localdb)\mssqllocaldb` (LocalDB)
- **数据库名称**: `AuditFlowDB`
- **NOT**: `JAXBER\SQL` (你当前连接的实例)

## 🔧 在 SSMS 中连接到 LocalDB

### 步骤 1: 连接到 LocalDB 实例

1. 在 SSMS 中，点击 **"Connect"** (连接) 按钮
2. 在 **"Server name"** (服务器名称) 输入框中输入：
   ```
   (localdb)\mssqllocaldb
   ```
3. 点击 **"Connect"** (连接)

### 步骤 2: 查看数据库

连接成功后，你应该能看到：
- 展开 **"Databases"** 文件夹
- 找到 **"AuditFlowDB"** 数据库
- 展开 **"Tables"** 文件夹
- 查看表：
  - `HardwareAssets` (硬件资产表)
  - `ThreatAlerts` (威胁警报表)

## 🔍 如果看不到数据库

### 可能的原因：

1. **服务未启动**
   - 确保 `AuditFlow.Simulator` 和 `AuditFlow.Engine` 服务正在运行
   - 数据库在服务首次启动时自动创建

2. **连接到了错误的实例**
   - 确保连接到 `(localdb)\mssqllocaldb`，而不是其他 SQL Server 实例

3. **数据库尚未创建**
   - 启动服务后，数据库会自动创建
   - 检查服务启动日志，确认数据库创建成功

## 📝 验证数据库是否存在

运行以下 PowerShell 命令检查：

```powershell
# 检查 LocalDB 实例中的数据库
$connection = New-Object System.Data.SqlClient.SqlConnection("Server=(localdb)\mssqllocaldb;Trusted_Connection=True;")
$connection.Open()
$command = $connection.CreateCommand()
$command.CommandText = "SELECT name FROM sys.databases WHERE name = 'AuditFlowDB'"
$result = $command.ExecuteScalar()
if ($result) {
    Write-Host "Database found: $result" -ForegroundColor Green
} else {
    Write-Host "Database not found. Start the services first." -ForegroundColor Red
}
$connection.Close()
```

## 🎯 快速连接步骤总结

1. SSMS → Connect (连接)
2. Server name: `(localdb)\mssqllocaldb`
3. Connect (连接)
4. 展开 Databases → AuditFlowDB → Tables

---

**注意**: LocalDB 是 SQL Server 的轻量级版本，通常随 Visual Studio 安装。如果你没有安装 Visual Studio，可能需要安装 SQL Server Express LocalDB。
