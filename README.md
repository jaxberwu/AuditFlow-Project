# AuditFlow System (Lightweight Edition)

A streamlined data-flow prototype for asset auditing using Minimal APIs in .NET 9 and React 19.

## Architecture

- **AuditFlow.Shared**: Shared class library for entities and DTOs
- **AuditFlow.Simulator** (Port 5001): Minimal API acting as the "Threat Provider"
- **AuditFlow.Engine** (Port 5002): Minimal API acting as the "Audit Brain"
- **AuditFlow.UI** (Port 5173): React 19 frontend with Vite, TypeScript, and Tailwind CSS

## Database

Uses SQL Server LocalDB with shared database `AuditFlowDB`. Both services share the same database.

## 如何运行 (How to Run)

### 前置要求 (Prerequisites)
- .NET 9 SDK
- Node.js 18+
- SQL Server LocalDB (通常随 Visual Studio 安装) 或 SQL Server Express

### 步骤 1: 启动后端服务

需要打开 **3个终端窗口** 分别运行：

#### 终端 1: 启动 Simulator (Port 5001)
```bash
cd AuditFlow.Simulator
dotnet run
```

等待看到类似输出：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
```

#### 终端 2: 启动 Engine (Port 5002)
```bash
cd AuditFlow.Engine
dotnet run
```

等待看到类似输出：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5002
```

#### 终端 3: 启动前端 (Port 5173)
```bash
cd AuditFlow.UI
npm install  # 如果还没安装依赖
npm run dev
```

等待看到类似输出：
```
  VITE v7.2.4  ready in 500 ms

  ➜  Local:   http://localhost:5173/
```

### 步骤 2: 访问应用

打开浏览器访问: **http://localhost:5173**

你应该看到 AuditFlow Dashboard，显示：
- 总资产数：5
- 不合规设备：1 (SN001 - 因为它是2018年购买且有关键威胁)
- 合规设备：4

### 验证 API 端点

你也可以直接测试 API：

1. **测试 Simulator**:
   ```bash
   curl http://localhost:5001/threats
   ```

2. **测试 Engine**:
   ```bash
   curl http://localhost:5002/audit/summary
   ```

## API Endpoints

### Simulator (Port 5001)
- `GET /threats` - Returns list of threat alerts

### Engine (Port 5002)
- `GET /audit/summary` - Returns audit summary with replacement required items

## 审计逻辑 (Audit Logic)

设备被标记为"不合规"（需要更换）的条件：
- 设备年龄 >= 5 年 (当前年份 - 购买年份 >= 5)
- **并且** 设备有活跃的威胁警报

## 种子数据 (Seed Data)

### Hardware Assets (硬件资产)
- SN001: DEVICE-OLD-01 (2018-01-15) - 有 Critical 威胁
- SN002: DEVICE-OLD-02 (2018-06-20) - 有 High 威胁
- SN003: DEVICE-OLD-03 (2018-11-10) - 有 Critical 威胁
- SN004: DEVICE-NEW-01 (2024-03-05) - 无威胁
- SN005: DEVICE-NEW-02 (2024-08-12) - 无威胁

### Threat Alerts (威胁警报)
- SN001: Critical
- SN002: High
- SN003: Critical

**预期结果**: 只有 SN001 会被标记为需要更换（因为它是2018年购买且有关键威胁）

## 故障排除 (Troubleshooting)

### 数据库连接错误
如果遇到 SQL Server 连接错误，确保：
1. SQL Server LocalDB 已安装
2. 或者修改 `appsettings.json` 中的连接字符串指向你的 SQL Server 实例

### 端口被占用
如果端口被占用，可以：
1. 修改 `Program.cs` 中的 `UseUrls()` 设置
2. 或修改 `vite.config.ts` 中的端口设置

### CORS 错误
确保：
1. Simulator 和 Engine 都已启动
2. 前端访问的端口是 5173
3. CORS 配置在 `Program.cs` 中已正确设置

## Future Integration

This is a simulation project. In production, the Simulator will be replaced with direct API calls to CrowdStrike to fetch real threat data.
