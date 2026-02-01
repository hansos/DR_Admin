using DomainRegistrationLib.Models;
using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

public interface IRegistrarService
{
    Task<IEnumerable<RegistrarDto>> GetAllRegistrarsAsync();
    Task<IEnumerable<RegistrarDto>> GetActiveRegistrarsAsync();
    Task<RegistrarDto?> GetRegistrarByIdAsync(int id);
    Task<RegistrarDto?> GetRegistrarByCodeAsync(string code);
    Task<RegistrarDto> CreateRegistrarAsync(CreateRegistrarDto createDto);
    Task<RegistrarDto?> UpdateRegistrarAsync(int id, UpdateRegistrarDto updateDto);
    Task<bool> DeleteRegistrarAsync(int id);
    Task<RegistrarTldDto> AssignTldToRegistrarAsync(int registrarId, int tldId);
    Task<RegistrarTldDto> AssignTldToRegistrarAsync(CreateRegistrarTldDto createDto);
    Task<RegistrarTldDto> AssignTldToRegistrarAsync(int registrarId, TldDto tldDto);
    Task<IEnumerable<TldDto>> GetTldsByRegistrarAsync(int registrarId);
    Task<int> DownloadTldsForRegistrarAsync(int registrarId);
    Task<DomainAvailabilityResult> CheckDomainAvailabilityAsync(int registrarId, string domainName);
}
