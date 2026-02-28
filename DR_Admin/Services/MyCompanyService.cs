using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing the reseller's own company profile.
/// </summary>
public class MyCompanyService : IMyCompanyService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<MyCompanyService>();

    public MyCompanyService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves the current company profile.
    /// </summary>
    /// <returns>The company profile if present, otherwise null.</returns>
    public async Task<MyCompanyDto?> GetMyCompanyAsync()
    {
        try
        {
            _log.Information("Fetching MyCompany profile");

            var entity = await _context.MyCompanies
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                _log.Information("MyCompany profile not found");
                return null;
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching MyCompany profile");
            throw;
        }
    }

    /// <summary>
    /// Creates or updates the company profile.
    /// </summary>
    /// <param name="dto">The company profile data.</param>
    /// <returns>The updated company profile.</returns>
    public async Task<MyCompanyDto> UpsertMyCompanyAsync(UpsertMyCompanyDto dto)
    {
        try
        {
            _log.Information("Upserting MyCompany profile");

            var entity = await _context.MyCompanies
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                entity = new MyCompany();
                _context.MyCompanies.Add(entity);
            }

            entity.Name = dto.Name;
            entity.LegalName = dto.LegalName;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.AddressLine1 = dto.AddressLine1;
            entity.AddressLine2 = dto.AddressLine2;
            entity.PostalCode = dto.PostalCode;
            entity.City = dto.City;
            entity.State = dto.State;
            entity.CountryCode = dto.CountryCode;
            entity.OrganizationNumber = dto.OrganizationNumber;
            entity.TaxId = dto.TaxId;
            entity.VatNumber = dto.VatNumber;
            entity.InvoiceEmail = dto.InvoiceEmail;
            entity.Website = dto.Website;
            entity.LogoUrl = dto.LogoUrl;
            entity.LetterheadFooter = dto.LetterheadFooter;

            await _context.SaveChangesAsync();

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while upserting MyCompany profile");
            throw;
        }
    }

    private static MyCompanyDto MapToDto(MyCompany entity)
    {
        return new MyCompanyDto
        {
            Id = entity.Id,
            Name = entity.Name,
            LegalName = entity.LegalName,
            Email = entity.Email,
            Phone = entity.Phone,
            AddressLine1 = entity.AddressLine1,
            AddressLine2 = entity.AddressLine2,
            PostalCode = entity.PostalCode,
            City = entity.City,
            State = entity.State,
            CountryCode = entity.CountryCode,
            OrganizationNumber = entity.OrganizationNumber,
            TaxId = entity.TaxId,
            VatNumber = entity.VatNumber,
            InvoiceEmail = entity.InvoiceEmail,
            Website = entity.Website,
            LogoUrl = entity.LogoUrl,
            LetterheadFooter = entity.LetterheadFooter,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}