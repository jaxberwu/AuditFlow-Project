namespace AuditFlow.Shared.Entities;

public class HardwareAsset
{
    public string SN { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
