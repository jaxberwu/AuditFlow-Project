# AuditFlow 项目打包脚本
# 用途: 创建项目压缩包，排除不必要的文件

$packageName = "AuditFlow-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
$tempDir = ".\$packageName"

# 创建临时目录
Write-Host "=== Creating Package: $packageName ===" -ForegroundColor Cyan
New-Item -ItemType Directory -Path $tempDir -Force | Out-Null

# 复制解决方案文件
Write-Host "`n1. Copying solution file..." -ForegroundColor Yellow
if (Test-Path "AuditFlow.sln") {
    Copy-Item "AuditFlow.sln" -Destination "$tempDir\" -Force
    Write-Host "   ✓ AuditFlow.sln" -ForegroundColor Green
} else {
    Write-Host "   ✗ AuditFlow.sln not found" -ForegroundColor Red
}

# 复制后端项目（排除 bin/obj）
Write-Host "`n2. Copying backend projects..." -ForegroundColor Yellow
$projects = @("AuditFlow.Shared", "AuditFlow.Simulator", "AuditFlow.Engine")
foreach ($project in $projects) {
    if (Test-Path $project) {
        Write-Host "   Copying $project..." -ForegroundColor Gray
        $dest = "$tempDir\$project"
        New-Item -ItemType Directory -Path $dest -Force | Out-Null
        
        # 复制文件，排除 bin, obj, *.user
        Get-ChildItem -Path $project -Recurse | Where-Object {
            $_.FullName -notmatch "\\bin\\" -and
            $_.FullName -notmatch "\\obj\\" -and
            $_.Name -notmatch "\.user$"
        } | Copy-Item -Destination {
            $_.FullName -replace [regex]::Escape((Get-Location).Path + "\$project"), "$dest"
        } -Force -ErrorAction SilentlyContinue
        
        Write-Host "   ✓ $project" -ForegroundColor Green
    } else {
        Write-Host "   ✗ $project not found" -ForegroundColor Red
    }
}

# 复制前端项目（排除 node_modules 和 dist）
Write-Host "`n3. Copying frontend project..." -ForegroundColor Yellow
if (Test-Path "AuditFlow.UI") {
    $dest = "$tempDir\AuditFlow.UI"
    New-Item -ItemType Directory -Path $dest -Force | Out-Null
    
    # 复制文件，排除 node_modules, dist, .vs
    Get-ChildItem -Path "AuditFlow.UI" -Recurse | Where-Object {
        $_.FullName -notmatch "\\node_modules\\" -and
        $_.FullName -notmatch "\\dist\\" -and
        $_.FullName -notmatch "\\.vs\\" -and
        $_.Name -notmatch "\.user$"
    } | Copy-Item -Destination {
        $_.FullName -replace [regex]::Escape((Get-Location).Path + "\AuditFlow.UI"), "$dest"
    } -Force -ErrorAction SilentlyContinue
    
    Write-Host "   ✓ AuditFlow.UI" -ForegroundColor Green
} else {
    Write-Host "   ✗ AuditFlow.UI not found" -ForegroundColor Red
}

# 复制文档文件
Write-Host "`n4. Copying documentation..." -ForegroundColor Yellow
$docs = Get-ChildItem -Path "." -Filter "*.md" | Where-Object { $_.Name -ne "README.md" }
foreach ($doc in $docs) {
    Copy-Item $doc.FullName -Destination "$tempDir\" -Force
    Write-Host "   ✓ $($doc.Name)" -ForegroundColor Gray
}

# 复制脚本文件
Write-Host "`n5. Copying scripts..." -ForegroundColor Yellow
$scripts = Get-ChildItem -Path "." -Filter "*.ps1" -Exclude "package-project.ps1"
foreach ($script in $scripts) {
    Copy-Item $script.FullName -Destination "$tempDir\" -Force
    Write-Host "   ✓ $($script.Name)" -ForegroundColor Gray
}

# 创建压缩包
Write-Host "`n6. Creating ZIP archive..." -ForegroundColor Yellow
$zipPath = "$packageName.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}
Compress-Archive -Path "$tempDir\*" -DestinationPath $zipPath -Force

# 清理临时目录
Remove-Item -Path $tempDir -Recurse -Force

# 显示结果
Write-Host "`n=== Package Created Successfully ===" -ForegroundColor Green
Write-Host "`nPackage: $zipPath" -ForegroundColor Cyan
$size = (Get-Item $zipPath).Length / 1MB
Write-Host "Size: $([math]::Round($size, 2)) MB" -ForegroundColor White
Write-Host "`nExcluded directories:" -ForegroundColor Yellow
Write-Host "  - bin/" -ForegroundColor Gray
Write-Host "  - obj/" -ForegroundColor Gray
Write-Host "  - node_modules/" -ForegroundColor Gray
Write-Host "  - dist/" -ForegroundColor Gray
Write-Host "`nReady for deployment! 🚀" -ForegroundColor Green
