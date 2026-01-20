# AuditFlow 企业级审计系统 - 开发指南

## 📋 目录

1. [系统架构](#系统架构)
2. [技术栈](#技术栈)
3. [项目结构](#项目结构)
4. [数据库配置](#数据库配置)
5. [本地运行](#本地运行)
6. [API 端点](#api-端点)
7. [数据模型](#数据模型)
8. [开发环境要求](#开发环境要求)

---

## 🏗️ 系统架构

### 完全物理隔离 + API 联动设计

```
┌─────────────────────┐         ┌─────────────────────┐
│  CS_Simulator       │         │  AuditFlow_Engine   │
│  (Port 5001)        │◄────────┤  (Port 5002)        │
│                     │  HTTP   │                     │
│  CS_SimulatorDB     │         │  AuditFlowDB        │
│  (独立数据库)        │         │  (独立数据库)        │
└─────────────────────┘         └─────────────────────┘
         ▲                                ▲
         │                                │
         └────────────────┬───────────────┘
                          │
                  ┌───────┴────────┐
                  │  React Frontend │
                  │  (Port 5173)    │
                  └─────────────────┘
```

### 架构特点

- **物理隔离**: 两个后端服务使用完全独立的数据库
- **API 通信**: Engine 通过 HTTP 调用 Simulator 获取威胁数据
- **前端分离**: React 前端独立运行，通过 REST API 与后端通信
- **微服务架构**: 每个服务职责单一，易于扩展和维护

---

## 🛠️ 技术栈

### 后端技术

| 技术 | 版本 | 用途 | 说明 |
|------|------|------|------|
| **.NET** | 9.0 | 后端开发框架 | 需要安装 .NET SDK |
| **ASP.NET Core** | 9.0 | Web API 框架 | 随 .NET SDK 安装 |
| **Kestrel** | 9.0 | Web 服务器 | ASP.NET Core 内置，**不是 Windows 自带** |
| **Minimal APIs** | 9.0 | 轻量级 API 开发（无 Controller） | ASP.NET Core 功能 |
| **Entity Framework Core** | 9.0.0 | ORM 框架 | 通过 NuGet 包安装 |
| **SQL Server LocalDB** | - | 本地数据库 | 需要单独安装 |
| **C#** | 12.0 | 编程语言 | 随 .NET SDK 安装 |

### 前端技术

| 技术 | 版本 | 用途 |
|------|------|------|
| **React** | 19.2.0 | UI 框架 |
| **TypeScript** | 5.9.3 | 类型安全的 JavaScript |
| **Vite** | 7.2.4 | 构建工具和开发服务器 |
| **Tailwind CSS** | 3.4.1 | CSS 框架 |
| **PostCSS** | 8.4.35 | CSS 后处理器 |

### 数据库

| 数据库 | 版本 | 用途 |
|--------|------|------|
| **SQL Server LocalDB** | - | 本地开发数据库 |
| **Entity Framework Core** | 9.0.0 | 数据库访问层 |

---

## 📁 项目结构

```
AuditFlow/
├── AuditFlow.Shared/              # 共享类库
│   ├── Entities/                   # 实体类
│   │   ├── HardwareAsset.cs       # 硬件资产实体
│   │   └── CVEThreat.cs           # CVE威胁实体
│   ├── DTOs/                      # 数据传输对象
│   │   ├── AuditSummaryDto.cs     # 审计摘要DTO
│   │   └── ThreatSummaryDto.cs    # 威胁摘要DTO
│   └── Data/                      # 数据访问层
│       ├── AuditFlowDbContext.cs  # Engine数据库上下文
│       └── CS_SimulatorDbContext.cs # Simulator数据库上下文
│
├── AuditFlow.Simulator/           # 威胁数据提供方 (Port 5001)
│   ├── Program.cs                 # Minimal API 入口
│   ├── appsettings.json          # 配置文件
│   └── CS_SimulatorDB            # 独立数据库
│
├── AuditFlow.Engine/              # 审计引擎 (Port 5002)
│   ├── Program.cs                 # Minimal API 入口
│   ├── appsettings.json          # 配置文件
│   └── AuditFlowDB               # 独立数据库
│
└── AuditFlow.UI/                  # React 前端 (Port 5173)
    ├── src/
    │   ├── App.tsx               # 主组件
    │   ├── types.ts              # TypeScript 类型定义
    │   └── main.tsx              # 入口文件
    ├── package.json              # 依赖配置
    └── vite.config.ts            # Vite 配置
```

---

## 🗄️ 数据库配置

### 数据库类型

**SQL Server LocalDB** - 轻量级本地 SQL Server 实例

### 数据库实例

系统使用两个完全独立的数据库：

1. **CS_SimulatorDB**
   - 服务: `AuditFlow.Simulator`
   - 连接字符串: `Server=(localdb)\mssqllocaldb;Database=CS_SimulatorDB;...`
   - 用途: 存储 CVE 威胁数据

2. **AuditFlowDB**
   - 服务: `AuditFlow.Engine`
   - 连接字符串: `Server=(localdb)\mssqllocaldb;Database=AuditFlowDB;...`
   - 用途: 存储硬件资产数据

### 数据库连接配置

#### CS_Simulator (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CS_SimulatorDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

#### AuditFlow_Engine (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuditFlowDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "SimulatorApiUrl": "http://localhost:5001"
}
```

### 数据库初始化

- **自动创建**: 使用 `db.Database.EnsureCreated()` 自动创建数据库和表
- **自动种子数据**: 数据库为空时自动生成种子数据
- **无需迁移**: 开发环境直接使用 `EnsureCreated()`

---

## 🚀 本地运行

### 运行方式

系统通过以下服务在 localhost 运行：

1. **Kestrel Web Server** (后端服务)
   - CS_Simulator: `http://localhost:5001`
   - AuditFlow_Engine: `http://localhost:5002`
   - **来源**: ASP.NET Core 框架内置（随 .NET SDK 安装）
   - **不需要 Node.js** - 运行在 .NET Runtime 上
   - **不是 Windows 自带** - 需要安装 .NET SDK

2. **Vite Dev Server** (前端开发服务器)
   - Frontend: `http://localhost:5173`
   - **来源**: Vite 工具（通过 npm 安装）
   - **需要 Node.js** - 仅用于开发环境

### Node.js 使用说明

#### 开发环境（需要 Node.js）

- **用途**: 
  - 运行 `npm install` 安装前端依赖
  - 运行 `npm run dev` 启动 Vite 开发服务器
  - 提供热重载（HMR）功能

- **必需**: ✅ 是（开发时必需）

#### 生产环境（不需要 Node.js）

- **构建**: 运行 `npm run build` 生成静态文件到 `dist/` 目录
- **部署**: 将 `dist/` 目录部署到任何 Web 服务器
  - Nginx
  - IIS
  - Apache
  - 或任何静态文件服务器
- **必需**: ❌ 否（生产环境不需要 Node.js）

### 启动步骤

#### 方式一：手动启动（推荐用于开发）

**1. 启动 CS_Simulator (终端 1)**
```powershell
cd AuditFlow.Simulator
dotnet run
```
等待输出: `Now listening on: http://localhost:5001`

**2. 启动 AuditFlow_Engine (终端 2)**
```powershell
cd AuditFlow.Engine
dotnet run
```
等待输出: `Now listening on: http://localhost:5002`

**3. 启动前端 (终端 3)**
```powershell
cd AuditFlow.UI
npm install  # 首次运行需要（需要 Node.js）
npm run dev  # 启动 Vite 开发服务器（需要 Node.js）
```
等待输出: `Local: http://localhost:5173/`

**注意**: 前端开发需要 Node.js，但后端服务（.NET）不需要 Node.js

#### 方式二：使用 PowerShell 脚本

```powershell
.\start-backend.ps1
```

### 访问地址

- **前端界面**: http://localhost:5173
- **Simulator API**: http://localhost:5001/api/v1/threats/summary
- **Engine API**: http://localhost:5002/api/audit/summary

### 验证服务运行

```powershell
# 检查进程
Get-Process | Where-Object { $_.ProcessName -like "*dotnet*" -or $_.ProcessName -like "*node*" }

# 检查端口
netstat -ano | findstr ":5001 :5002 :5173"

# 测试 API
Invoke-WebRequest -Uri "http://localhost:5001/api/v1/threats/summary" -UseBasicParsing
Invoke-WebRequest -Uri "http://localhost:5002/api/audit/summary" -UseBasicParsing
```

---

## 🔌 API 端点

### CS_Simulator (Port 5001)

#### GET /api/v1/threats/summary
返回所有主机及其威胁统计

**响应示例**:
```json
{
  "hostThreatCounts": [
    {
      "hostname": "DX35GB8",
      "threatCount": 5
    }
  ],
  "totalUniqueHostsWithThreats": 12,
  "totalCVEs": 84
}
```

#### GET /api/v1/threats/details/{hostname}
返回指定主机的详细 CVE 列表

**响应示例**:
```json
[
  {
    "id": 1,
    "hostname": "DX35GB8",
    "cve_ID": "CVE-2024-12345",
    "severity": "Critical",
    "remediation": "Apply security patch immediately",
    "detectedDate": "2024-01-15T10:30:00"
  }
]
```

### AuditFlow_Engine (Port 5002)

#### GET /api/audit/summary
返回合规审计摘要

**响应示例**:
```json
{
  "complianceItems": [
    {
      "sn": "X35GB8",
      "hostname": "DX35GB8",
      "purchaseDate": "2018-01-15T00:00:00",
      "ageYears": 6,
      "threatCount": 5,
      "status": "Non-Compliant",
      "action": "Replace",
      "reason": "Device is 6 years old and has 5 threat(s)"
    }
  ],
  "totalAssets": 6,
  "compliantCount": 3,
  "nonCompliantCount": 3
}
```

---

## 📊 数据模型

### HardwareAsset (硬件资产)

存储在 `AuditFlowDB` 数据库中

| 字段 | 类型 | 说明 |
|------|------|------|
| SN | string (PK) | 序列号（例如: "X35GB8"） |
| Hostname | string | 主机名（格式: D + SN，例如: "DX35GB8"） |
| PurchaseDate | DateTime | 购买日期 |
| Status | string | 状态（例如: "Active"） |

### CVEThreat (CVE 威胁)

存储在 `CS_SimulatorDB` 数据库中

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int (PK) | 主键 |
| Hostname | string | 主机名（格式: D + 5-8位字母数字） |
| CVE_ID | string | CVE 编号（格式: CVE-YYYY-XXXXX） |
| Severity | string | 严重程度（Critical/High/Medium/Low） |
| Remediation | string | 修复建议 |
| DetectedDate | DateTime | 检测日期 |

### 数据格式说明

- **主机名格式**: `D` + 序列号（例如: `DX35GB8`）
- **序列号格式**: 5-8 位随机字母数字组合（例如: `X35GB8`）
- **SN 字段**: 存储序列号部分（不含 D 前缀）
- **Hostname 字段**: 存储完整主机名（含 D 前缀）

---

## 💻 开发环境要求

### 必需软件

1. **.NET 9 SDK** ⭐ 必需
   - 下载: https://dotnet.microsoft.com/download/dotnet/9.0
   - 验证: `dotnet --version` (应显示 9.x.x)
   - 用途: 
     - 运行后端服务（Simulator 和 Engine）
     - 包含 ASP.NET Core 和 Kestrel Web Server
   - **注意**: Kestrel 不是 Windows 自带，而是随 .NET SDK 安装的

2. **Node.js 18+** ⚠️ 仅开发环境需要
   - 下载: https://nodejs.org/
   - 验证: `node --version` (应显示 v18+)
   - 用途: 
     - 开发时运行前端（`npm run dev`）
     - 构建前端（`npm run build`）
   - **注意**: 生产环境部署静态文件后不需要 Node.js

3. **SQL Server LocalDB** ⭐ 必需
   - 通常随 Visual Studio 安装
   - 或下载 SQL Server Express with LocalDB
   - 验证: `sqllocaldb info` 或 `sqllocaldb info mssqllocaldb`
   - 用途: 存储数据（两个独立数据库）

4. **PowerShell 7+** (Windows) - 可选
   - 用于运行启动脚本
   - 验证: `pwsh --version`
   - 也可以手动启动服务

### 可选工具

- **Visual Studio 2022** 或 **Visual Studio Code**
- **SQL Server Management Studio (SSMS)** - 用于查看数据库
- **Postman** 或 **curl** - 用于测试 API

### 环境变量

无需特殊环境变量，所有配置都在 `appsettings.json` 中。

---

## 🔧 配置说明

### 日志级别

默认设置为 `Warning`，减少控制台输出：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "System.Net.Http": "Warning"
    }
  }
}
```

### CORS 配置

Engine 和 Simulator 都配置了 CORS，允许前端访问：

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

### API 缓存

Engine 使用 5 秒内存缓存减少对 Simulator 的调用：

```csharp
var threatSummaryCache = new ConcurrentDictionary<string, (ThreatSummaryDto data, DateTime expires)>();
```

---

## 📝 审计逻辑

### 合规判断规则

设备被标记为 **Non-Compliant**（需要更换）的条件：

1. **设备年龄** >= 5 年（当前年份 - 购买年份 >= 5）
2. **并且** 设备在威胁列表中（有活跃的 CVE 威胁）

### 状态说明

- **Non-Compliant**: 需要更换（年龄 >= 5 年 且 有威胁）
- **Compliant**: 合规（其他情况）
  - 有威胁但年龄 < 5 年 → Monitor
  - 年龄 >= 5 年但无威胁 → Monitor
  - 新设备且无威胁 → None

---

## 🐛 故障排除

### 常见问题

1. **端口被占用**
   - 检查: `netstat -ano | findstr ":5001 :5002 :5173"`
   - 解决: 修改 `Program.cs` 中的端口或关闭占用端口的进程

2. **数据库连接错误**
   - 检查: SQL Server LocalDB 是否运行
   - 解决: `sqllocaldb start mssqllocaldb`

3. **CORS 错误**
   - 检查: 所有服务是否都已启动
   - 解决: 确保前端访问 `http://localhost:5173`

4. **前端空白页面**
   - 检查: 浏览器控制台错误
   - 解决: 等待服务完全启动（可能需要 30-60 秒）

5. **API 超时**
   - 检查: Engine 服务是否正常启动
   - 解决: 查看 Engine 服务窗口的错误信息

---

## 📚 相关文档

- `企业级审计系统说明.md` - 详细的系统说明
- `ISSUE_LOGS.md` - 问题日志和解决方案
- `性能优化说明.md` - 性能优化记录
- `重置数据库.ps1` - 数据库重置脚本

---

## 📦 部署说明

### 开发环境 vs 生产环境

#### 开发环境
- **后端**: .NET Kestrel Web Server（不需要 Node.js）
- **前端**: Vite Dev Server（需要 Node.js）
- **数据库**: SQL Server LocalDB

#### 生产环境

**后端服务器需要安装：**
- ✅ **.NET 9 Runtime** 或 **.NET 9 SDK**（运行后端服务）
- ✅ **SQL Server**（生产数据库）

**前端服务器需要安装：**
- ❌ **不需要 Node.js**（构建后是静态文件）
- ✅ **Web 服务器**（任选其一）：
  - **Nginx**（Linux/Windows，推荐）
  - **IIS**（Windows Server）
  - **Apache**（Linux/Windows）
  - 或任何静态文件服务器

### 前端部署步骤

#### 1. 构建前端（在开发机器上）

```powershell
cd AuditFlow.UI
npm install        # 如果还没安装依赖
npm run build      # 构建生产版本
```

构建完成后，会在 `dist/` 目录生成静态文件：
```
dist/
├── index.html
├── assets/
│   ├── index-[hash].js
│   └── index-[hash].css
└── ...
```

#### 2. 部署到服务器

**选项 A: Nginx（推荐，跨平台）**

1. **安装 Nginx**
   - Windows: 下载 https://nginx.org/en/download.html
   - Linux: `sudo apt install nginx` (Ubuntu) 或 `sudo yum install nginx` (CentOS)

2. **配置 Nginx**
   ```nginx
   server {
       listen 80;
       server_name your-domain.com;
       
       # 前端静态文件
       root /path/to/dist;
       index index.html;
       
       # 前端路由（React Router）
       location / {
           try_files $uri $uri/ /index.html;
       }
       
       # 后端 API 代理
       location /api/ {
           proxy_pass http://localhost:5002;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
       }
   }
   ```

3. **复制文件**
   ```bash
   # 将 dist/ 目录内容复制到服务器
   scp -r dist/* user@server:/path/to/nginx/html/
   ```

**选项 B: IIS（Windows Server）**

1. **安装 IIS**
   - Windows Server: 通过"服务器管理器"添加"Web 服务器(IIS)"角色

2. **安装 URL Rewrite 模块**
   - 下载: https://www.iis.net/downloads/microsoft/url-rewrite

3. **创建网站**
   - 在 IIS 中创建新网站
   - 物理路径指向 `dist/` 目录
   - 绑定端口（如 80 或 443）

4. **配置 web.config**（放在 dist/ 目录）
   ```xml
   <?xml version="1.0" encoding="UTF-8"?>
   <configuration>
     <system.webServer>
       <rewrite>
         <rules>
           <rule name="React Routes" stopProcessing="true">
             <match url=".*" />
             <conditions logicalGrouping="MatchAll">
               <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
               <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
             </conditions>
             <action type="Rewrite" url="/index.html" />
           </rule>
         </rules>
       </rewrite>
     </system.webServer>
   </configuration>
   ```

**选项 C: Apache（Linux/Windows）**

1. **安装 Apache**
   - Linux: `sudo apt install apache2` (Ubuntu)
   - Windows: 下载 https://httpd.apache.org/download.cgi

2. **配置 Apache**
   ```apache
   <VirtualHost *:80>
       ServerName your-domain.com
       DocumentRoot /path/to/dist
       
       <Directory /path/to/dist>
           Options Indexes FollowSymLinks
           AllowOverride All
           Require all granted
       </Directory>
       
       # 前端路由
       RewriteEngine On
       RewriteBase /
       RewriteRule ^index\.html$ - [L]
       RewriteCond %{REQUEST_FILENAME} !-f
       RewriteCond %{REQUEST_FILENAME} !-d
       RewriteRule . /index.html [L]
   </VirtualHost>
   ```

### 后端部署步骤

#### Windows Server

1. **安装 .NET 9 Runtime**
   - 下载: https://dotnet.microsoft.com/download/dotnet/9.0
   - 选择 "ASP.NET Core Runtime" 或 "Runtime"

2. **部署应用**
   ```powershell
   # 发布应用
   dotnet publish -c Release -o ./publish
   
   # 复制到服务器
   # 将 publish/ 目录复制到服务器
   ```

3. **运行服务**
   ```powershell
   cd publish
   dotnet AuditFlow.Engine.dll
   # 或配置为 Windows Service
   ```

#### Linux Server

1. **安装 .NET 9 Runtime**
   ```bash
   # Ubuntu/Debian
   wget https://dot.net/v1/dotnet-install.sh
   bash dotnet-install.sh --channel 9.0
   
   # 或使用包管理器
   sudo apt-get update
   sudo apt-get install -y dotnet-runtime-9.0
   ```

2. **部署应用**
   ```bash
   # 发布应用
   dotnet publish -c Release -o ./publish
   
   # 复制到服务器
   scp -r publish/* user@server:/opt/auditflow/
   ```

3. **配置 Systemd 服务**（可选）
   ```ini
   # /etc/systemd/system/auditflow-engine.service
   [Unit]
   Description=AuditFlow Engine Service
   
   [Service]
   WorkingDirectory=/opt/auditflow
   ExecStart=/usr/bin/dotnet /opt/auditflow/AuditFlow.Engine.dll
   Restart=always
   
   [Install]
   WantedBy=multi-user.target
   ```

### 服务器需求总结

| 服务器类型 | 需要安装的软件 |
|-----------|---------------|
| **前端服务器** | Web 服务器（Nginx/IIS/Apache）<br>❌ 不需要 Node.js<br>❌ 不需要 .NET |
| **后端服务器** | .NET 9 Runtime<br>SQL Server<br>❌ 不需要 Node.js<br>❌ 不需要 Web 服务器（Kestrel 内置） |

### 完整部署架构示例

```
┌─────────────────────┐
│   前端服务器         │
│   (Nginx/IIS)       │
│   - 静态文件         │
│   - 反向代理         │
└──────────┬──────────┘
           │
           │ HTTP
           ↓
┌─────────────────────┐
│   后端服务器         │
│   (.NET Runtime)    │
│   - Kestrel         │
│   - API 服务        │
└──────────┬──────────┘
           │
           ↓
┌─────────────────────┐
│   数据库服务器       │
│   (SQL Server)      │
└─────────────────────┘
```

### 总结

| 组件 | 开发环境 | 生产环境 |
|------|---------|---------|
| 后端 (.NET) | ✅ 需要 .NET SDK | ✅ 需要 .NET Runtime |
| 前端 (React) | ✅ 需要 Node.js | ❌ 不需要 Node.js<br>✅ 需要 Web 服务器 |
| 数据库 | ✅ SQL Server LocalDB | ✅ SQL Server |

---

## 🎯 未来集成

当前 Simulator 是模拟数据。生产环境将：
- 直接调用 CrowdStrike API 获取真实威胁数据
- 替换 Simulator 服务为 CrowdStrike API 客户端
- 保持 Engine 和前端代码不变

---

**最后更新**: 2026-01-20  
**版本**: 1.0.0 (Final)
