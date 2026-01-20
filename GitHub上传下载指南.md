# GitHub 上传和下载指南

## 📤 上传到 GitHub

### 步骤 1: 在 GitHub 上创建仓库

1. **登录 GitHub**
   - 访问: https://github.com
   - 登录你的账号

2. **创建新仓库**
   - 点击右上角 "+" → "New repository"
   - Repository name: `AuditFlow` (或你喜欢的名字)
   - Description: `Enterprise Asset Compliance Audit System`
   - 选择: **Public** 或 **Private**
   - **不要**勾选 "Initialize with README"（如果已有代码）
   - 点击 "Create repository"

3. **复制仓库 URL**
   - 复制显示的 URL，例如: `https://github.com/yourusername/AuditFlow.git`

---

### 步骤 2: 初始化本地 Git 仓库

**在项目根目录执行**:

```powershell
# 1. 初始化 Git 仓库
git init

# 2. 添加所有文件（.gitignore 会自动排除不需要的文件）
git add .

# 3. 提交文件
git commit -m "Initial commit: AuditFlow system"

# 4. 添加远程仓库（替换为你的 GitHub 仓库 URL）
git remote add origin https://github.com/yourusername/AuditFlow.git

# 5. 推送到 GitHub
git branch -M main
git push -u origin main
```

---

### 步骤 3: 验证上传

1. **刷新 GitHub 页面**
   - 应该看到所有文件已上传

2. **检查文件**
   - 确认所有源代码文件都在
   - 确认 `bin/`, `obj/`, `node_modules/` 等被排除

---

## 📥 从 GitHub 下载

### 方式 1: 使用 Git Clone（推荐）

**在新电脑上执行**:

```powershell
# 1. 安装 Git（如果还没安装）
# 下载: https://git-scm.com/download/win

# 2. Clone 仓库
git clone https://github.com/yourusername/AuditFlow.git

# 3. 进入项目目录
cd AuditFlow

# 4. 验证文件
dir AuditFlow.sln
dir AuditFlow.Shared
dir AuditFlow.Simulator
dir AuditFlow.Engine
dir AuditFlow.UI
```

**优点**:
- ✅ 自动排除 .gitignore 中的文件
- ✅ 保留 Git 历史记录
- ✅ 可以轻松更新（`git pull`）

---

### 方式 2: 下载 ZIP 文件

**在新电脑上操作**:

1. **访问 GitHub 仓库**
   - 打开: `https://github.com/yourusername/AuditFlow`

2. **下载 ZIP**
   - 点击绿色的 "Code" 按钮
   - 选择 "Download ZIP"

3. **解压文件**
   - 解压到目标位置（如 `D:\Projects\AuditFlow`）

4. **验证文件**
   - 检查所有必需文件是否都在

**优点**:
- ✅ 简单直接
- ✅ 不需要安装 Git

**缺点**:
- ❌ 不包含 Git 历史
- ❌ 无法使用 `git pull` 更新

---

## 🔄 后续更新

### 上传更新

**在开发机器上**:

```powershell
# 1. 查看更改
git status

# 2. 添加更改的文件
git add .

# 3. 提交更改
git commit -m "Update: 描述你的更改"

# 4. 推送到 GitHub
git push
```

### 下载更新

**在新电脑上**（如果使用 Git Clone）:

```powershell
# 进入项目目录
cd AuditFlow

# 拉取最新更改
git pull
```

---

## 📋 完整操作流程

### 首次上传

```powershell
# 1. 初始化
git init

# 2. 检查 .gitignore 是否存在
if (-not (Test-Path ".gitignore")) {
    Write-Host "Creating .gitignore..." -ForegroundColor Yellow
    # .gitignore 文件已创建
}

# 3. 添加文件
git add .

# 4. 提交
git commit -m "Initial commit: AuditFlow Enterprise Audit System"

# 5. 添加远程仓库（替换为你的 URL）
git remote add origin https://github.com/yourusername/AuditFlow.git

# 6. 推送
git branch -M main
git push -u origin main
```

### 首次下载

```powershell
# 使用 Git Clone
git clone https://github.com/yourusername/AuditFlow.git
cd AuditFlow

# 或下载 ZIP 文件
# 1. 访问 GitHub 仓库
# 2. 点击 "Code" → "Download ZIP"
# 3. 解压到目标位置
```

---

## 🔐 GitHub 认证

### 使用 HTTPS（需要 Personal Access Token）

1. **创建 Personal Access Token**
   - GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
   - 点击 "Generate new token"
   - 选择权限: `repo` (完整仓库访问)
   - 复制生成的 token

2. **使用 Token**
   ```powershell
   # 推送时会提示输入用户名和密码
   # 用户名: 你的 GitHub 用户名
   # 密码: 使用 Personal Access Token（不是 GitHub 密码）
   git push
   ```

### 使用 SSH（推荐，更安全）

1. **生成 SSH Key**
   ```powershell
   ssh-keygen -t ed25519 -C "your_email@example.com"
   # 按 Enter 使用默认路径
   # 可以设置密码（可选）
   ```

2. **添加 SSH Key 到 GitHub**
   - 复制公钥: `cat ~/.ssh/id_ed25519.pub`
   - GitHub → Settings → SSH and GPG keys → New SSH key
   - 粘贴公钥并保存

3. **使用 SSH URL**
   ```powershell
   git remote set-url origin git@github.com:yourusername/AuditFlow.git
   git push
   ```

---

## ✅ 检查清单

### 上传前检查

- [ ] .gitignore 文件存在
- [ ] 所有源代码文件都在
- [ ] 配置文件都在（appsettings.json）
- [ ] 文档文件都在（.md 文件）
- [ ] 已排除 bin/, obj/, node_modules/, dist/

### 下载后检查

- [ ] 所有项目文件夹都在
- [ ] AuditFlow.sln 存在
- [ ] 所有 .csproj 文件都在
- [ ] package.json 存在
- [ ] 可以运行 `dotnet restore`
- [ ] 可以运行 `npm install`

---

## 🚀 快速命令参考

### 上传命令

```powershell
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/yourusername/AuditFlow.git
git branch -M main
git push -u origin main
```

### 下载命令

```powershell
# Git Clone
git clone https://github.com/yourusername/AuditFlow.git

# 或下载 ZIP
# 访问: https://github.com/yourusername/AuditFlow/archive/refs/heads/main.zip
```

---

## 📝 注意事项

### .gitignore 已配置

已创建的 `.gitignore` 会自动排除：
- ✅ `bin/`, `obj/` (编译输出)
- ✅ `node_modules/` (npm 依赖)
- ✅ `dist/` (前端构建输出)
- ✅ `.vs/`, `.vscode/` (IDE 配置)
- ✅ `*.user` (用户特定文件)

### 文件大小

- **源代码**: 约 2-5 MB
- **包含 node_modules**: 约 200+ MB（不推荐上传）
- **使用 .gitignore**: 只上传源代码，约 2-5 MB ✅

---

## 🎯 推荐流程

### 开发机器（上传）

1. 创建 GitHub 仓库
2. 运行 `git init`
3. 运行 `git add .`
4. 运行 `git commit -m "Initial commit"`
5. 运行 `git remote add origin <your-repo-url>`
6. 运行 `git push -u origin main`

### 新电脑（下载）

1. 安装 Git（如果还没有）
2. 运行 `git clone <your-repo-url>`
3. 按照 `迁移部署指南.md` 部署

---

**最后更新**: 2026-01-20
