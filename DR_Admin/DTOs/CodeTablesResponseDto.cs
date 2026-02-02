namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for code tables check and update response
/// </summary>
public class CodeTablesResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int RolesAdded { get; set; }
    public int CustomerStatusesAdded { get; set; }
    public int DnsRecordTypesAdded { get; set; }
    public int ServiceTypesAdded { get; set; }
    public int TotalRoles { get; set; }
    public int TotalCustomerStatuses { get; set; }
    public int TotalDnsRecordTypes { get; set; }
    public int TotalServiceTypes { get; set; }
}
