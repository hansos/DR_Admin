using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportGeneratorLib.Implementations;
using ReportGeneratorLib.Infrastructure.Enums;
using ReportGeneratorLib.Models;
using Serilog;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ISPAdmin.Controllers;

/// <summary>
/// System-level operations including data normalization and maintenance tasks
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SystemController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ISystemService _systemService;
    private readonly IConfiguration _configuration;
    private static readonly Serilog.ILogger _log = Log.ForContext<SystemController>();

    public SystemController(
        ApplicationDbContext context,
        ISystemService systemService,
        IConfiguration configuration)
    {
        _context = context;
        _systemService = systemService;
        _configuration = configuration;
    }

    /// <summary>
    /// Generates an offer PDF on the server for print verification
    /// </summary>
    /// <param name="offer">Offer document data used to generate the PDF</param>
    /// <returns>Information about the generated verification PDF file</returns>
    /// <response code="200">Returns metadata for the generated file</response>
    /// <response code="400">If the request body is missing or invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("verify-offer-print")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> VerifyOfferPrint([FromBody] OfferDocumentDto offer)
    {
        try
        {
            if (offer == null)
            {
                return BadRequest("Offer payload is required");
            }

            var quote = await UpsertQuoteFromOfferAsync(offer, markAsSent: false);
            offer.QuoteId = quote.Id;

            var (fileName, outputPath) = await GenerateOfferPdfAsync(offer, "Printed");
            var revision = await SaveQuoteRevisionAsync(quote, offer, "Printed", fileName, outputPath);

            _log.Information("API: VerifyOfferPrint generated report at {OutputPath} for user {User}", outputPath, User.Identity?.Name);
            return Ok(new
            {
                success = true,
                quoteId = quote.Id,
                status = quote.Status.ToString(),
                revisionNumber = revision.RevisionNumber,
                actionType = revision.ActionType,
                fileName,
                outputPath
            });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in VerifyOfferPrint");
            return StatusCode(500, "An error occurred while generating verification PDF");
        }
    }

    /// <summary>
    /// Retrieves dashboard sales summary with offers, orders and open invoices
    /// </summary>
    /// <returns>Latest offers, orders and open invoices</returns>
    /// <response code="200">Returns sales summary collections</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("sales-summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSalesSummary()
    {
        try
        {
            var offers = await _context.Quotes
                .AsNoTracking()
                .Where(q => q.DeletedAt == null)
                .OrderByDescending(q => q.CreatedAt)
                .Take(50)
                .Select(q => new
                {
                    id = q.Id,
                    quoteNumber = q.QuoteNumber,
                    status = q.Status.ToString(),
                    totalAmount = q.TotalAmount,
                    currencyCode = q.CurrencyCode
                })
                .ToListAsync();

            var orders = await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .Take(50)
                .Select(o => new
                {
                    id = o.Id,
                    orderNumber = o.OrderNumber,
                    status = o.Status.ToString(),
                    totalAmount = o.SetupFee + o.RecurringAmount,
                    currencyCode = o.CurrencyCode
                })
                .ToListAsync();

            var openInvoices = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.DeletedAt == null && i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled && i.Status != InvoiceStatus.Credited)
                .OrderByDescending(i => i.CreatedAt)
                .Take(50)
                .Select(i => new
                {
                    id = i.Id,
                    invoiceNumber = i.InvoiceNumber,
                    status = i.Status.ToString(),
                    totalAmount = i.AmountDue,
                    currencyCode = i.CurrencyCode
                })
                .ToListAsync();

            return Ok(new
            {
                offers,
                orders,
                openInvoices
            });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetSalesSummary");
            return StatusCode(500, "An error occurred while retrieving sales summary");
        }
    }

    /// <summary>
    /// Retrieves the latest persisted offer snapshot for a quote so it can be reopened in the offer editor
    /// </summary>
    /// <param name="quoteId">The quote ID</param>
    /// <returns>Latest persisted offer snapshot and quote metadata</returns>
    /// <response code="200">Returns the offer snapshot for editing</response>
    /// <response code="404">If quote or snapshot is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("offer-editor/{quoteId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetOfferEditorSnapshot(int quoteId)
    {
        try
        {
            var quote = await _context.Quotes
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == quoteId && q.DeletedAt == null);

            if (quote == null)
            {
                return NotFound($"Quote with ID {quoteId} was not found.");
            }

            var latestRevision = await _context.QuoteRevisions
                .AsNoTracking()
                .Where(r => r.QuoteId == quoteId)
                .OrderByDescending(r => r.RevisionNumber)
                .FirstOrDefaultAsync();

            if (latestRevision == null || string.IsNullOrWhiteSpace(latestRevision.SnapshotJson))
            {
                return NotFound($"No persisted offer snapshot was found for quote ID {quoteId}.");
            }

            var snapshot = JsonSerializer.Deserialize<OfferDocumentDto>(latestRevision.SnapshotJson);
            if (snapshot == null)
            {
                return NotFound($"Persisted snapshot for quote ID {quoteId} is invalid.");
            }

            return Ok(new
            {
                quoteId = quote.Id,
                quoteStatus = quote.Status.ToString(),
                lastAction = latestRevision.ActionType,
                lastRevisionNumber = latestRevision.RevisionNumber,
                offer = snapshot
            });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetOfferEditorSnapshot for QuoteId {QuoteId}", quoteId);
            return StatusCode(500, "An error occurred while retrieving offer snapshot");
        }
    }

    /// <summary>
    /// Persists and marks an offer as sent while generating a server-side PDF snapshot
    /// </summary>
    /// <param name="offer">Offer document data used to persist and send the offer</param>
    /// <returns>Persisted quote and revision details</returns>
    /// <response code="200">Returns persisted quote and revision metadata</response>
    /// <response code="400">If the request body is missing or invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("send-offer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendOffer([FromBody] OfferDocumentDto offer)
    {
        try
        {
            if (offer == null)
            {
                return BadRequest("Offer payload is required");
            }

            var quote = await UpsertQuoteFromOfferAsync(offer, markAsSent: true);
            offer.QuoteId = quote.Id;

            var (fileName, outputPath) = await GenerateOfferPdfAsync(offer, "Sent");
            var revision = await SaveQuoteRevisionAsync(quote, offer, "Sent", fileName, outputPath);

            _log.Information("API: SendOffer persisted quote {QuoteId}, revision {RevisionNumber} and PDF {OutputPath}", quote.Id, revision.RevisionNumber, outputPath);

            return Ok(new
            {
                success = true,
                quoteId = quote.Id,
                status = quote.Status.ToString(),
                sentAt = quote.SentAt,
                revisionNumber = revision.RevisionNumber,
                actionType = revision.ActionType,
                fileName,
                outputPath
            });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SendOffer");
            return StatusCode(500, "An error occurred while sending and persisting offer");
        }
    }

    /// <summary>
    /// Normalizes all records in the database by updating normalized fields for exact searches
    /// </summary>
    /// <remarks>
    /// This endpoint updates all normalized name fields across all entities:
    /// - Country: NormalizedEnglishName, NormalizedLocalName
    /// - Coupon: NormalizedName
    /// - Customer: NormalizedName, NormalizedCompanyName, NormalizedContactPerson
    /// - Domain: NormalizedName
    /// - HostingPackage: NormalizedName
    /// - PaymentGateway: NormalizedName
    /// - PostalCode: NormalizedCode, NormalizedCountryCode, NormalizedCity, NormalizedState, NormalizedRegion, NormalizedDistrict
    /// - Registrar: NormalizedName
    /// - SalesAgent: NormalizedFirstName, NormalizedLastName
    /// - User: NormalizedUsername
    /// 
    /// This operation should be run:
    /// - After upgrading from a version without normalized fields
    /// - To fix any data inconsistencies
    /// - As part of data maintenance procedures
    /// </remarks>
    /// <returns>Summary of normalization results including count of records processed per entity</returns>
    /// <response code="200">Returns the normalization summary</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("normalize-all-records")]
    [Authorize(Policy = "Admin.Only")]
    [ProducesResponseType(typeof(NormalizationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NormalizationResultDto>> NormalizeAllRecords()
    {
        try
        {
            _log.Information("API: NormalizeAllRecords called by user {User}", User.Identity?.Name);
            
            var result = await _systemService.NormalizeAllRecordsAsync();

            if (!result.Success)
            {
                _log.Error("API: Normalization failed: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(500, result);
            }

            _log.Information("API: Successfully normalized {TotalCount} records in {Duration}ms", 
                result.TotalRecordsProcessed, result.Duration.TotalMilliseconds);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in NormalizeAllRecords");
            return StatusCode(500, new NormalizationResultDto
            {
                Success = false,
                ErrorMessage = "An error occurred while normalizing records: " + ex.Message
            });
        }
    }

    /// <summary>
    /// Health check endpoint for the system controller
    /// </summary>
    /// <returns>OK if the system is healthy</returns>
    /// <response code="200">System is healthy</response>
    [HttpGet("health")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "SystemController"
        });
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <param name="backupFileName">Optional custom backup file name (without extension)</param>
    /// <remarks>
    /// This endpoint creates a backup of the database based on the configured database type:
    /// - SQLite: Copies the database file to the Backups directory
    /// - SQL Server: Creates a .bak file using SQL Server BACKUP DATABASE command
    /// - PostgreSQL: Uses pg_dump to create a backup file (requires pg_dump in PATH)
    /// 
    /// The backup file will be created in the Backups directory in the application root.
    /// A timestamp will be automatically appended to the filename.
    /// 
    /// This operation should be run:
    /// - Before major updates or data migrations
    /// - As part of regular backup procedures
    /// - Before restore operations
    /// </remarks>
    /// <returns>Summary of backup results including file path and size</returns>
    /// <response code="200">Returns the backup summary</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("backup")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BackupResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BackupResultDto>> CreateBackup([FromQuery] string? backupFileName = null)
    {
        try
        {
            _log.Information("API: CreateBackup called by user {User} with filename {FileName}", 
                User.Identity?.Name, backupFileName ?? "(auto-generated)");
            
            var result = await _systemService.CreateBackupAsync(backupFileName);

            if (!result.Success)
            {
                _log.Error("API: Backup failed: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(500, result);
            }

            _log.Information("API: Successfully created backup at {BackupPath} ({Size} bytes) in {Duration}ms", 
                result.BackupFilePath, result.BackupFileSizeBytes, result.Duration.TotalMilliseconds);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateBackup");
            return StatusCode(500, new BackupResultDto
            {
                Success = false,
                ErrorMessage = "An error occurred while creating backup: " + ex.Message
            });
        }
    }

    /// <summary>
    /// Restores the database from a backup file
    /// </summary>
    /// <param name="backupFilePath">Full path to the backup file to restore</param>
    /// <remarks>
    /// This endpoint restores the database from a backup file based on the configured database type:
    /// - SQLite: Copies the backup file to replace the current database (creates pre-restore backup)
    /// - SQL Server: Uses SQL Server RESTORE DATABASE command
    /// - PostgreSQL: Uses pg_restore to restore the database (requires pg_restore in PATH)
    /// 
    /// WARNING: This operation will replace all current data with the backup data.
    /// For SQLite, a pre-restore backup of the current database is automatically created.
    /// 
    /// This operation should be run:
    /// - To recover from data corruption or loss
    /// - To restore a previous state of the database
    /// - Only when you are certain you want to replace current data
    /// </remarks>
    /// <returns>Summary of restore results</returns>
    /// <response code="200">Returns the restore summary</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have Admin role</response>
    /// <response code="404">If backup file is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("restore")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RestoreResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RestoreResultDto>> RestoreFromBackup([FromQuery] string backupFilePath)
    {
        try
        {
            _log.Information("API: RestoreFromBackup called by user {User} with file {FilePath}", 
                User.Identity?.Name, backupFilePath);

            if (string.IsNullOrWhiteSpace(backupFilePath))
            {
                return BadRequest(new RestoreResultDto
                {
                    Success = false,
                    ErrorMessage = "Backup file path is required"
                });
            }

            if (!System.IO.File.Exists(backupFilePath))
            {
                return NotFound(new RestoreResultDto
                {
                    Success = false,
                    ErrorMessage = $"Backup file not found: {backupFilePath}"
                });
            }
            
            var result = await _systemService.RestoreFromBackupAsync(backupFilePath);

            if (!result.Success)
            {
                _log.Error("API: Restore failed: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(500, result);
            }

            _log.Information("API: Successfully restored database from {BackupPath} in {Duration}ms", 
                backupFilePath, result.Duration.TotalMilliseconds);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in RestoreFromBackup");
            return StatusCode(500, new RestoreResultDto
            {
                Success = false,
                ErrorMessage = "An error occurred while restoring from backup: " + ex.Message
            });
        }
    }

    private async Task<Quote> UpsertQuoteFromOfferAsync(OfferDocumentDto offer, bool markAsSent)
    {
        var customerId = offer.SaleContext?.Customer?.Id ?? 0;
        if (customerId <= 0)
        {
            throw new InvalidOperationException("Offer customer is required for persistence.");
        }

        var customer = await _context.Customers.FindAsync(customerId)
            ?? throw new InvalidOperationException($"Customer with ID {customerId} was not found.");

        var now = DateTime.UtcNow;
        var oneTimeSubtotal = offer.Totals?.OneTimeSubtotal ?? 0;
        var recurringSubtotal = offer.Totals?.RecurringSubtotal ?? 0;
        var grandTotal = offer.Totals?.GrandTotal ?? (oneTimeSubtotal + recurringSubtotal);

        Quote quote;
        if (offer.QuoteId.HasValue)
        {
            quote = await _context.Quotes.FirstOrDefaultAsync(q => q.Id == offer.QuoteId.Value && q.DeletedAt == null)
                ?? CreateNewQuote(customer, now, offer);
            if (quote.Id == 0)
            {
                _context.Quotes.Add(quote);
            }
        }
        else
        {
            quote = CreateNewQuote(customer, now, offer);
            _context.Quotes.Add(quote);
        }

        quote.CustomerId = customer.Id;
        quote.CustomerName = customer.Name;
        quote.CurrencyCode = offer.SaleContext?.Currency ?? "USD";
        quote.ValidUntil = ParseValidUntil(offer) ?? now.AddDays(14);
        quote.Notes = offer.OfferSettings?.Notes ?? string.Empty;
        quote.SubTotal = oneTimeSubtotal + recurringSubtotal;
        quote.TotalSetupFee = oneTimeSubtotal;
        quote.TotalRecurring = recurringSubtotal;
        quote.TotalAmount = grandTotal;
        quote.DiscountAmount = CalculateDiscountAmount(oneTimeSubtotal + recurringSubtotal, grandTotal);

        if (markAsSent)
        {
            quote.Status = QuoteStatus.Sent;
            quote.SentAt ??= now;
        }

        await _context.SaveChangesAsync();

        var existingLines = _context.QuoteLines.Where(line => line.QuoteId == quote.Id);
        _context.QuoteLines.RemoveRange(existingLines);

        var defaultBillingCycle = await _context.BillingCycles
            .OrderBy(c => c.Id)
            .Select(c => new { c.Id, c.Name })
            .FirstOrDefaultAsync();
        if (defaultBillingCycle == null)
        {
            throw new InvalidOperationException("At least one billing cycle must exist before persisting offer lines.");
        }

        var quoteLines = offer.LineItems.Select(item =>
        {
            var isRecurring = string.Equals(item.Type, "Recurring", StringComparison.OrdinalIgnoreCase);
            var quantity = item.Quantity <= 0 ? 1 : (int)Math.Ceiling(item.Quantity);

            return new QuoteLine
            {
                QuoteId = quote.Id,
                ServiceId = item.ServiceId,
                BillingCycleId = defaultBillingCycle.Id,
                LineNumber = item.LineNumber > 0 ? item.LineNumber : 1,
                Description = item.Description ?? string.Empty,
                Quantity = quantity,
                SetupFee = isRecurring ? 0 : item.UnitPrice,
                RecurringPrice = isRecurring ? item.UnitPrice : 0,
                Discount = 0,
                TotalSetupFee = isRecurring ? 0 : item.Subtotal,
                TotalRecurringPrice = isRecurring ? item.Subtotal : 0,
                TaxRate = 0,
                TaxAmount = 0,
                TotalWithTax = item.Subtotal,
                ServiceNameSnapshot = item.Description ?? string.Empty,
                BillingCycleNameSnapshot = defaultBillingCycle.Name,
                Notes = string.Empty
            };
        }).ToList();

        _context.QuoteLines.AddRange(quoteLines);
        await _context.SaveChangesAsync();

        return quote;
    }

    private Quote CreateNewQuote(Customer customer, DateTime now, OfferDocumentDto offer)
    {
        return new Quote
        {
            QuoteNumber = $"Q-{now:yyyyMMddHHmmssfff}",
            CustomerId = customer.Id,
            Status = QuoteStatus.Draft,
            ValidUntil = ParseValidUntil(offer) ?? now.AddDays(14),
            CurrencyCode = offer.SaleContext?.Currency ?? "USD",
            TaxRate = 0,
            TaxName = "VAT",
            CustomerName = customer.Name,
            CustomerAddress = string.Empty,
            CustomerTaxId = customer.TaxId ?? string.Empty,
            Notes = offer.OfferSettings?.Notes ?? string.Empty,
            TermsAndConditions = string.Empty,
            InternalComment = string.Empty,
            RejectionReason = string.Empty,
            AcceptanceToken = Guid.NewGuid().ToString("N")
        };
    }

    private static DateTime? ParseValidUntil(OfferDocumentDto offer)
    {
        return offer.OfferSettings?.ValidUntil?.ToDateTime(TimeOnly.MinValue);
    }

    private static decimal CalculateDiscountAmount(decimal gross, decimal grandTotal)
    {
        var discount = gross - grandTotal;
        return discount > 0 ? discount : 0;
    }

    private async Task<(string fileName, string outputPath)> GenerateOfferPdfAsync(OfferDocumentDto offer, string actionPrefix)
    {
        var outputDirectory = ResolveOutputDirectory();
        var reportGenerator = new QuestPdfReportGenerator(outputDirectory);
        var safeDomain = SanitizeFileName(offer.SaleContext?.DomainName);
        var fileName = $"Offer-{actionPrefix}-{safeDomain}-{DateTime.UtcNow:yyyyMMddHHmmssfff}.pdf";
        var outputPath = Path.Combine(outputDirectory, fileName);

        await reportGenerator.SaveReportAsync(ReportType.Offer, offer, outputPath, OutputFormat.Pdf);
        return (fileName, outputPath);
    }

    private async Task<QuoteRevision> SaveQuoteRevisionAsync(
        Quote quote,
        OfferDocumentDto offer,
        string actionType,
        string fileName,
        string outputPath)
    {
        var nextRevisionNumber = await _context.QuoteRevisions
            .Where(r => r.QuoteId == quote.Id)
            .Select(r => (int?)r.RevisionNumber)
            .MaxAsync() ?? 0;

        var snapshotJson = JsonSerializer.Serialize(offer, new JsonSerializerOptions { WriteIndented = true });
        var contentHash = string.Empty;
        if (System.IO.File.Exists(outputPath))
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(outputPath);
            contentHash = Convert.ToHexString(SHA256.HashData(bytes));
        }

        var revision = new QuoteRevision
        {
            QuoteId = quote.Id,
            RevisionNumber = nextRevisionNumber + 1,
            QuoteStatus = quote.Status,
            ActionType = actionType,
            SnapshotJson = snapshotJson,
            PdfFileName = fileName,
            PdfFilePath = outputPath,
            ContentHash = contentHash,
            Notes = "Generated from dashboard/new-sale/offer"
        };

        _context.QuoteRevisions.Add(revision);
        await _context.SaveChangesAsync();
        return revision;
    }

    private string ResolveOutputDirectory()
    {
        var configuredOutputDirectory = _configuration["ReportSettings:QuestPdf:OutputPath"];
        if (!string.IsNullOrWhiteSpace(configuredOutputDirectory))
        {
            return configuredOutputDirectory;
        }

        return Path.Combine(AppContext.BaseDirectory, "GeneratedReports", "OfferVerification");
    }

    private static string SanitizeFileName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "offer";
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(value.Select(ch => invalidChars.Contains(ch) ? '-' : ch).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(sanitized) ? "offer" : sanitized;
    }
}
