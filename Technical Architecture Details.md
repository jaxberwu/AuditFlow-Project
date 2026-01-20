# AuditFlow System - Technical Architecture Details

## 📋 Overview

This document provides a comprehensive technical description of the AuditFlow enterprise audit system architecture, data flow, and component interactions.

---

## 🏗️ System Architecture

### Component Overview

The system consists of **three main components**:

1. **CS_Simulator** - CrowdStrike Threat Data Simulator
2. **AuditFlow.Engine** - Audit Engine & Compliance Analyzer
3. **AuditFlow.UI** - React-based Web Dashboard

---

## 🔧 Component 1: CS_Simulator (Threat Data Provider)

### Purpose
Simulates CrowdStrike API functionality by providing threat/vulnerability data. In production, this would be replaced with actual CrowdStrike API calls.

### Technical Stack
- **Framework**: .NET 9 Minimal API
- **Port**: 5001
- **Database**: `CS_SimulatorDB` (SQL Server LocalDB)
- **DbContext**: `CS_SimulatorDbContext`

### Database Schema

**Table: `CVEThreats`**
```
┌──────┬───────────┬──────────┬────────────┬─────────────┬──────────────┐
│ Id   │ Hostname  │ CVE_ID   │ Severity   │ Remediation │ DetectedDate │
├──────┼───────────┼──────────┼────────────┼─────────────┼──────────────┤
│ 1    │ DX35GB8   │ CVE-2024-│ Critical   │ Apply patch │ 2024-01-15   │
│      │           │ 12345    │            │             │              │
└──────┴───────────┴──────────┴────────────┴─────────────┴──────────────┘
```

### Data Population (Random Seed Data)
- **On First Run**: Automatically generates **50-100 random CVE threat records**
- **Hostname Format**: `D` + 5-8 alphanumeric characters (e.g., `DX35GB8`)
- **CVE IDs**: Random CVE-2021/2022/2023/2024 prefixes with random numbers
- **Severity Levels**: Critical, High, Medium, Low (randomly assigned)
- **Unique Hostnames**: Generates 10-15 unique hostnames, each with multiple threats

### API Endpoints

#### 1. `GET /api/v1/threats/summary`
- **Purpose**: Returns threat summary grouped by hostname
- **Response Format**:
```json
{
  "hostThreatCounts": [
    {
      "hostname": "DX35GB8",
      "threatCount": 5,
      "criticalCount": 2,
      "highCount": 1,
      "mediumCount": 1,
      "lowCount": 1
    }
  ],
  "totalThreats": 84,
  "totalHosts": 12
}
```

#### 2. `GET /api/v1/threats/details/{hostname}`
- **Purpose**: Returns detailed CVE list for a specific hostname
- **Response Format**:
```json
{
  "hostname": "DX35GB8",
  "cves": [
    {
      "cve_ID": "CVE-2024-12345",
      "severity": "Critical",
      "remediation": "Apply security patch immediately",
      "detectedDate": "2024-01-15T00:00:00Z"
    }
  ],
  "totalCount": 5
}
```

---

## 🔧 Component 2: AuditFlow.Engine (Audit Engine)

### Purpose
Performs compliance auditing by correlating threat data from CS_Simulator with local hardware asset data, and applies business rules to determine compliance status.

### Technical Stack
- **Framework**: .NET 9 Minimal API
- **Port**: 5002
- **Database**: `AuditFlowDB` (SQL Server LocalDB)
- **DbContext**: `AuditFlowDbContext`

### Database Schema

**Table: `HardwareAssets`**
```
┌───────────┬──────────────┬──────────────┬──────────┐
│ SN        │ Hostname     │ PurchaseDate │ Status   │
├───────────┼──────────────┼──────────────┼──────────┤
│ X35GB8    │ DX35GB8      │ 2018-06-15   │ Active   │
│ WKG9FK    │ DWKG9FK      │ 2024-03-20   │ Active   │
└───────────┴──────────────┴──────────────┴──────────┘
```

**Field Relationships**:
- `SN`: Serial number part (e.g., `X35GB8` - without "D" prefix)
- `Hostname`: Full hostname (e.g., `DX35GB8` - "D" + SerialNumber)
- **Matching Key**: `Hostname` is used to match with CS_Simulator data

### Data Population (Random Seed Data)
- **On First Run**: Automatically generates **5-10 hardware asset records**
- **Hostname Matching Strategy**:
  1. First attempts to fetch hostnames from CS_Simulator API (`/api/v1/threats/summary`)
  2. Extracts serial numbers from Simulator hostnames (removes "D" prefix)
  3. Creates HardwareAsset records with matching hostnames
  4. If Simulator is unavailable, generates random hostnames (will match on next audit)

- **Device Age Distribution**:
  - ~50% devices from **2018** (old devices, 6+ years old)
  - ~50% devices from **2024** (new devices, 0-1 years old)

### API Endpoints

#### `GET /api/audit/summary`
- **Purpose**: Performs compliance audit and returns audit summary

**Processing Logic**:

1. **Step 1: Fetch Threat Data from CS_Simulator**
   ```csharp
   // HTTP GET request to CS_Simulator API
   GET http://localhost:5001/api/v1/threats/summary
   
   // Response cached in memory for 5 seconds to reduce API calls
   var threatSummaryCache = ConcurrentDictionary<string, (data, expires)>
   ```

2. **Step 2: Retrieve Hardware Assets from Local Database**
   ```csharp
   // Query AuditFlowDB
   var assets = await db.HardwareAssets.ToListAsync()
   ```

3. **Step 3: Match Hostnames**
   ```csharp
   // Create lookup set of hostnames with threats
   var hostnamesWithThreats = threatSummary.HostThreatCounts
       .Where(h => h.ThreatCount > 0)
       .Select(h => h.Hostname)
       .ToHashSet()
   
   // Match logic: Asset.Hostname == Threat.Hostname
   var hasThreat = hostnamesWithThreats.Contains(asset.Hostname)
   ```

4. **Step 4: Apply Compliance Rules**
   ```
   Rule: Device is Non-Compliant (requires replacement) when:
   - Device age >= 5 years (Current Year - Purchase Year >= 5)
   AND
   - Device has active threats (ThreatCount > 0)
   
   Status Determination:
   - Non-Compliant + Replace: age >= 5 AND hasThreat
   - Compliant + Monitor: (hasThreat AND age < 5) OR (age >= 5 AND !hasThreat)
   - Compliant + Monitor: (new device, no threats)
   ```

5. **Step 5: Return Audit Summary**
   ```json
   {
     "complianceItems": [
       {
         "sn": "X35GB8",
         "hostname": "DX35GB8",
         "purchaseDate": "2018-06-15T00:00:00Z",
         "ageYears": 6,
         "status": "Non-Compliant",
         "action": "Replace",
         "threatCount": 5,
         "reason": "Device is 6 years old and has 5 threat(s)"
       }
     ],
     "totalAssets": 8,
     "compliantCount": 5,
     "nonCompliantCount": 3
   }
   ```

### Key Design Decisions

1. **Physical Isolation**: Two independent databases
   - `CS_SimulatorDB`: Threat data (managed by Simulator)
   - `AuditFlowDB`: Asset data (managed by Engine)
   - No direct database access between services

2. **API-Based Communication**: Engine communicates with Simulator via HTTP REST API
   - Simulates real-world scenario where CrowdStrike is an external API
   - No threat data stored in Engine database (only cached in memory for 5 seconds)

3. **Hostname Matching**: Critical for correlating threat data with assets
   - Format: `Hostname = "D" + SerialNumber` (e.g., `DX35GB8`)
   - Both services use same hostname format for matching

---

## 🔧 Component 3: AuditFlow.UI (Web Dashboard)

### Purpose
Provides a web-based user interface for viewing compliance audit results and detailed threat information.

### Technical Stack
- **Framework**: React 19
- **Build Tool**: Vite
- **Language**: TypeScript
- **Port**: 5173 (development), static files (production)

### Data Flow

```
1. Component Mount → fetchAuditSummary()
   ↓
2. HTTP GET http://localhost:5002/api/audit/summary
   ↓
3. Receive AuditSummaryDto with complianceItems[]
   ↓
4. Display in Dashboard Table
```

### User Interaction Flow

#### Viewing Compliance Status
1. User opens dashboard: `http://localhost:5173`
2. UI automatically fetches audit summary from Engine API
3. Displays:
   - Total Assets count
   - Compliant/Non-Compliant counts
   - Detailed compliance table with all devices

#### Viewing Threat Details (Drill-Down)
1. User clicks "View CVEs" button on a device row
2. UI fetches threat details: 
   ```
   GET http://localhost:5001/api/v1/threats/details/{hostname}
   ```
3. Displays modal with:
   - Full CVE list for that hostname
   - Severity levels (Critical, High, Medium, Low)
   - Remediation recommendations
   - Detection dates
   - "Source: CrowdStrike API" label

### UI Components

- **Stats Cards**: Total Assets, Non-Compliant, Compliant counts
- **Compliance Table**: 
  - Serial Number, Hostname, Purchase Date, Age
  - Threat Count (with "Source: CrowdStrike" label)
  - Status badge (Compliant/Non-Compliant)
  - Action badge (Replace/Monitor)
  - "View CVEs" button (shown when threatCount > 0)
- **Threat Details Modal**: 
  - Opens when "View CVEs" is clicked
  - Shows detailed CVE information from CS_Simulator
  - Includes "Source: CrowdStrike API" label

---

## 🔄 Complete Data Flow

### End-to-End Flow Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     CS_Simulator                            │
│  ┌─────────────────┐         ┌─────────────────┐           │
│  │ CS_SimulatorDB  │         │  Minimal API    │           │
│  │ (CVEThreats)    │────────▶│  Port: 5001     │           │
│  │                 │         │                 │           │
│  │ • Random seed   │         │ • GET /summary  │           │
│  │   50-100 threats│         │ • GET /details  │           │
│  └─────────────────┘         └─────────────────┘           │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ HTTP REST API
                              │ GET /api/v1/threats/summary
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  AuditFlow.Engine                           │
│  ┌─────────────────┐         ┌─────────────────┐           │
│  │  AuditFlowDB    │         │  Minimal API    │           │
│  │ (HardwareAssets)│         │  Port: 5002     │           │
│  │                 │         │                 │           │
│  │ • Random seed   │◀───────▶│ • GET /audit/   │           │
│  │   5-10 devices  │         │     summary     │           │
│  │                 │         │                 │           │
│  │ Matching Logic: │         │ • Fetches from  │           │
│  │ Hostname match  │         │   Simulator API │           │
│  │ • Age >= 5      │         │ • Queries local │           │
│  │ • Has threats   │         │   database      │           │
│  │ • Compliance    │         │ • Applies rules │           │
│  └─────────────────┘         └─────────────────┘           │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ HTTP REST API
                              │ GET /api/audit/summary
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    AuditFlow.UI                             │
│  ┌─────────────────┐         ┌─────────────────┐           │
│  │  React 19 App   │         │  User Browser   │           │
│  │  Port: 5173     │────────▶│  localhost:5173 │           │
│  │                 │         │                 │           │
│  │ • Fetches from  │         │ • Dashboard     │           │
│  │   Engine API    │         │ • Threat Details│           │
│  │ • Fetches from  │         │ • Drill-down    │           │
│  │   Simulator API │         │   (View CVEs)   │           │
│  │   (for details) │         │                 │           │
│  └─────────────────┘         └─────────────────┘           │
└─────────────────────────────────────────────────────────────┘
```

### Step-by-Step Data Flow

#### Scenario: User Views Compliance Dashboard

1. **User Action**: Opens `http://localhost:5173`
   - React app loads and mounts `AuditDashboardContent` component

2. **UI → Engine API Call**:
   ```typescript
   fetch('http://localhost:5002/api/audit/summary')
   ```

3. **Engine Processing**:
   - **Step 3.1**: Fetches threat summary from CS_Simulator
     ```
     GET http://localhost:5001/api/v1/threats/summary
     ```
   - **Step 3.2**: Queries local database for hardware assets
     ```sql
     SELECT * FROM HardwareAssets
     ```
   - **Step 3.3**: Matches hostnames
     - Engine: `DX35GB8` (from HardwareAssets table)
     - Simulator: `DX35GB8` (from threat summary)
     - Match found → threatCount = 5
   - **Step 3.4**: Calculates compliance
     - Age: 6 years (2024 - 2018)
     - ThreatCount: 5
     - Result: Non-Compliant (age >= 5 AND hasThreat)

4. **Engine → UI Response**:
   ```json
   {
     "complianceItems": [...],
     "totalAssets": 8,
     "compliantCount": 5,
     "nonCompliantCount": 3
   }
   ```

5. **UI Rendering**:
   - Displays stats cards (Total: 8, Compliant: 5, Non-Compliant: 3)
   - Renders compliance table with all devices
   - Shows threat counts with "Source: CrowdStrike" labels

#### Scenario: User Clicks "View CVEs"

1. **User Action**: Clicks "View CVEs" button for device `DX35GB8`

2. **UI → Simulator API Call**:
   ```typescript
   fetch('http://localhost:5001/api/v1/threats/details/DX35GB8')
   ```

3. **Simulator Processing**:
   - Queries `CS_SimulatorDB` for all CVEThreats where Hostname = 'DX35GB8'
   - Returns detailed CVE list

4. **Simulator → UI Response**:
   ```json
   {
     "hostname": "DX35GB8",
     "cves": [
       { "cve_ID": "CVE-2024-12345", "severity": "Critical", ... },
       { "cve_ID": "CVE-2024-67890", "severity": "High", ... }
     ],
     "totalCount": 5
   }
   ```

5. **UI Rendering**:
   - Opens modal with threat details
   - Displays CVE table with severity badges
   - Shows "Source: CrowdStrike API" label

---

## 🎯 Key Technical Design Principles

### 1. **Physical Isolation**
- Two completely independent databases
- Separate DbContext classes (`CS_SimulatorDbContext` and `AuditFlowDbContext`)
- No direct database cross-access
- Simulates real-world microservices architecture

### 2. **API-Based Communication**
- Services communicate via HTTP REST APIs
- No shared database or direct database connections
- Engine acts as API consumer, Simulator acts as API provider
- Models real CrowdStrike integration scenario

### 3. **Hostname as Matching Key**
- **Critical Requirement**: Hostname must match exactly between services
- Format: `D` + SerialNumber (e.g., `DX35GB8`)
- Engine database stores:
  - `SN`: Serial number part (`X35GB8`)
  - `Hostname`: Full hostname (`DX35GB8`)
- Simulator database stores:
  - `Hostname`: Full hostname (`DX35GB8`)
- Matching logic: `asset.Hostname == threat.Hostname`

### 4. **In-Memory Caching**
- Engine caches Simulator API responses for 5 seconds
- Reduces API call frequency
- Uses `ConcurrentDictionary` for thread-safe caching
- Trade-off: Threat data is never persisted in Engine database (ensures real-time data)

### 5. **Random Seed Data**
- **CS_Simulator**: 50-100 random CVE threats, 10-15 unique hostnames
- **AuditFlow**: 5-10 hardware assets with matching hostnames (when Simulator available)
- Ensures realistic test data for demonstration
- Hostnames are synchronized between services during seed

---

## 📊 Database Relationships

### CS_SimulatorDB Structure
```
CS_SimulatorDB
└── CVEThreats
    ├── Id (PK, auto-increment)
    ├── Hostname (indexed, for matching)
    ├── CVE_ID
    ├── Severity (Critical/High/Medium/Low)
    ├── Remediation
    └── DetectedDate
```

### AuditFlowDB Structure
```
AuditFlowDB
└── HardwareAssets
    ├── SN (PK, serial number part)
    ├── Hostname (indexed, for matching)
    ├── PurchaseDate
    └── Status
```

### Matching Relationship
```
CS_SimulatorDB.CVEThreats.Hostname 
    == 
AuditFlowDB.HardwareAssets.Hostname

Example:
  Simulator: Hostname = "DX35GB8", ThreatCount = 5
  Engine:    Hostname = "DX35GB8", SN = "X35GB8", PurchaseDate = 2018-06-15
  Match: ✅ Device has 5 threats, age = 6 years → Non-Compliant
```

---

## 🔐 API Security & Configuration

### CORS Configuration
- Both APIs configured to allow requests from `http://localhost:5173`
- Enables frontend to call backend APIs

### JSON Serialization
- CamelCase naming policy for API responses
- Consistent with JavaScript/TypeScript conventions

### Logging
- Set to `Warning` level to reduce console output
- Focuses on important warnings and errors

---

## 🚀 Deployment Architecture

### Development Environment
- All services run on localhost
- CS_Simulator: `http://localhost:5001`
- AuditFlow.Engine: `http://localhost:5002`
- AuditFlow.UI: `http://localhost:5173` (Vite dev server)

### Production Environment
- **Backend**: Kestrel web server (built into .NET)
- **Frontend**: Static files (built with `npm run build`)
  - Can be deployed to IIS, Nginx, or any static file server
  - No Node.js required in production (only for development/build)

---

## ✅ Summary: Your Understanding is Correct!

Your description is **accurate**. Here's the confirmation:

✅ **CS_Simulator + Database** with random threat data  
✅ **AuditFlow Engine + Database** with random asset data  
✅ **Both communicate via API** (REST HTTP calls)  
✅ **AuditFlow calls CS_Simulator** to get threat information  
✅ **Matches with local database** device information using Hostname  
✅ **Displays results on UI**  

### Additional Details to Complete the Picture:

1. **Matching Mechanism**: Uses `Hostname` field as the key (must match exactly)
2. **Compliance Rules**: Age >= 5 years AND hasThreats = Non-Compliant
3. **Data Flow**: UI → Engine API → (Engine queries local DB + calls Simulator API) → Match → Calculate → Return → Display
4. **Caching**: Engine caches Simulator responses (5 seconds) but doesn't persist threat data
5. **Physical Isolation**: Two completely separate databases for true microservices architecture

---

**This architecture effectively demonstrates a real-world scenario where an audit system integrates with an external threat intelligence provider (CrowdStrike) via API, correlates the data with internal asset information, and presents actionable compliance insights through a web dashboard.**
