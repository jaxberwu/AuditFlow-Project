namespace AuditFlow.Shared.Entities;

public class ThreatAlert
{
    public int Id { get; set; }
    public string SN { get; set; } = string.Empty;
    public string Severity { get; set; } = "Critical";
}
