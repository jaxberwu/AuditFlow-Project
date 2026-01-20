# VS Code + GitHub 开发指南 / VS Code + GitHub Development Guide

## 📋 概述 / Overview

本指南说明如何在新电脑上通过 VS Code 从 GitHub 克隆项目，并使用 GitHub Copilot 继续开发。  
This guide explains how to clone a project from GitHub using VS Code on a new computer and continue development with GitHub Copilot.

---

## 🚀 步骤 1: 在新电脑上克隆项目 / Step 1: Clone Project on New Computer

### 方法 1: 使用 VS Code 直接克隆（推荐） / Method 1: Clone Directly in VS Code (Recommended)

#### 1.1 打开 VS Code / Open VS Code

#### 1.2 克隆仓库 / Clone Repository

1. **按 `Ctrl+Shift+P` 打开命令面板**  
   Press `Ctrl+Shift+P` to open command palette

2. **输入并选择: `Git: Clone`**  
   Type and select: `Git: Clone`

3. **输入 GitHub 仓库 URL**  
   Enter GitHub repository URL:
   ```
   https://github.com/jaxberwu/AuditFlow-Project.git
   ```

4. **选择本地保存位置**  
   Select local folder to save the project

5. **选择是否在新窗口中打开**  
   Choose whether to open in new window

#### 1.3 打开项目文件夹 / Open Project Folder

VS Code 会询问是否打开克隆的项目文件夹  
VS Code will ask if you want to open the cloned project folder

- 点击 **"Open"** / 点击 **"打开"**

---

### 方法 2: 使用命令行克隆 / Method 2: Clone Using Command Line

```bash
# 1. 打开 PowerShell 或 Git Bash
# Open PowerShell or Git Bash

# 2. 导航到你想保存项目的目录
# Navigate to directory where you want to save project
cd D:\Projects

# 3. 克隆仓库
# Clone repository
git clone https://github.com/jaxberwu/AuditFlow-Project.git

# 4. 进入项目目录
# Enter project directory
cd AuditFlow-Project

# 5. 使用 VS Code 打开
# Open with VS Code
code .
```

---

## 🔧 步骤 2: 安装必要的扩展 / Step 2: Install Required Extensions

### 必需扩展 / Required Extensions

1. **C# Dev Kit** 或 **C#** - .NET 开发支持
2. **GitHub Copilot** - AI 编程助手
3. **GitLens** - Git 增强功能（可选但推荐）

### 安装方法 / Installation Method

1. 点击左侧扩展图标（或按 `Ctrl+Shift+X`）
2. 搜索扩展名称
3. 点击 **Install** / **安装**

---

## 🤖 步骤 3: 设置 GitHub Copilot / Step 3: Setup GitHub Copilot

### 3.1 安装 GitHub Copilot 扩展 / Install GitHub Copilot Extension

1. **在 VS Code 扩展市场中搜索: `GitHub Copilot`**
2. **点击 Install** / **点击安装**

### 3.2 登录 GitHub 账户 / Sign In to GitHub Account

1. **安装后会提示登录 GitHub**
2. **点击 "Sign in to GitHub"** / **点击 "登录 GitHub"**
3. **在浏览器中完成授权**
4. **返回 VS Code，授权完成**

### 3.3 验证 Copilot 是否工作 / Verify Copilot is Working

1. 打开任意 `.cs` 或 `.ts` 文件
2. 开始输入代码
3. 如果看到灰色代码建议，说明 Copilot 已激活

### 3.4 Copilot 常用快捷键 / Common Copilot Shortcuts

- `Tab` - 接受建议 / Accept suggestion
- `Alt + ]` - 下一个建议 / Next suggestion
- `Alt + [` - 上一个建议 / Previous suggestion
- `Ctrl + Enter` - 打开 Copilot 面板查看多个建议 / Open Copilot panel to see multiple suggestions

---

## 🔄 步骤 4: 同步项目更改 / Step 4: Sync Project Changes

### 4.1 拉取最新更改 / Pull Latest Changes

#### 方法 1: 使用 VS Code 界面 / Method 1: Using VS Code UI

1. **点击左下角的分支图标**
2. **选择 "Pull, Push" 或 "Sync Changes"**
3. **或按 `Ctrl+Shift+P`，输入 `Git: Pull`**

#### 方法 2: 使用命令行 / Method 2: Using Command Line

```bash
git pull origin main
```

### 4.2 推送本地更改 / Push Local Changes

#### 使用 VS Code / Using VS Code

1. **点击左侧源代码管理图标（或按 `Ctrl+Shift+G`）**
2. **输入提交信息**
3. **点击 ✓ 提交**
4. **点击 "..." 菜单，选择 "Push"** / **点击 "推送"**

#### 使用命令行 / Using Command Line

```bash
# 1. 添加更改
git add .

# 2. 提交
git commit -m "Your commit message"

# 3. 推送
git push origin main
```

---

## 🛠️ 步骤 5: 配置开发环境 / Step 5: Setup Development Environment

### 5.1 安装 .NET SDK

```bash
# 检查是否已安装
dotnet --version

# 如果未安装，下载: https://dotnet.microsoft.com/download
```

### 5.2 安装 Node.js

```bash
# 检查是否已安装
node --version

# 如果未安装，下载: https://nodejs.org/
```

### 5.3 恢复项目依赖 / Restore Project Dependencies

#### .NET 依赖 / .NET Dependencies

```bash
# 在项目根目录运行
dotnet restore
```

#### Node.js 依赖 / Node.js Dependencies

```bash
# 进入前端目录
cd AuditFlow.UI

# 安装依赖
npm install
```

### 5.4 配置数据库 / Configure Database

1. 确保 SQL Server LocalDB 已安装
2. 运行项目，数据库会自动创建
3. 或手动运行迁移（如果需要）

---

## 🎯 使用 GitHub Copilot 继续开发 / Continue Development with GitHub Copilot

### 示例：添加新功能 / Example: Add New Feature

1. **打开相关代码文件**
2. **开始输入注释描述功能**，例如：
   ```csharp
   // Add new endpoint to get device details by SN
   // 添加新端点根据序列号获取设备详情
   ```
3. **Copilot 会自动生成代码建议**
4. **按 `Tab` 接受建议，或继续输入**

### 示例：修改现有代码 / Example: Modify Existing Code

1. **选择要修改的代码块**
2. **输入你的修改意图作为注释**
3. **Copilot 会提供修改建议**
4. **按 `Tab` 接受**

### 示例：添加测试 / Example: Add Tests

1. **创建测试文件**
2. **输入测试描述**，例如：
   ```csharp
   // Test that audit summary returns correct compliant count
   // 测试审计摘要返回正确的合规数量
   ```
3. **Copilot 会生成测试代码**

---

## 📝 工作流程最佳实践 / Best Practices for Workflow

### 每日开发流程 / Daily Development Workflow

1. **拉取最新代码 / Pull Latest Code**
   ```bash
   git pull origin main
   ```

2. **创建新分支（如果需要） / Create New Branch (if needed)**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **使用 Copilot 编写代码 / Write Code with Copilot**

4. **测试代码 / Test Code**

5. **提交更改 / Commit Changes**
   ```bash
   git add .
   git commit -m "Description of changes"
   ```

6. **推送到 GitHub / Push to GitHub**
   ```bash
   git push origin main
   # 或推送分支 / Or push branch
   git push origin feature/your-feature-name
   ```

---

## 🔍 VS Code 集成 Git 功能 / VS Code Integrated Git Features

### 源代码管理面板 / Source Control Panel

- **`Ctrl+Shift+G`** - 打开源代码管理
- **查看更改差异 / View Changes**
- **暂存更改 / Stage Changes**
- **提交更改 / Commit Changes**
- **推送/拉取 / Push/Pull**

### Git 命令面板 / Git Command Palette

- **`Ctrl+Shift+P`** - 打开命令面板
- **输入 `Git:`** 查看所有 Git 命令
- **常用命令 / Common Commands**:
  - `Git: Pull` - 拉取
  - `Git: Push` - 推送
  - `Git: Commit` - 提交
  - `Git: Show Output` - 显示 Git 输出

### 分支管理 / Branch Management

- **点击左下角分支名称**可以：
  - 创建新分支 / Create new branch
  - 切换分支 / Switch branch
  - 合并分支 / Merge branch

---

## ⚠️ 常见问题 / Common Issues

### 问题 1: Copilot 不工作 / Copilot Not Working

**解决方法 / Solution**:
1. 检查是否已登录 GitHub
2. 检查 Copilot 订阅状态
3. 重启 VS Code
4. 重新安装 Copilot 扩展

### 问题 2: Git 推送失败 / Git Push Failed

**解决方法 / Solution**:
1. 确保已配置 GitHub 认证
2. 使用 Personal Access Token
3. 检查网络连接

### 问题 3: 依赖安装失败 / Dependencies Installation Failed

**解决方法 / Solution**:
1. 检查 Node.js 和 .NET SDK 版本
2. 清除缓存后重试
   ```bash
   # Node.js
   npm cache clean --force
   npm install
   
   # .NET
   dotnet nuget locals all --clear
   dotnet restore
   ```

---

## ✅ 验证清单 / Verification Checklist

完成以下步骤后，你的开发环境就准备好了：

- [ ] VS Code 已安装
- [ ] 项目已从 GitHub 克隆到本地
- [ ] GitHub Copilot 扩展已安装并登录
- [ ] .NET SDK 已安装（检查: `dotnet --version`）
- [ ] Node.js 已安装（检查: `node --version`）
- [ ] 项目依赖已恢复（运行 `dotnet restore` 和 `npm install`）
- [ ] 项目可以正常启动
- [ ] Git 可以正常推送/拉取

---

## 📚 总结 / Summary

### 完整工作流程 / Complete Workflow

```
1. VS Code 克隆项目
   ↓
2. 安装扩展（GitHub Copilot）
   ↓
3. 配置开发环境（.NET, Node.js）
   ↓
4. 恢复依赖
   ↓
5. 使用 Copilot 开发新功能
   ↓
6. 提交并推送到 GitHub
```

### 关键优势 / Key Benefits

- ✅ VS Code 和 GitHub 无缝集成
- ✅ GitHub Copilot 提供 AI 编程辅助
- ✅ 代码版本控制和协作更方便
- ✅ 多设备开发同步更容易

---

**现在你可以在任何电脑上克隆项目，并使用 GitHub Copilot 继续开发了！**  
**Now you can clone the project on any computer and continue development with GitHub Copilot!**
