namespace AuditFlow.Shared.DTOs;

public class ThreatSummaryDto
{
    public List<HostThreatCount> HostThreatCounts { get; set; } = new();
    public int TotalThreats { get; set; }
    public int TotalHosts { get; set; }
}

public class HostThreatCount
{
    public string Hostname { get; set; } = string.Empty;
    public int ThreatCount { get; set; }
    public int CriticalCount { get; set; }
    public int HighCount { get; set; }
    public int MediumCount { get; set; }
    public int LowCount { get; set; }
}

public class ThreatDetailsDto
{
    public string Hostname { get; set; } = string.Empty;
    public List<CVEThreatDetail> CVEs { get; set; } = new();
    public int TotalCount { get; set; }
}

public class CVEThreatDetail
{
    public string CVE_ID { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Remediation { get; set; } = string.Empty;
    public DateTime DetectedDate { get; set; }
}
