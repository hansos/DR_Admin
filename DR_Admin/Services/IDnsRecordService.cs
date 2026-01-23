using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IDnsRecordService
{
    Task<IEnumerable<DnsRecordDto>> GetAllDnsRecordsAsync();
    Task<DnsRecordDto?> GetDnsRecordByIdAsync(int id);
    Task<IEnumerable<DnsRecordDto>> GetDnsRecordsByDomainIdAsync(int domainId);
    Task<IEnumerable<DnsRecordDto>> GetDnsRecordsByTypeAsync(string type);
    Task<DnsRecordDto> CreateDnsRecordAsync(CreateDnsRecordDto createDto);
    Task<DnsRecordDto?> UpdateDnsRecordAsync(int id, UpdateDnsRecordDto updateDto);
    Task<bool> DeleteDnsRecordAsync(int id);
}
