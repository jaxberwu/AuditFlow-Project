# IIS 生产环境部署指南

## 🎯 部署流程总览

### 两个阶段

1. **本地构建**（需要 Node.js）✅
   - 运行 `npm run build` 生成静态文件
   
2. **IIS 部署**（不需要 Node.js）❌
   - 将静态文件复制到 IIS
   - IIS 直接提供静态文件

---

## 📋 部署步骤

### 阶段 1: 本地构建（需要 Node.js）

**在你的开发机器上操作**（需要有 Node.js 的电脑）:

```powershell
# 1. 进入前端项目目录
cd AuditFlow.UI

# 2. 安装依赖（如果需要）
npm install

# 3. 构建生产版本
npm run build
```

**构建结果**:
```
AuditFlow.UI/
└── dist/              # 构建后的静态文件
    ├── index.html
    ├── assets/
    │   ├── index-[hash].js
    │   └── index-[hash].css
    └── ...
```

**关键点**: 
- ✅ 这一步**需要 Node.js**
- ✅ 只在**本地开发机器**上执行
- ✅ **IIS 服务器不需要执行这一步**

---

### 阶段 2: IIS 服务器部署（不需要 Node.js）

**在 IIS 服务器上操作**（不需要 Node.js）:

#### 步骤 1: 复制文件到 IIS 服务器

```powershell
# 将 dist/ 目录的内容复制到 IIS 服务器
# 例如：C:\inetpub\wwwroot\AuditFlow\
```

**目录结构**:
```
C:\inetpub\wwwroot\AuditFlow\
├── index.html
├── assets/
│   ├── index-[hash].js
│   └── index-[hash].css
└── ...
```

#### 步骤 2: 在 IIS 中创建网站

1. **打开 IIS 管理器**
   - Windows Server: 服务器管理器 → 角色 → Web 服务器 (IIS)
   - 或运行 `inetmgr`

2. **创建新网站**
   - 右键"网站" → "添加网站"
   - 网站名称: `AuditFlow`
   - 物理路径: `C:\inetpub\wwwroot\AuditFlow`
   - 绑定端口: `80` 或 `443` (HTTPS)
   - 主机名: (可选) `auditflow.yourcompany.com`

#### 步骤 3: 配置 URL Rewrite（支持 React Router）

1. **安装 URL Rewrite 模块**
   - 下载: https://www.iis.net/downloads/microsoft/url-rewrite
   - 安装到 IIS 服务器

2. **创建 web.config 文件**

在 `C:\inetpub\wwwroot\AuditFlow\` 目录创建 `web.config`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <!-- React Router 支持 -->
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
    
    <!-- 静态文件缓存 -->
    <staticContent>
      <clientCache cacheControlMode="UseMaxAge" cacheControlMaxAge="365.00:00:00" />
    </staticContent>
  </system.webServer>
</configuration>
```

#### 步骤 4: 配置 API 代理（可选）

如果需要 IIS 代理后端 API，可以使用 **Application Request Routing (ARR)** 或 **反向代理**。

**配置示例**（在 web.config 中添加）:
```xml
<system.webServer>
  <rewrite>
    <rules>
      <!-- API 代理 -->
      <rule name="API Proxy" stopProcessing="true">
        <match url="^api/(.*)" />
        <action type="Rewrite" url="http://your-backend-server:5002/api/{R:1}" />
        <serverVariables>
          <set name="HTTP_X_FORWARDED_HOST" value="{HTTP_HOST}" />
          <set name="HTTP_X_FORWARDED_PROTO" value="https" />
        </serverVariables>
      </rule>
      
      <!-- React Router 支持 -->
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
```

#### 步骤 5: 验证部署

1. **访问网站**
   - 浏览器打开: `http://your-server-ip` 或 `http://auditflow.yourcompany.com`

2. **检查功能**
   - 页面正常加载
   - API 请求正常（检查 Network 标签）
   - 路由跳转正常

---

## 📊 部署架构

### 完整生产环境架构

```
┌─────────────────────┐
│   IIS 服务器         │
│   (不需要 Node.js)   │
│                     │
│   ┌───────────────┐ │
│   │  静态文件      │ │
│   │  (dist/)      │ │
│   └───────┬───────┘ │
└───────────┼─────────┘
            │ HTTP
            ↓
    ┌───────┴────────┐
    │   用户浏览器    │
    │  (访问前端)     │
    └────────────────┘
            │
            │ API 请求
            ↓
┌─────────────────────┐
│   后端服务器         │
│   (.NET + Kestrel)  │
│   Port 5001, 5002   │
└─────────────────────┘
            │
            ↓
┌─────────────────────┐
│   SQL Server        │
│   (数据库)          │
└─────────────────────┘
```

---

## ✅ Node.js 使用总结

### 需要 Node.js 的场景

✅ **本地开发/测试环境**
- 运行 `npm install` 安装依赖
- 运行 `npm run dev` 启动开发服务器
- 运行 `npm run build` 构建生产版本

### 不需要 Node.js 的场景

❌ **IIS 生产服务器**
- 只提供静态文件（HTML/CSS/JS）
- IIS 内置的静态文件处理
- 不需要任何 JavaScript 运行时

---

## 🔑 关键要点

### 你的理解完全正确！

1. ✅ **Node.js 只是用于本地临时搭建的开发服务器**
2. ✅ **IIS 服务器部署不需要 Node.js**
3. ✅ **生产环境使用 IIS，直接提供静态文件**

### 工作流程

```
本地开发机器（有 Node.js）
  ↓
npm run build  (构建静态文件)
  ↓
复制 dist/ 到 IIS 服务器
  ↓
IIS 服务器（不需要 Node.js）
  ↓
用户访问网站
```

---

## 📝 对比表

| 环境 | 需要 Node.js | 用途 |
|------|-------------|------|
| **本地开发** | ✅ 是 | 临时开发服务器 (`npm run dev`) |
| **本地构建** | ✅ 是 | 生成静态文件 (`npm run build`) |
| **IIS 服务器** | ❌ **否** | 提供静态文件（不需要 Node.js） |

---

## 💡 总结

**你的理解完全正确** ✅:

- Node.js = 用于**本地临时搭建的服务环境**（开发服务器）
- IIS 服务器 = **正式生产环境**，不需要 Node.js

**流程**:
1. 在本地（有 Node.js）构建前端
2. 将构建结果部署到 IIS
3. IIS 直接提供静态文件

---

**最后更新**: 2026-01-20
