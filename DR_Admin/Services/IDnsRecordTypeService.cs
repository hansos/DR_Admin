using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IDnsRecordTypeService
{
    Task<IEnumerable<DnsRecordTypeDto>> GetAllDnsRecordTypesAsync();
    Task<IEnumerable<DnsRecordTypeDto>> GetActiveDnsRecordTypesAsync();
    Task<DnsRecordTypeDto?> GetDnsRecordTypeByIdAsync(int id);
    Task<DnsRecordTypeDto?> GetDnsRecordTypeByTypeAsync(string type);
    Task<DnsRecordTypeDto> CreateDnsRecordTypeAsync(CreateDnsRecordTypeDto createDto);
    Task<DnsRecordTypeDto?> UpdateDnsRecordTypeAsync(int id, UpdateDnsRecordTypeDto updateDto);
    Task<bool> DeleteDnsRecordTypeAsync(int id);
}
