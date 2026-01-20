# Fix PowerShell encoding for Chinese characters
# Run this script before using PowerShell commands

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
chcp 65001 | Out-Null

Write-Host "Encoding fixed to UTF-8" -ForegroundColor Green
Write-Host "You can now use Chinese characters in PowerShell" -ForegroundColor Green
