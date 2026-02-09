using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing vendor tax profiles
/// </summary>
public interface IVendorTaxProfileService
{
    Task<IEnumerable<VendorTaxProfileDto>> GetAllVendorTaxProfilesAsync();
    Task<VendorTaxProfileDto?> GetVendorTaxProfileByIdAsync(int id);
    Task<VendorTaxProfileDto?> GetVendorTaxProfileByVendorIdAsync(int vendorId);
    Task<VendorTaxProfileDto> CreateVendorTaxProfileAsync(CreateVendorTaxProfileDto dto);
    Task<VendorTaxProfileDto?> UpdateVendorTaxProfileAsync(int id, UpdateVendorTaxProfileDto dto);
    Task<bool> DeleteVendorTaxProfileAsync(int id);
}
