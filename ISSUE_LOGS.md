# Issue Logs - AuditFlow System

本文档记录系统开发过程中遇到的所有问题和修复方案。

---

## Issue #001 - Tailwind CSS v4 PostCSS 配置错误

**发生时间**: 2026-01-19 23:37

**错误信息**:
```
[plugin:vite:css] [postcss] It looks like you're trying to use `tailwindcss` directly as a PostCSS plugin. 
The PostCSS plugin has moved to a separate package, so to continue using Tailwind CSS with PostCSS you'll 
need to install `@tailwindcss/postcss` and update your PostCSS configuration.
```

**问题描述**: 
- Tailwind CSS v4 改变了 PostCSS 插件的使用方式
- 项目使用了 Tailwind CSS v4.1.18，但 PostCSS 配置使用的是 v3 的方式

**修复方案**:
1. 将 Tailwind CSS 从 v4.1.18 降级到 v3.4.1（兼容版本）
2. 将 PostCSS 配置文件从 `postcss.config.js` 改为 `postcss.config.cjs`（CommonJS 格式）
3. 清理 node_modules 和 Vite 缓存

**修复命令**:
```bash
cd AuditFlow.UI
npm uninstall tailwindcss
npm install -D tailwindcss@3.4.1 postcss@8.4.35 autoprefixer@10.4.16
```

**状态**: ✅ 已修复

---

## Issue #002 - TypeScript 类型导入错误

**发生时间**: 2026-01-19 23:45

**错误信息**:
```
src/App.tsx(2,10): error TS1484: 'AuditSummaryDto' is a type and must be imported using a type-only import 
when 'verbatimModuleSyntax' is enabled.
```

**问题描述**: 
- TypeScript 配置启用了 `verbatimModuleSyntax`，要求类型必须使用 `type` 关键字导入

**修复方案**:
将类型导入改为 type-only import：
```typescript
// 修复前
import { AuditSummaryDto } from './types';

// 修复后
import type { AuditSummaryDto } from './types';
```

**状态**: ✅ 已修复

---

## Issue #003 - React 19 use() Hook 兼容性问题

**发生时间**: 2026-01-19 23:50

**错误信息**: 
页面显示白板，没有任何内容渲染

**问题描述**:
- 使用了 React 19 的 `use()` hook 和 `Suspense` 进行数据获取
- 可能存在兼容性问题或错误处理不当

**修复方案**:
1. 改用传统的 `useEffect` + `useState` 模式（更稳定）
2. 添加详细的错误处理和加载状态
3. 添加控制台日志用于调试

**修复代码**:
```typescript
// 修复前：使用 React 19 use() hook
const auditSummary = use(fetchAuditSummary());

// 修复后：使用 useEffect + useState
const [auditSummary, setAuditSummary] = useState<AuditSummaryDto | null>(null);
const [loading, setLoading] = useState(true);
const [error, setError] = useState<string | null>(null);

useEffect(() => {
  fetch('http://localhost:5002/audit/summary')
    .then(res => res.json())
    .then(data => {
      setAuditSummary(data);
      setLoading(false);
    })
    .catch(err => {
      setError(err.message);
      setLoading(false);
    });
}, []);
```

**状态**: ✅ 已修复

---

## Issue #004 - CSS 文件加载失败 (500 错误)

**发生时间**: 2026-01-20 00:15

**错误信息**:
```
index.css:1 Failed to load resource: the server responded with a status of 500 (Internal Server Error)
```

**问题描述**:
- `index.css` 文件包含 Tailwind CSS 指令（@tailwind base/components/utilities）
- PostCSS 处理这些指令时出错，导致 CSS 文件无法加载
- 页面显示白板

**修复方案**:
1. 简化 `index.css`，移除 Tailwind 指令
2. 暂时注释掉 `main.tsx` 中的 CSS 导入
3. 改用内联样式（inline styles）替代 Tailwind CSS 类名

**修复步骤**:
1. 修改 `src/index.css`:
```css
/* 移除 Tailwind 指令，使用基础样式 */
body {
  margin: 0;
  font-family: system-ui, -apple-system, sans-serif;
}
```

2. 修改 `src/main.tsx`:
```typescript
// import './index.css' // 暂时注释掉
```

3. 所有组件改用内联样式（inline styles）

**状态**: ✅ 已修复

**备注**: 
- 后续可以考虑修复 Tailwind CSS 配置，或继续使用内联样式
- 内联样式的优势：无需构建步骤，更简单直接

---

## Issue #005 - 前端页面白板问题

**发生时间**: 2026-01-20 00:20

**错误信息**: 
页面完全空白，没有任何内容

**问题描述**:
- 多个问题叠加导致页面无法渲染：
  1. CSS 文件加载失败（Issue #004）
  2. React 组件可能没有正确挂载
  3. 需要确认 JavaScript 是否正常执行

**修复方案**:
1. 创建最简单的测试组件验证 React 是否工作
2. 添加详细的控制台日志
3. 逐步恢复功能

**测试步骤**:
1. 创建最简单的 React 组件显示 "React is Working!"
2. 测试 API 连接
3. 确认 React 和 API 都正常后，恢复完整界面

**状态**: ✅ 已修复

**验证结果**:
- React 正常工作 ✓
- API 连接成功 ✓
- 完整仪表板界面已恢复 ✓

---

## 总结

### 主要问题类别：
1. **依赖版本兼容性** - Tailwind CSS v4 vs v3
2. **TypeScript 配置** - 类型导入语法
3. **React API 使用** - use() hook 兼容性
4. **构建工具配置** - PostCSS 处理 CSS

### 最佳实践：
1. ✅ 使用稳定的依赖版本（避免使用最新版本，除非必要）
2. ✅ 使用成熟的 React 模式（useEffect + useState）
3. ✅ 添加详细的错误处理和日志
4. ✅ 使用内联样式作为备选方案（避免构建工具问题）

### 待优化项：
- [ ] 修复 Tailwind CSS PostCSS 配置（如果需要使用 Tailwind）
- [ ] 考虑使用 CSS Modules 或 styled-components
- [ ] 添加错误边界（Error Boundary）组件

---

---

## Issue #006 - 添加威胁数据来源标注

**发生时间**: 2026-01-20 00:45

**需求描述**: 
用户要求在 Threat Severity 列中添加数据来源标注，显示威胁数据来自 CrowdStrike API

**实现方案**:
1. 在表头添加数据来源说明：`(Source: CrowdStrike API)`
2. 在每个威胁严重性标签下方添加小字标注：`Source: CrowdStrike API`

**修改文件**:
- `AuditFlow.UI/src/App.tsx`

**修改内容**:
- 表头添加数据来源说明
- 每个威胁严重性单元格添加来源标注

**状态**: ✅ 已完成

**备注**: 
- 当前使用模拟数据，将来集成真实 CrowdStrike API 时，此标注将准确反映数据来源
- 标注使用小字体和灰色，不影响主要信息的可读性

---

---

## Issue #007 - Update Device Hostname Naming Convention

**发生时间**: 2026-01-20 01:00

**需求描述**: 
用户要求修改设备主机名命名规则：
- 格式: `D` + 5-8位随机字母数字组合
- 示例: `D7K2M9P`, `DA3B5C8X`, `DXYZ123`
- 添加约10个随机设备数据

**实现方案**:
1. 修改 `AuditFlow.Engine/Program.cs` 中的种子数据生成逻辑
2. 实现随机序列号生成函数（5-8位字母数字组合）
3. 生成10个设备，混合新旧购买日期
4. 更新威胁数据生成逻辑，随机分配威胁

**修改文件**:
- `AuditFlow.Engine/Program.cs` - 更新硬件资产种子数据
- `AuditFlow.Simulator/Program.cs` - 更新威胁数据生成逻辑

**新命名规则**:
```csharp
Hostname = "D" + RandomAlphanumeric(5-8 characters)
// Examples: D7K2M9P, DA3B5C8X, DXYZ123
```

**数据生成逻辑**:
- 10个设备（SN001-SN010）
- 前5个：旧设备（2016-2019年购买）
- 后5个：新设备（2020-2024年购买）
- 随机威胁分配（约30-40%的设备有威胁）

**状态**: ✅ 已完成

**备注**: 
- 需要重置数据库才能看到新数据
- 已创建 `重置数据库.ps1` 脚本方便重置

---

---

## Issue #008 - Enterprise AuditFlow System Refactoring

**发生时间**: 2026-01-20 01:30

**需求描述**: 
重构为完全独立的企业级审计系统，实现物理隔离和API联动

**主要变更**:

### 1. CS_Simulator (系统A - 数据提供方)
- **独立数据库**: CS_SimulatorDB
- **数据生成**: 50-100条随机CVE漏洞数据
- **新实体**: CVEThreat (CVE_ID, Severity, Remediation, DetectedDate)
- **新API端点**:
  - `GET /api/v1/threats/summary` - 返回所有主机名及其漏洞总数
  - `GET /api/v1/threats/details/{hostname}` - 返回指定主机的详细CVE列表
- **独立DbContext**: CS_SimulatorDbContext

### 2. AuditFlow_Engine (系统B - 审计引擎)
- **独立数据库**: AuditFlowDB (保持不变)
- **审计逻辑**: 
  - 通过HttpClient调用CS_Simulator API获取威胁数据
  - 规则: 设备在风险名单中 AND 年龄 >= 5年 = Non-Compliant
  - 返回Status和Action字段
- **新API端点**: `GET /api/audit/summary` (更新为新的DTO格式)
- **新DTO**: ComplianceItem (包含Status, Action, ThreatCount等)

### 3. React Frontend
- **使用React 19 use() hook**: 使用Suspense包装
- **主表显示**: 合规状态表，包含所有设备信息
- **下钻功能**: 点击设备行或"View CVEs"按钮，显示该设备的详细CVE列表
- **模态框**: ThreatDetailsPanel组件显示CVE详情

**技术实现**:
- ✅ Minimal API模式（无Controller）
- ✅ 完全隔离的数据库和DbContext
- ✅ CORS配置正确
- ✅ 使用HttpClient进行跨服务通信
- ✅ React 19 use() hook + Suspense

**修改文件**:
- `AuditFlow.Shared/Entities/CVEThreat.cs` (新建)
- `AuditFlow.Shared/Data/CS_SimulatorDbContext.cs` (新建)
- `AuditFlow.Shared/DTOs/ThreatSummaryDto.cs` (新建)
- `AuditFlow.Shared/DTOs/AuditSummaryDto.cs` (重构)
- `AuditFlow.Simulator/Program.cs` (完全重写)
- `AuditFlow.Engine/Program.cs` (完全重写)
- `AuditFlow.UI/src/App.tsx` (完全重写)
- `AuditFlow.UI/src/types.ts` (更新)

**状态**: ✅ 已完成

**备注**: 
- 两个系统完全独立，通过HTTP API通信
- 模拟真实企业级跨系统审计场景
- 将来CS_Simulator可替换为真实CrowdStrike API调用

---

**最后更新**: 2026-01-20 01:30
