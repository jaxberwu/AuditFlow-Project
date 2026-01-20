# Git 安装指南 / Git Installation Guide

## ⚠️ 错误信息 / Error Message

```
git is not a recognized command
```

这表示 Git 还没有安装或者没有正确添加到系统 PATH 环境变量中。  
This means Git is not installed or not properly added to the system PATH environment variable.

---

## 🔧 解决方案 / Solutions

### 方法 1: 安装 Git for Windows（推荐） / Method 1: Install Git for Windows (Recommended)

#### 步骤 / Steps:

1. **下载 Git for Windows**
   - 访问: https://git-scm.com/download/win
   - 或直接下载: https://github.com/git-for-windows/git/releases/latest
   - 选择 `Git-2.xx.x-64-bit.exe`（最新版本）

2. **运行安装程序**
   - 双击下载的 `.exe` 文件
   - 按照安装向导进行安装

3. **安装选项（建议设置）**
   - ✅ 选择 "Use Git from the command line and also from 3rd-party software"
   - ✅ 选择 "Use the OpenSSL library"
   - ✅ 选择 "Checkout Windows-style, commit Unix-style line endings"
   - ✅ 选择 "Use Windows' default console window"
   - ✅ 其他选项保持默认

4. **完成安装后**
   - 关闭所有 PowerShell 或命令提示符窗口
   - 重新打开 PowerShell

5. **验证安装**
   ```powershell
   git --version
   ```
   应该显示类似: `git version 2.xx.x.windows.x`

---

### 方法 2: 安装 GitHub Desktop（更简单） / Method 2: Install GitHub Desktop (Easier)

#### 步骤 / Steps:

1. **下载 GitHub Desktop**
   - 访问: https://desktop.github.com/
   - 点击 "Download for Windows"

2. **运行安装程序**
   - 双击下载的安装程序
   - GitHub Desktop 会自动安装 Git（如果未安装）

3. **登录 GitHub 账户**

4. **使用 GitHub Desktop 克隆项目**
   - 点击 "File" -> "Clone Repository"
   - 输入: `https://github.com/jaxberwu/AuditFlow-Project.git`
   - 选择保存位置
   - 点击 "Clone"

5. **用 VS Code 打开项目**
   - 在 GitHub Desktop 中，右键项目 -> "Open in Visual Studio Code"

**优点 / Advantages**:
- ✅ 自动安装 Git
- ✅ 图形界面，更易用
- ✅ 内置 VS Code 集成

---

### 方法 3: 直接下载 ZIP 文件（不需要 Git） / Method 3: Download ZIP Directly (No Git Required)

如果你只是想获取代码，不需要 Git 功能，可以直接下载 ZIP 文件：

#### 步骤 / Steps:

1. **访问 GitHub 仓库**
   ```
   https://github.com/jaxberwu/AuditFlow-Project
   ```

2. **下载 ZIP**
   - 点击绿色的 "Code" 按钮
   - 选择 "Download ZIP"
   - 保存到本地

3. **解压 ZIP 文件**
   - 右键 ZIP 文件 -> "Extract All..."
   - 选择解压位置

4. **用 VS Code 打开**
   - 打开 VS Code
   - File -> Open Folder
   - 选择解压后的 `AuditFlow-Project` 文件夹

**注意 / Note**: 
- ⚠️ 这种方法不包含 Git 历史记录
- ⚠️ 无法使用 Git 推送/拉取功能
- ✅ 但可以正常打开和编辑代码
- ✅ 适合只是查看或编辑代码，不需要版本控制的情况

---

## 🎯 推荐方案 / Recommended Solutions

### 场景 1: 需要完整 Git 功能
**推荐**: 安装 Git for Windows  
**步骤**: 使用方法 1

### 场景 2: 想要图形界面，简单易用
**推荐**: 安装 GitHub Desktop  
**步骤**: 使用方法 2

### 场景 3: 只需要代码，不需要版本控制
**推荐**: 直接下载 ZIP  
**步骤**: 使用方法 3

---

## ✅ 安装后验证 / Verification After Installation

### 检查 Git 是否安装成功

1. **打开 PowerShell**（必须重新打开）

2. **运行命令**:
   ```powershell
   git --version
   ```

3. **如果显示版本号**，说明安装成功:
   ```
   git version 2.xx.x.windows.x
   ```

4. **如果仍然报错**:
   - 重启电脑
   - 或检查 PATH 环境变量

---

## 🔍 如果安装后仍然无法识别 / If Still Not Recognized After Installation

### 检查 PATH 环境变量

1. **打开系统属性**
   - 右键 "此电脑" -> "属性"
   - 点击 "高级系统设置"
   - 点击 "环境变量"

2. **检查 PATH 变量**
   - 在 "系统变量" 中找到 "Path"
   - 确认包含 Git 安装路径，例如:
     ```
     C:\Program Files\Git\cmd
     C:\Program Files\Git\bin
     ```

3. **如果没有，添加路径**
   - 点击 "编辑"
   - 添加 Git 的 `cmd` 和 `bin` 文件夹路径
   - Git 默认安装位置: `C:\Program Files\Git\`

4. **保存并重启 PowerShell**

---

## 📝 安装 Git 后的下一步 / Next Steps After Installing Git

### 1. 配置 Git（首次使用）

```powershell
# 设置用户名
git config --global user.name "Your Name"

# 设置邮箱
git config --global user.email "your.email@example.com"
```

### 2. 克隆项目

```powershell
# 克隆项目
git clone https://github.com/jaxberwu/AuditFlow-Project.git

# 进入项目目录
cd AuditFlow-Project

# 用 VS Code 打开
code .
```

---

## 🆘 快速解决方案总结 / Quick Solution Summary

**最简单的方法 / Simplest Method**:

1. **下载并安装 GitHub Desktop**
   - https://desktop.github.com/
   - 自动包含 Git
   - 图形界面操作

2. **使用 GitHub Desktop 克隆项目**

3. **用 VS Code 打开项目**

**或者 / Or**:

1. **直接下载 ZIP**
   - 访问: https://github.com/jaxberwu/AuditFlow-Project
   - 点击 Code -> Download ZIP
   - 解压后用 VS Code 打开

---

**选择最适合你的方法，如果需要帮助，告诉我你遇到的具体问题！**  
**Choose the method that works best for you. If you need help, tell me the specific issue you encounter!**
