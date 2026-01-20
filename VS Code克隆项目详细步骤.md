# VS Code 克隆项目详细步骤 / VS Code Clone Project Detailed Steps

## 🔍 如果找不到 "Git: Clone" 命令 / If You Can't Find "Git: Clone" Command

### 方法 1: 使用 VS Code 欢迎页面（最简单） / Method 1: Using VS Code Welcome Page (Easiest)

#### 步骤 / Steps:

1. **打开 VS Code**
2. **查看欢迎页面**（如果没有显示，按 `Ctrl+Shift+P`，输入 `Welcome`）
3. **点击 "Clone Git Repository" 按钮**
   - 或点击 **"File"** -> **"Clone Repository"**

4. **输入 GitHub URL**:
   ```
   https://github.com/jaxberwu/AuditFlow-Project.git
   ```

5. **选择保存位置**

6. **选择是否在新窗口中打开**

---

### 方法 2: 使用命令面板（尝试不同命令名） / Method 2: Using Command Palette (Try Different Command Names)

#### 步骤 / Steps:

1. **按 `Ctrl+Shift+P` 打开命令面板**

2. **尝试输入以下任一命令**（不同 VS Code 版本可能名称不同）:
   - `Git: Clone`
   - `Clone Repository`
   - `Git Clone`
   - `clone`
   - `Git: Clone Repository`
   - `Repository: Clone`

3. **选择匹配的命令**

4. **输入 GitHub URL**

---

### 方法 3: 使用命令行（最可靠） / Method 3: Using Command Line (Most Reliable)

#### 步骤 / Steps:

1. **打开 PowerShell 或命令提示符**

2. **导航到你想保存项目的目录**:
   ```powershell
   cd D:\Projects
   # 或任何你想保存的位置
   ```

3. **克隆仓库**:
   ```powershell
   git clone https://github.com/jaxberwu/AuditFlow-Project.git
   ```

4. **进入项目目录**:
   ```powershell
   cd AuditFlow-Project
   ```

5. **用 VS Code 打开**:
   ```powershell
   code .
   ```
   或者手动在 VS Code 中: **File** -> **Open Folder** -> 选择 `AuditFlow-Project` 文件夹

---

### 方法 4: 使用 GitHub 扩展 / Method 4: Using GitHub Extension

#### 步骤 / Steps:

1. **安装 GitHub 扩展**:
   - 打开扩展面板（`Ctrl+Shift+X`）
   - 搜索 "GitHub"
   - 安装 "GitHub Pull Requests and Issues" 或 "GitHub Repositories"

2. **使用扩展克隆**:
   - 扩展安装后，会有新的克隆选项
   - 按 `Ctrl+Shift+P`，输入 `GitHub: Clone`

---

### 方法 5: 直接从 GitHub 网站下载 / Method 5: Download Directly from GitHub Website

#### 步骤 / Steps:

1. **访问 GitHub 仓库**:
   ```
   https://github.com/jaxberwu/AuditFlow-Project
   ```

2. **点击绿色的 "Code" 按钮**

3. **选择 "Download ZIP"**

4. **解压 ZIP 文件**

5. **用 VS Code 打开解压后的文件夹**:
   - **File** -> **Open Folder**

**注意**: 这种方法不会包含 Git 历史，如果需要 Git 功能，建议使用方法 3（命令行）

---

## ⚠️ 常见问题 / Common Issues

### 问题 1: 命令面板中没有 Git 相关命令 / No Git Commands in Command Palette

**可能原因 / Possible Reasons**:
- Git 未安装
- VS Code 未检测到 Git

**解决方法 / Solution**:

1. **检查 Git 是否安装**:
   ```powershell
   git --version
   ```

2. **如果未安装，下载安装 Git**:
   - Windows: https://git-scm.com/download/win
   - 安装后重启 VS Code

3. **在 VS Code 中配置 Git 路径**（如果需要）:
   - 按 `Ctrl+,` 打开设置
   - 搜索 `git.path`
   - 输入 Git 可执行文件的路径

---

### 问题 2: VS Code 版本太旧 / VS Code Version Too Old

**解决方法 / Solution**:
- 更新 VS Code 到最新版本
- 下载: https://code.visualstudio.com/

---

### 问题 3: 网络连接问题 / Network Connection Issue

**解决方法 / Solution**:
- 检查网络连接
- 如果使用代理，配置 Git 代理:
  ```powershell
  git config --global http.proxy http://proxy.example.com:8080
  ```

---

## ✅ 推荐方法 / Recommended Method

**最可靠的方法是使用命令行**:

```powershell
# 1. 打开 PowerShell
# Open PowerShell

# 2. 导航到项目目录
# Navigate to project directory
cd D:\Projects

# 3. 克隆项目
# Clone project
git clone https://github.com/jaxberwu/AuditFlow-Project.git

# 4. 进入项目目录
# Enter project directory
cd AuditFlow-Project

# 5. 用 VS Code 打开
# Open with VS Code
code .
```

---

## 📝 验证克隆是否成功 / Verify Clone Success

克隆完成后，检查以下内容:

- [ ] 项目文件夹已创建
- [ ] 文件夹中包含 `.git` 目录（隐藏文件夹）
- [ ] 所有源代码文件都在
- [ ] VS Code 左下角显示分支名称（如 `main`）

---

## 🎯 下一步 / Next Steps

克隆成功后:

1. **安装项目依赖**:
   ```powershell
   # .NET 依赖
   dotnet restore
   
   # Node.js 依赖
   cd AuditFlow.UI
   npm install
   ```

2. **安装 VS Code 扩展**:
   - C# Dev Kit
   - GitHub Copilot

3. **开始开发！**

---

**如果以上方法都不行，请告诉我你看到的错误信息，我会帮你解决！**  
**If none of these methods work, please tell me the error message you see, and I'll help you solve it!**
