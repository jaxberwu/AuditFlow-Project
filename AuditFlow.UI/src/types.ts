export interface ComplianceItem {
  sn: string;
  hostname: string;
  purchaseDate: string;
  ageYears: number;
  status: string;
  action: string;
  threatCount: number;
  reason: string;
}

export interface AuditSummaryDto {
  complianceItems: ComplianceItem[];
  totalAssets: number;
  compliantCount: number;
  nonCompliantCount: number;
}

export interface CVEThreatDetail {
  cve_ID: string;
  severity: string;
  remediation: string;
  detectedDate: string;
}

export interface ThreatDetailsDto {
  hostname: string;
  cves: CVEThreatDetail[];
  totalCount: number;
}
