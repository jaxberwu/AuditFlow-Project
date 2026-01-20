# UI 界面数据来源说明

## 📊 数据来源架构

### 前端 UI 展示的数据来源

**UI 界面显示的是多个数据源整合后的结果**，不是单一数据库表。

---

## 🔄 数据流程

### 主仪表板 (Dashboard)

**API 调用**: `GET http://localhost:5002/api/audit/summary`

**数据整合过程**:

```
前端 UI
  ↓ (1个 API 调用)
Engine API (/api/audit/summary)
  ↓ (内部整合多个数据源)
┌─────────────────────────────────────┐
│ 数据源 1: Simulator API             │
│ - 调用 HTTP API                     │
│ - 获取威胁数据                      │
│ - 来源: CS_SimulatorDB 数据库       │
└─────────────────────────────────────┘
           +
┌─────────────────────────────────────┐
│ 数据源 2: Engine 本地数据库         │
│ - AuditFlowDB 数据库                │
│ - HardwareAssets 表                 │
│ - 硬件资产数据                      │
└─────────────────────────────────────┘
           ↓
    数据交叉引用和审计逻辑
           ↓
    整合后的合规报告
           ↓
    返回给前端
```

### CVE 详情弹窗

**API 调用**: `GET http://localhost:5001/api/v1/threats/details/{hostname}`

**数据来源**:
- **直接调用**: Simulator API
- **数据来源**: CS_SimulatorDB 数据库的 CVEThreats 表
- **无需整合**: 直接返回该主机的所有 CVE 威胁

---

## 📋 具体数据来源

### 1. 主仪表板数据（整合数据）

**API**: Engine `/api/audit/summary`

**整合的数据来源**:

#### 数据源 A: 威胁数据（来自 Simulator）

**来源**: 
- **数据库**: `CS_SimulatorDB`
- **表**: `CVEThreats`
- **获取方式**: Engine 通过 HTTP API 调用 Simulator

**包含信息**:
- 主机名 (Hostname)
- 威胁数量 (ThreatCount)
- 严重程度分布 (Critical/High/Medium/Low)

#### 数据源 B: 硬件资产数据（来自 Engine）

**来源**:
- **数据库**: `AuditFlowDB`
- **表**: `HardwareAssets`
- **获取方式**: Engine 直接查询本地数据库

**包含信息**:
- 序列号 (SN)
- 主机名 (Hostname)
- 购买日期 (PurchaseDate)
- 状态 (Status)

#### 整合逻辑

Engine 在 `/api/audit/summary` 端点中：

1. **获取威胁数据**: 调用 Simulator API (`/api/v1/threats/summary`)
2. **获取资产数据**: 查询本地数据库 (`HardwareAssets` 表)
3. **数据交叉引用**: 通过 Hostname 关联威胁和资产
4. **审计计算**: 
   - 计算设备年龄（当前年份 - 购买年份）
   - 判断是否有威胁
   - 生成合规状态（Compliant/Non-Compliant）
5. **返回整合数据**: 包含所有信息的 `AuditSummaryDto`

**前端收到的数据结构**:
```json
{
  "complianceItems": [
    {
      "sn": "X35GB8",                    // 来自 HardwareAssets 表
      "hostname": "DX35GB8",             // 来自 HardwareAssets 表
      "purchaseDate": "2018-01-15",      // 来自 HardwareAssets 表
      "ageYears": 6,                     // 计算得出
      "threatCount": 5,                  // 来自 Simulator API
      "status": "Non-Compliant",         // 计算得出
      "action": "Replace",               // 计算得出
      "reason": "..."                    // 计算得出
    }
  ],
  "totalAssets": 6,                      // 来自 HardwareAssets 表
  "compliantCount": 3,                   // 计算得出
  "nonCompliantCount": 3                 // 计算得出
}
```

### 2. CVE 详情数据（单一数据源）

**API**: Simulator `/api/v1/threats/details/{hostname}`

**数据来源**:
- **数据库**: `CS_SimulatorDB`
- **表**: `CVEThreats`
- **过滤条件**: Hostname = {hostname}

**前端收到的数据结构**:
```json
[
  {
    "id": 1,
    "hostname": "DX35GB8",               // 来自 CVEThreats 表
    "cve_ID": "CVE-2024-12345",          // 来自 CVEThreats 表
    "severity": "Critical",              // 来自 CVEThreats 表
    "remediation": "...",                // 来自 CVEThreats 表
    "detectedDate": "2024-01-15"         // 来自 CVEThreats 表
  }
]
```

---

## 🎯 总结

### UI 主界面展示

**数据来源**: **多个数据源整合** ✅

- ✅ 硬件资产数据（Engine 本地数据库）
- ✅ 威胁数据（Simulator API）
- ✅ 审计计算结果（Engine 逻辑）

**API 调用**: 1 个（Engine 的整合端点）

### CVE 详情弹窗

**数据来源**: **单一数据源** ✅

- ✅ CVE 威胁数据（Simulator API）

**API 调用**: 1 个（Simulator 的详情端点）

---

## 💡 优势

### 1. 数据整合在后端完成

**好处**:
- 前端只需要调用 1 个 API 即可获得完整数据
- 减少前端复杂度
- 降低网络请求次数
- 数据一致性更好

### 2. 符合微服务架构

**好处**:
- 每个服务职责清晰
- Simulator 只负责威胁数据
- Engine 负责业务逻辑和整合
- 易于维护和扩展

### 3. 演示效果清晰

**好处**:
- 可以清楚看到数据整合过程
- 可以演示跨服务数据关联
- 符合真实企业架构模式

---

**最后更新**: 2026-01-20
