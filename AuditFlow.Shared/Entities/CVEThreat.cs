namespace AuditFlow.Shared.Entities;

public class CVEThreat
{
    public int Id { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public string CVE_ID { get; set; } = string.Empty;
    public string Severity { get; set; } = "Medium"; // Critical, High, Medium, Low
    public string Remediation { get; set; } = string.Empty;
    public DateTime DetectedDate { get; set; }
}
