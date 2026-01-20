namespace AuditFlow.Shared.DTOs;

public class AuditSummaryDto
{
    public List<ComplianceItem> ComplianceItems { get; set; } = new();
    public int TotalAssets { get; set; }
    public int CompliantCount { get; set; }
    public int NonCompliantCount { get; set; }
}

public class ComplianceItem
{
    public string SN { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public int AgeYears { get; set; }
    public string Status { get; set; } = "Compliant"; // Compliant, Non-Compliant - 状态：合规、不合规
    public string Action { get; set; } = string.Empty; // Replace, Monitor, etc. - 操作：替换、监控等
    public int ThreatCount { get; set; }
    public string Reason { get; set; } = string.Empty;
}
