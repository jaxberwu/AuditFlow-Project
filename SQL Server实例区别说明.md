# SQL Server 实例区别说明

## 两个 SQL Server 实例的区别

### 1. JAXBER\SQL (命名实例)

**类型**: SQL Server 完整版/标准版/Express 的命名实例

**特点**:
- ✅ 功能完整，支持所有 SQL Server 功能
- ✅ 可以作为生产环境数据库服务器
- ✅ 支持远程连接（如果配置了网络访问）
- ✅ 可以承载多个数据库
- ✅ 需要单独安装和配置
- ✅ 通常有独立的服务进程运行
- ✅ 性能更好，适合大型应用

**用途**:
- 生产环境
- 多用户访问
- 需要高可用性和性能的场景
- 企业级应用

**连接方式**:
```
Server name: JAXBER\SQL
或
Server name: JAXBER\SQL,1433  (如果指定端口)
```

---

### 2. (localdb)\mssqllocaldb (LocalDB)

**类型**: SQL Server LocalDB（轻量级本地数据库）

**特点**:
- ✅ 轻量级，占用资源少
- ✅ 随 Visual Studio 自动安装
- ✅ 适合开发和测试
- ❌ 仅支持本地连接（不能远程访问）
- ❌ 功能有限（不支持某些高级功能）
- ❌ 性能较低，不适合生产环境
- ✅ 数据库文件存储在用户目录
- ✅ 进程按需启动，不用时自动关闭

**用途**:
- 本地开发
- 单元测试
- 小型项目原型
- 学习和实验

**连接方式**:
```
Server name: (localdb)\mssqllocaldb
或
Server name: (localdb)\MSSQLLocalDB  (大写版本)
```

**数据库文件位置**:
```
C:\Users\{用户名}\AuditFlowDB.mdf
C:\Users\{用户名}\AuditFlowDB_log.ldf
```

---

## 📊 对比表

| 特性 | JAXBER\SQL | (localdb)\mssqllocaldb |
|------|------------|------------------------|
| **类型** | 完整 SQL Server 实例 | LocalDB (轻量级) |
| **安装** | 需要单独安装 | 随 Visual Studio 安装 |
| **资源占用** | 较高 | 较低 |
| **性能** | 高 | 中等 |
| **远程访问** | ✅ 支持 | ❌ 仅本地 |
| **生产环境** | ✅ 适合 | ❌ 不适合 |
| **开发测试** | ✅ 适合 | ✅ 更适合 |
| **多用户** | ✅ 支持 | ❌ 单用户 |
| **数据库数量** | 无限制 | 无限制 |
| **启动方式** | 服务常驻 | 按需启动 |

---

## 🎯 为什么 AuditFlow 使用 LocalDB？

### 原因：

1. **开发便利性**
   - 不需要安装和配置完整的 SQL Server
   - 随 Visual Studio 自动可用
   - 适合快速原型开发

2. **轻量级**
   - 不占用太多系统资源
   - 适合本地开发环境

3. **简单部署**
   - 数据库文件可以轻松备份和恢复
   - 适合演示和测试

4. **项目定位**
   - 这是一个模拟/原型项目
   - 将来会集成 CrowdStrike API
   - 不需要生产级数据库

---

## 🔄 如何迁移到完整 SQL Server？

如果将来需要迁移到 `JAXBER\SQL`：

### 步骤：

1. **修改连接字符串** (在 `appsettings.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=JAXBER\\SQL;Database=AuditFlowDB;Trusted_Connection=True;"
  }
}
```

2. **创建数据库**:
```sql
CREATE DATABASE AuditFlowDB;
```

3. **运行迁移**:
```bash
dotnet ef database update
```

4. **重新启动服务**

---

## 💡 总结

- **JAXBER\SQL**: 完整版 SQL Server，适合生产环境
- **(localdb)\mssqllocaldb**: 轻量级 LocalDB，适合开发测试

**当前项目使用 LocalDB 是因为**:
- 这是开发/原型阶段
- 不需要生产级数据库
- LocalDB 更简单、更轻量

**将来如果需要**:
- 可以轻松迁移到完整 SQL Server
- 只需修改连接字符串即可

---

**最后更新**: 2026-01-20
