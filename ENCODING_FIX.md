# PowerShell Encoding Fix (编码修复说明)

## Problem (问题)
PowerShell displays Chinese characters as `????` (乱码)

## Solution (解决方案)

### Option 1: Run encoding fix script (运行编码修复脚本)
```powershell
.\fix-encoding.ps1
```

### Option 2: Set encoding manually (手动设置编码)
```powershell
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
chcp 65001
```

### Option 3: Use PowerShell 7+ (使用 PowerShell 7+)
PowerShell 7+ (pwsh) has better UTF-8 support by default.

### Option 4: Set in PowerShell Profile (设置 PowerShell 配置文件)
Add to your PowerShell profile (`$PROFILE`):
```powershell
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
```

## Note (注意)
From now on, I'll use English in PowerShell commands to avoid encoding issues.
