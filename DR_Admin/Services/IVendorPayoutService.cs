using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing vendor payouts
/// </summary>
public interface IVendorPayoutService
{
    Task<IEnumerable<VendorPayoutDto>> GetAllVendorPayoutsAsync();
    Task<PagedResult<VendorPayoutDto>> GetAllVendorPayoutsPagedAsync(PaginationParameters parameters);
    Task<VendorPayoutDto?> GetVendorPayoutByIdAsync(int id);
    Task<IEnumerable<VendorPayoutDto>> GetVendorPayoutsByVendorIdAsync(int vendorId);
    Task<VendorPayoutDto> CreateVendorPayoutAsync(CreateVendorPayoutDto dto);
    Task<VendorPayoutDto?> UpdateVendorPayoutAsync(int id, UpdateVendorPayoutDto dto);
    Task<bool> DeleteVendorPayoutAsync(int id);
    Task<VendorPayoutDto?> ProcessVendorPayoutAsync(ProcessVendorPayoutDto dto);
    Task<VendorPayoutDto?> ResolvePayoutInterventionAsync(ResolvePayoutInterventionDto dto);
    Task<VendorPayoutSummaryDto?> GetVendorPayoutSummaryByVendorIdAsync(int vendorId);
}
