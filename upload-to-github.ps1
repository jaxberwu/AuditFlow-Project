# AuditFlow 自动化上传到 GitHub 脚本
# 功能: 初始化 Git、添加文件、提交、推送到 GitHub

param(
    [string]$GitHubUrl = "",
    [switch]$SkipPush = $false
)

Write-Host "=== AuditFlow GitHub Upload Script ===" -ForegroundColor Cyan
Write-Host "`nThis script will:" -ForegroundColor Yellow
Write-Host "  1. Initialize Git repository (if needed)" -ForegroundColor White
Write-Host "  2. Add all files (excluding .gitignore items)" -ForegroundColor White
Write-Host "  3. Create initial commit" -ForegroundColor White
Write-Host "  4. Configure remote repository" -ForegroundColor White
Write-Host "  5. Push to GitHub (if URL provided)" -ForegroundColor White

# 检查 Git 是否安装
Write-Host "`n=== Step 1: Checking Git Installation ===" -ForegroundColor Cyan
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Host "✗ Git is not installed!" -ForegroundColor Red
    Write-Host "`nPlease install Git first:" -ForegroundColor Yellow
    Write-Host "  Download: https://git-scm.com/download/win" -ForegroundColor White
    exit 1
}

$gitVersion = git --version
Write-Host "✓ Git is installed: $gitVersion" -ForegroundColor Green

# 检查 .gitignore
Write-Host "`n=== Step 2: Checking .gitignore ===" -ForegroundColor Cyan
if (-not (Test-Path ".gitignore")) {
    Write-Host "⚠ .gitignore not found, but it should be created" -ForegroundColor Yellow
    Write-Host "  (It was created earlier, checking again...)" -ForegroundColor Gray
} else {
    Write-Host "✓ .gitignore exists" -ForegroundColor Green
}

# 初始化 Git（如果需要）
Write-Host "`n=== Step 3: Initializing Git Repository ===" -ForegroundColor Cyan
if (-not (Test-Path ".git")) {
    Write-Host "Initializing Git repository..." -ForegroundColor Yellow
    git init
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Git repository initialized" -ForegroundColor Green
    } else {
        Write-Host "✗ Failed to initialize Git repository" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "✓ Git repository already exists" -ForegroundColor Green
}

# 检查是否有未提交的更改
Write-Host "`n=== Step 4: Checking Changes ===" -ForegroundColor Cyan
git status --short | Out-Null
$hasChanges = $LASTEXITCODE -eq 0 -and (git status --short | Measure-Object -Line).Lines -gt 0

if (-not $hasChanges -and (Test-Path ".git\refs\heads\main") -or (Test-Path ".git\refs\heads\master")) {
    Write-Host "⚠ No changes to commit" -ForegroundColor Yellow
    $hasChanges = $false
} else {
    Write-Host "Found files to commit..." -ForegroundColor Yellow
}

# 添加文件
if ($hasChanges) {
    Write-Host "`n=== Step 5: Adding Files ===" -ForegroundColor Cyan
    Write-Host "Adding all files (excluding .gitignore items)..." -ForegroundColor Yellow
    git add .
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Files added" -ForegroundColor Green
        
        # 显示将要提交的文件
        $filesToCommit = git status --short | Select-Object -First 10
        if ($filesToCommit) {
            Write-Host "`nFiles to be committed (first 10):" -ForegroundColor Gray
            $filesToCommit | ForEach-Object { Write-Host "  $_" -ForegroundColor White }
        }
    } else {
        Write-Host "✗ Failed to add files" -ForegroundColor Red
        exit 1
    }
} else {
    # 如果没有更改，检查是否需要初始提交
    $hasCommits = git rev-parse --verify HEAD 2>$null
    if (-not $hasCommits) {
        Write-Host "`n=== Step 5: Adding Files (Initial Commit) ===" -ForegroundColor Cyan
        Write-Host "Adding all files for initial commit..." -ForegroundColor Yellow
        git add .
        $hasChanges = $true
    }
}

# 创建提交
if ($hasChanges) {
    Write-Host "`n=== Step 6: Creating Commit ===" -ForegroundColor Cyan
    $commitMessage = "Initial commit: AuditFlow Enterprise Audit System"
    
    Write-Host "Creating commit: '$commitMessage'" -ForegroundColor Yellow
    git commit -m $commitMessage
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Commit created successfully" -ForegroundColor Green
    } else {
        Write-Host "⚠ Commit may have failed or no changes to commit" -ForegroundColor Yellow
    }
} else {
    Write-Host "`n=== Step 6: Commit ===" -ForegroundColor Cyan
    Write-Host "✓ Already committed, skipping..." -ForegroundColor Green
}

# 配置远程仓库
Write-Host "`n=== Step 7: Configuring Remote Repository ===" -ForegroundColor Cyan

# 如果没有提供 URL，尝试从现有配置获取
if ([string]::IsNullOrWhiteSpace($GitHubUrl)) {
    $existingRemote = git remote get-url origin 2>$null
    if ($existingRemote) {
        Write-Host "✓ Remote already configured: $existingRemote" -ForegroundColor Green
        $GitHubUrl = $existingRemote
    } else {
        Write-Host "⚠ No GitHub URL provided and no remote configured" -ForegroundColor Yellow
        Write-Host "`nTo complete the upload, you need to:" -ForegroundColor Cyan
        Write-Host "  1. Create a repository on GitHub" -ForegroundColor White
        Write-Host "  2. Run this script again with the repository URL:" -ForegroundColor White
        Write-Host "     .\upload-to-github.ps1 -GitHubUrl 'https://github.com/yourusername/AuditFlow.git'" -ForegroundColor Gray
        Write-Host "`nOr manually configure remote:" -ForegroundColor Yellow
        Write-Host "  git remote add origin https://github.com/yourusername/AuditFlow.git" -ForegroundColor Gray
        Write-Host "  git push -u origin main" -ForegroundColor Gray
        exit 0
    }
} else {
    # 检查 remote 是否已存在
    $existingRemote = git remote get-url origin 2>$null
    if ($existingRemote) {
        if ($existingRemote -ne $GitHubUrl) {
            Write-Host "⚠ Remote already exists with different URL" -ForegroundColor Yellow
            Write-Host "  Current: $existingRemote" -ForegroundColor Gray
            Write-Host "  New:     $GitHubUrl" -ForegroundColor Gray
            $update = Read-Host "Update remote? (y/n)"
            if ($update -eq 'y' -or $update -eq 'Y') {
                git remote set-url origin $GitHubUrl
                Write-Host "✓ Remote URL updated" -ForegroundColor Green
            }
        } else {
            Write-Host "✓ Remote already configured correctly" -ForegroundColor Green
        }
    } else {
        git remote add origin $GitHubUrl
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Remote repository added: $GitHubUrl" -ForegroundColor Green
        } else {
            Write-Host "✗ Failed to add remote repository" -ForegroundColor Red
            exit 1
        }
    }
}

# 设置默认分支为 main
Write-Host "`n=== Step 8: Setting Default Branch ===" -ForegroundColor Cyan
$currentBranch = git branch --show-current
if ($currentBranch -ne "main") {
    git branch -M main 2>$null
    Write-Host "✓ Branch renamed to 'main'" -ForegroundColor Green
} else {
    Write-Host "✓ Already on 'main' branch" -ForegroundColor Green
}

# 推送到 GitHub
if (-not $SkipPush) {
    Write-Host "`n=== Step 9: Pushing to GitHub ===" -ForegroundColor Cyan
    Write-Host "Pushing to GitHub..." -ForegroundColor Yellow
    Write-Host "`nNote: You may be prompted for GitHub credentials:" -ForegroundColor Yellow
    Write-Host "  - Username: Your GitHub username" -ForegroundColor White
    Write-Host "  - Password: Use Personal Access Token (not GitHub password)" -ForegroundColor White
    Write-Host "`nIf you don't have a token, create one at:" -ForegroundColor Cyan
    Write-Host "  https://github.com/settings/tokens" -ForegroundColor White
    Write-Host ""
    
    git push -u origin main
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n✓ Successfully pushed to GitHub!" -ForegroundColor Green
        Write-Host "`nRepository URL: $GitHubUrl" -ForegroundColor Cyan
        Write-Host "You can now access it on GitHub!" -ForegroundColor Green
    } else {
        Write-Host "`n⚠ Push may have failed. Common reasons:" -ForegroundColor Yellow
        Write-Host "  1. Authentication failed (need Personal Access Token)" -ForegroundColor White
        Write-Host "  2. Repository doesn't exist or you don't have access" -ForegroundColor White
        Write-Host "  3. Network connection issue" -ForegroundColor White
        Write-Host "`nYou can try pushing manually:" -ForegroundColor Cyan
        Write-Host "  git push -u origin main" -ForegroundColor Gray
    }
} else {
    Write-Host "`n=== Step 9: Push (Skipped) ===" -ForegroundColor Cyan
    Write-Host "Push skipped. To push manually, run:" -ForegroundColor Yellow
    Write-Host "  git push -u origin main" -ForegroundColor Gray
}

Write-Host "`n=== Upload Process Complete ===" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "  1. Verify files on GitHub" -ForegroundColor White
Write-Host "  2. Share repository URL with others" -ForegroundColor White
Write-Host "  3. Use 'git pull' to download updates" -ForegroundColor White
