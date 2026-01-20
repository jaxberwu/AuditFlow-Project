namespace AuditFlow.Shared.Entities;

public class CVEThreat
{
    public int Id { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public string CVE_ID { get; set; } = string.Empty;
    public string Severity { get; set; } = "Medium"; // Critical, High, Medium, Low - 严重程度：严重、高、中、低
    public string Remediation { get; set; } = string.Empty;
    public DateTime DetectedDate { get; set; }
}
