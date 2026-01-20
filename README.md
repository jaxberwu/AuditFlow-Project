# AuditFlow System (Lightweight Edition) / AuditFlow 系统（轻量版）

A streamlined data-flow prototype for asset auditing using Minimal APIs in .NET 9 and React 19.  
使用 .NET 9 和 React 19 的 Minimal API 构建的资产审计数据流原型。

## Architecture / 架构

- **AuditFlow.Shared**: Shared class library for entities and DTOs / 共享类库，包含实体和 DTO
- **AuditFlow.Simulator** (Port 5001): Minimal API acting as the "Threat Provider" / Minimal API，作为"威胁数据提供者"
- **AuditFlow.Engine** (Port 5002): Minimal API acting as the "Audit Brain" / Minimal API，作为"审计引擎"
- **AuditFlow.UI** (Port 5173): React 19 frontend with Vite, TypeScript, and Tailwind CSS / React 19 前端，使用 Vite、TypeScript 和 Tailwind CSS

## Database / 数据库

Uses SQL Server LocalDB with shared database `AuditFlowDB`. Both services share the same database.  
使用 SQL Server LocalDB，共享数据库 `AuditFlowDB`。两个服务共享同一个数据库。

## How to Run / 如何运行

### Prerequisites / 前置要求
- .NET 9 SDK
- Node.js 18+
- SQL Server LocalDB (usually installed with Visual Studio) or SQL Server Express / SQL Server LocalDB（通常随 Visual Studio 安装）或 SQL Server Express

### Step 1: Start Backend Services / 步骤 1：启动后端服务

You need to open **3 terminal windows** and run each service separately.  
需要打开 **3个终端窗口** 分别运行：

#### Terminal 1: Start Simulator (Port 5001) / 终端 1：启动 Simulator（端口 5001）
```bash
cd AuditFlow.Simulator
dotnet run
```

Wait for output similar to: / 等待看到类似输出：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
```

#### Terminal 2: Start Engine (Port 5002) / 终端 2：启动 Engine（端口 5002）
```bash
cd AuditFlow.Engine
dotnet run
```

Wait for output similar to: / 等待看到类似输出：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5002
```

#### Terminal 3: Start Frontend (Port 5173) / 终端 3：启动前端（端口 5173）
```bash
cd AuditFlow.UI
npm install  # Install dependencies if not already installed / 如果还没安装依赖
npm run dev
```

Wait for output similar to: / 等待看到类似输出：
```
  VITE v7.2.4  ready in 500 ms

  ➜  Local:   http://localhost:5173/
```

### Step 2: Access Application / 步骤 2：访问应用

Open browser and visit: **http://localhost:5173**  
打开浏览器访问: **http://localhost:5173**

You should see the AuditFlow Dashboard showing: / 你应该看到 AuditFlow Dashboard，显示：
- Total Assets: 5 / 总资产数：5
- Non-Compliant Devices: 1 (SN001 - purchased in 2018 with critical threat) / 不合规设备：1 (SN001 - 因为它是2018年购买且有关键威胁)
- Compliant Devices: 4 / 合规设备：4

### Verify API Endpoints / 验证 API 端点

You can also test the APIs directly: / 你也可以直接测试 API：

1. **Test Simulator / 测试 Simulator**:
   ```bash
   curl http://localhost:5001/threats
   ```

2. **Test Engine / 测试 Engine**:
   ```bash
   curl http://localhost:5002/audit/summary
   ```

## API Endpoints / API 端点

### Simulator (Port 5001)
- `GET /threats` - Returns list of threat alerts / 返回威胁警报列表

### Engine (Port 5002)
- `GET /audit/summary` - Returns audit summary with replacement required items / 返回审计摘要，包含需要更换的项目

## Audit Logic / 审计逻辑

A device is marked as "Non-Compliant" (requires replacement) when: / 设备被标记为"不合规"（需要更换）的条件：
- Device age >= 5 years (Current Year - Purchase Year >= 5) / 设备年龄 >= 5 年 (当前年份 - 购买年份 >= 5)
- **AND** device has active threat alerts / **并且** 设备有活跃的威胁警报

## Seed Data / 种子数据

### Hardware Assets / 硬件资产
- SN001: DEVICE-OLD-01 (2018-01-15) - Has Critical threat / 有 Critical 威胁
- SN002: DEVICE-OLD-02 (2018-06-20) - Has High threat / 有 High 威胁
- SN003: DEVICE-OLD-03 (2018-11-10) - Has Critical threat / 有 Critical 威胁
- SN004: DEVICE-NEW-01 (2024-03-05) - No threats / 无威胁
- SN005: DEVICE-NEW-02 (2024-08-12) - No threats / 无威胁

### Threat Alerts / 威胁警报
- SN001: Critical
- SN002: High
- SN003: Critical

**Expected Result / 预期结果**: Only SN001 will be marked for replacement (purchased in 2018 with critical threat) / 只有 SN001 会被标记为需要更换（因为它是2018年购买且有关键威胁）

## Troubleshooting / 故障排除

### Database Connection Error / 数据库连接错误
If you encounter SQL Server connection errors, ensure: / 如果遇到 SQL Server 连接错误，确保：
1. SQL Server LocalDB is installed / SQL Server LocalDB 已安装
2. Or modify the connection string in `appsettings.json` to point to your SQL Server instance / 或者修改 `appsettings.json` 中的连接字符串指向你的 SQL Server 实例

### Port Already in Use / 端口被占用
If the port is already in use, you can: / 如果端口被占用，可以：
1. Modify the `UseUrls()` setting in `Program.cs` / 修改 `Program.cs` 中的 `UseUrls()` 设置
2. Or modify the port setting in `vite.config.ts` / 或修改 `vite.config.ts` 中的端口设置

### CORS Error / CORS 错误
Ensure: / 确保：
1. Both Simulator and Engine are started / Simulator 和 Engine 都已启动
2. Frontend is accessing port 5173 / 前端访问的端口是 5173
3. CORS configuration in `Program.cs` is correct / CORS 配置在 `Program.cs` 中已正确设置

## Future Integration / 未来集成

This is a simulation project. In production, the Simulator will be replaced with direct API calls to CrowdStrike to fetch real threat data.  
这是一个模拟项目。在生产环境中，Simulator 将被替换为直接调用 CrowdStrike API 以获取真实的威胁数据。
