# Git 提交和更新说明 / Git Commit and Update Guide

## 如何提交更新到 GitHub / How to Commit Updates to GitHub

### 自动更新机制 / Auto-Update Mechanism

**重要说明 / Important Note**:  
Git **不会自动更新**。每次提交都需要手动执行命令。  
Git does **NOT automatically update**. You need to manually execute commands for each commit.

### 提交新版本的步骤 / Steps to Commit New Version

#### 1. 检查更改状态 / Check Change Status

```bash
git status
```

这会显示哪些文件被修改了。  
This shows which files have been modified.

#### 2. 添加更改的文件 / Add Changed Files

```bash
# 添加所有更改 / Add all changes
git add .

# 或者添加特定文件 / Or add specific files
git add AuditFlow.UI/src/App.tsx
git add README.md
```

#### 3. 创建提交 / Create Commit

```bash
git commit -m "Update: Add CrowdStrike source labels and bilingual documentation"
# 或中文 / Or in Chinese
git commit -m "更新：添加 CrowdStrike 来源标记和中英文双语文档"
```

#### 4. 推送到 GitHub / Push to GitHub

```bash
git push origin main
```

**注意 / Note**:  
- 如果这是第一次推送，使用 `git push -u origin main`  
- If this is the first push, use `git push -u origin main`
- 推送时需要 GitHub 认证（用户名和 Personal Access Token）  
- You need GitHub authentication (username and Personal Access Token) to push

### 版本更新说明 / Version Update Explanation

#### 每次提交都会创建新版本 / Each Commit Creates a New Version

- **提交 ID (Commit Hash)**: 每次提交都会生成唯一的 ID（如 `aad59ae`）  
  Each commit generates a unique ID (e.g., `aad59ae`)

- **版本历史 (Version History)**: 所有提交都保存在 Git 历史中，可以随时查看和回退  
  All commits are saved in Git history and can be viewed or reverted at any time

- **GitHub 上的更新 (Updates on GitHub)**: 推送后，GitHub 上的代码会立即更新到最新版本  
  After pushing, the code on GitHub will be immediately updated to the latest version

#### 查看提交历史 / View Commit History

```bash
# 查看提交历史 / View commit history
git log --oneline

# 查看详细历史 / View detailed history
git log
```

#### 回退到之前的版本 / Revert to Previous Version

```bash
# 查看所有提交 / View all commits
git log --oneline

# 回退到指定提交 / Revert to specific commit
git checkout <commit-hash>

# 回到最新版本 / Return to latest version
git checkout main
```

### 最佳实践 / Best Practices

1. **提交前检查 / Check Before Commit**
   ```bash
   git status
   git diff  # 查看具体更改内容 / View specific changes
   ```

2. **有意义的提交信息 / Meaningful Commit Messages**
   - 描述做了什么更改 / Describe what changes were made
   - 使用清晰的语言 / Use clear language
   - 示例 / Example: `"Add CrowdStrike source labels to UI"`

3. **定期提交 / Regular Commits**
   - 完成一个功能后立即提交 / Commit immediately after completing a feature
   - 不要积累太多更改 / Don't accumulate too many changes

4. **推送前测试 / Test Before Push**
   - 确保代码可以正常运行 / Ensure code runs correctly
   - 检查是否有编译错误 / Check for compilation errors

### 常见问题 / Common Issues

#### 问题 1: 推送被拒绝 / Push Rejected

**原因 / Reason**: 远程仓库有新的提交，本地没有  
Remote repository has new commits that local doesn't have

**解决方法 / Solution**:
```bash
# 先拉取远程更改 / Pull remote changes first
git pull origin main

# 解决冲突（如果有） / Resolve conflicts (if any)
# 然后再次推送 / Then push again
git push origin main
```

#### 问题 2: 忘记添加文件 / Forgot to Add Files

**解决方法 / Solution**:
```bash
# 添加遗漏的文件 / Add missing files
git add <file-name>

# 修改上次提交 / Amend last commit
git commit --amend --no-edit

# 推送 / Push
git push origin main
```

#### 问题 3: 提交了错误的文件 / Committed Wrong Files

**解决方法 / Solution**:
```bash
# 撤销上次提交（保留更改） / Undo last commit (keep changes)
git reset --soft HEAD~1

# 重新添加正确的文件 / Re-add correct files
git add <correct-files>

# 重新提交 / Commit again
git commit -m "Correct commit message"
```

### 快速提交脚本 / Quick Commit Script

你可以使用之前创建的 `upload-to-github.ps1` 脚本，或者手动执行：

You can use the previously created `upload-to-github.ps1` script, or execute manually:

```bash
git add .
git commit -m "Your commit message"
git push origin main
```

---

## 总结 / Summary

- ✅ Git **不会自动更新**，需要手动提交和推送  
  Git does **NOT automatically update**, requires manual commit and push

- ✅ 每次 `git push` 后，GitHub 上的代码会更新到最新版本  
  After each `git push`, the code on GitHub will be updated to the latest version

- ✅ 所有提交历史都保存在 Git 中，可以随时查看和回退  
  All commit history is saved in Git and can be viewed or reverted at any time

- ✅ 建议使用有意义的提交信息，方便后续查找  
  Recommend using meaningful commit messages for easier searching later
