using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing reseller companies
/// </summary>
public class ResellerCompanyService : IResellerCompanyService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<ResellerCompanyService>();

    public ResellerCompanyService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all reseller companies
    /// </summary>
    /// <returns>Collection of reseller company DTOs</returns>
    public async Task<IEnumerable<ResellerCompanyDto>> GetAllResellerCompaniesAsync()
    {
        try
        {
            _log.Information("Fetching all reseller companies");
            
            var companies = await _context.ResellerCompanies
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();

            var companyDtos = companies.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} reseller companies", companies.Count);
            return companyDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all reseller companies");
            throw;
        }
    }

    /// <summary>
    /// Retrieves active reseller companies only
    /// </summary>
    /// <returns>Collection of active reseller company DTOs</returns>
    public async Task<IEnumerable<ResellerCompanyDto>> GetActiveResellerCompaniesAsync()
    {
        try
        {
            _log.Information("Fetching active reseller companies");
            
            var companies = await _context.ResellerCompanies
                .AsNoTracking()
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();

            var companyDtos = companies.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} active reseller companies", companies.Count);
            return companyDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active reseller companies");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a reseller company by its ID
    /// </summary>
    /// <param name="id">The ID of the reseller company</param>
    /// <returns>The reseller company DTO, or null if not found</returns>
    public async Task<ResellerCompanyDto?> GetResellerCompanyByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching reseller company with ID: {ResellerCompanyId}", id);
            
            var company = await _context.ResellerCompanies
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (company == null)
            {
                _log.Warning("Reseller company with ID: {ResellerCompanyId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched reseller company with ID: {ResellerCompanyId}", id);
            return MapToDto(company);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching reseller company with ID: {ResellerCompanyId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new reseller company
    /// </summary>
    /// <param name="dto">The reseller company data to create</param>
    /// <returns>The created reseller company DTO</returns>
    public async Task<ResellerCompanyDto> CreateResellerCompanyAsync(CreateResellerCompanyDto dto)
    {
        try
        {
            _log.Information("Creating new reseller company with name: {CompanyName}", dto.Name);

            var company = new ResellerCompany
            {
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                CountryCode = dto.CountryCode,
                CompanyRegistrationNumber = dto.CompanyRegistrationNumber,
                TaxId = dto.TaxId,
                VatNumber = dto.VatNumber,
                IsActive = dto.IsActive,
                Notes = dto.Notes,
                DefaultCurrency = dto.DefaultCurrency,
                SupportedCurrencies = dto.SupportedCurrencies,
                ApplyCurrencyMarkup = dto.ApplyCurrencyMarkup,
                DefaultCurrencyMarkup = dto.DefaultCurrencyMarkup
            };

            _context.ResellerCompanies.Add(company);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created reseller company with ID: {ResellerCompanyId}", company.Id);
            return MapToDto(company);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating reseller company with name: {CompanyName}", dto.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing reseller company
    /// </summary>
    /// <param name="id">The ID of the reseller company to update</param>
    /// <param name="dto">The updated reseller company data</param>
    /// <returns>The updated reseller company DTO, or null if not found</returns>
    public async Task<ResellerCompanyDto?> UpdateResellerCompanyAsync(int id, UpdateResellerCompanyDto dto)
    {
        try
        {
            _log.Information("Updating reseller company with ID: {ResellerCompanyId}", id);
            
            var company = await _context.ResellerCompanies.FindAsync(id);
            
            if (company == null)
            {
                _log.Warning("Reseller company with ID: {ResellerCompanyId} not found for update", id);
                return null;
            }

            company.Name = dto.Name;
            company.ContactPerson = dto.ContactPerson;
            company.Email = dto.Email;
            company.Phone = dto.Phone;
            company.Address = dto.Address;
            company.City = dto.City;
            company.State = dto.State;
            company.PostalCode = dto.PostalCode;
            company.CountryCode = dto.CountryCode;
            company.CompanyRegistrationNumber = dto.CompanyRegistrationNumber;
            company.TaxId = dto.TaxId;
            company.VatNumber = dto.VatNumber;
            company.IsActive = dto.IsActive;
            company.Notes = dto.Notes;
            company.DefaultCurrency = dto.DefaultCurrency;
            company.SupportedCurrencies = dto.SupportedCurrencies;
            company.ApplyCurrencyMarkup = dto.ApplyCurrencyMarkup;
            company.DefaultCurrencyMarkup = dto.DefaultCurrencyMarkup;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated reseller company with ID: {ResellerCompanyId}", id);
            return MapToDto(company);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating reseller company with ID: {ResellerCompanyId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a reseller company
    /// </summary>
    /// <param name="id">The ID of the reseller company to delete</param>
    /// <returns>True if deleted successfully, false if not found</returns>
    public async Task<bool> DeleteResellerCompanyAsync(int id)
    {
        try
        {
            _log.Information("Deleting reseller company with ID: {ResellerCompanyId}", id);
            
            var company = await _context.ResellerCompanies.FindAsync(id);
            
            if (company == null)
            {
                _log.Warning("Reseller company with ID: {ResellerCompanyId} not found for deletion", id);
                return false;
            }

            _context.ResellerCompanies.Remove(company);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted reseller company with ID: {ResellerCompanyId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting reseller company with ID: {ResellerCompanyId}", id);
            throw;
        }
    }

    private static ResellerCompanyDto MapToDto(ResellerCompany company)
    {
        return new ResellerCompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            ContactPerson = company.ContactPerson,
            Email = company.Email,
            Phone = company.Phone,
            Address = company.Address,
            City = company.City,
            State = company.State,
            PostalCode = company.PostalCode,
            CountryCode = company.CountryCode,
            CompanyRegistrationNumber = company.CompanyRegistrationNumber,
            TaxId = company.TaxId,
            VatNumber = company.VatNumber,
            IsActive = company.IsActive,
            Notes = company.Notes,
            DefaultCurrency = company.DefaultCurrency,
            SupportedCurrencies = company.SupportedCurrencies,
            ApplyCurrencyMarkup = company.ApplyCurrencyMarkup,
            DefaultCurrencyMarkup = company.DefaultCurrencyMarkup,
            CreatedAt = company.CreatedAt,
            UpdatedAt = company.UpdatedAt
        };
    }
}
