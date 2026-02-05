using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.Domain.Events.DomainEvents;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.EntityFrameworkCore;
using DomainRegistrationLib.Factories;
using DomainRegistrationLib.Models;
using DomainEntity = ISPAdmin.Data.Entities.Domain;
using ISPAdmin.Workflow.Domain.Services;

namespace ISPAdmin.Workflow.Domain.Workflows;

/// <summary>
/// Orchestrates the domain renewal workflow
/// </summary>
public class DomainRenewalWorkflow : IDomainRenewalWorkflow
{
    private readonly ApplicationDbContext _context;
    private readonly IDomainEventPublisher _eventPublisher;
    private readonly IInvoiceService _invoiceService;
    private readonly IEmailQueueService _emailService;
    private readonly DomainRegistrarFactory _registrarFactory;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<DomainRenewalWorkflow>();

    public DomainRenewalWorkflow(
        ApplicationDbContext context,
        IDomainEventPublisher eventPublisher,
        IInvoiceService invoiceService,
        IEmailQueueService emailService,
        DomainRegistrarFactory registrarFactory)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _invoiceService = invoiceService;
        _emailService = emailService;
        _registrarFactory = registrarFactory;
    }

    public async Task<WorkflowResult> ExecuteAsync(int domainId)
    {
        try
        {
            _log.Information("Starting domain renewal workflow for domain ID: {DomainId}", domainId);

            var domain = await _context.Domains
                .Include(d => d.Service)
                .Include(d => d.Customer)
                .Include(d => d.Registrar)
                .FirstOrDefaultAsync(d => d.Id == domainId);

            if (domain == null)
            {
                return WorkflowResult.Failed($"Domain with ID {domainId} not found");
            }

            // Check if renewal is needed
            var daysUntilExpiration = (domain.ExpirationDate - DateTime.UtcNow).Days;

            if (daysUntilExpiration > 30)
            {
                _log.Information("Domain {DomainName} not yet due for renewal. Days until expiration: {Days}", 
                    domain.Name, daysUntilExpiration);
                return WorkflowResult.Success(domainId, "Domain not yet due for renewal");
            }

            // Generate renewal invoice
            var invoice = await GenerateRenewalInvoiceAsync(domain);

            // If auto-renew is enabled, attempt automatic renewal
            if (domain.AutoRenew)
            {
                return await ProcessAutoRenewalAsync(domainId);
            }
            else
            {
                // Send renewal reminder email
                await SendRenewalReminderEmailAsync(domain, invoice);

                _log.Information("Renewal reminder sent for domain {DomainName}", domain.Name);
                return WorkflowResult.Success(domainId, "Renewal reminder sent");
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Domain renewal workflow failed for domain ID: {DomainId}", domainId);
            return WorkflowResult.Failed(ex.Message);
        }
    }

    public async Task<WorkflowResult> ProcessAutoRenewalAsync(int domainId)
    {
        try
        {
            _log.Information("Processing auto-renewal for domain ID: {DomainId}", domainId);

            var domain = await _context.Domains
                .Include(d => d.Registrar)
                .Include(d => d.Customer)
                .FirstOrDefaultAsync(d => d.Id == domainId);

            if (domain == null)
            {
                return WorkflowResult.Failed($"Domain with ID {domainId} not found");
            }

            // Check for active payment method
            var hasPaymentMethod = await HasActivePaymentMethodAsync(domain.CustomerId);

            if (!hasPaymentMethod)
            {
                _log.Warning("No active payment method found for customer {CustomerId}", domain.CustomerId);
                await SendRenewalReminderEmailAsync(domain, null);
                return WorkflowResult.Failed("No active payment method available");
            }

            // Generate invoice
            var invoice = await GenerateRenewalInvoiceAsync(domain);

            // TODO: Process payment using customer's default payment method
            // For now, we'll simulate successful payment
            var paymentSuccessful = false; // await ProcessPaymentAsync(domain.CustomerId, invoice.TotalAmount);

            if (paymentSuccessful)
            {
                // TODO: Renew domain with registrar once configuration is complete
                // var domainRegistrar = _registrarFactory.CreateRegistrar();
                // var renewalResult = await domainRegistrar.RenewDomainAsync(new DomainRenewalRequest
                // {
                //     DomainName = domain.Name,
                //     Years = 1
                // });
                
                // For now, simulate successful renewal
                var renewalSuccessful = true;

                if (renewalSuccessful)
                {
                    // Update domain expiration date
                    var previousExpiration = domain.ExpirationDate;
                    domain.ExpirationDate = domain.ExpirationDate.AddYears(1); // Extend by 1 year
                    domain.UpdatedAt = DateTime.UtcNow;

                    // Mark invoice as paid
                    var invoiceEntity = await _context.Invoices.FindAsync(invoice.Id);
                    if (invoiceEntity != null)
                    {
                        invoiceEntity.Status = InvoiceStatus.Paid;
                        invoiceEntity.PaidAt = DateTime.UtcNow;
                        invoiceEntity.AmountPaid = invoiceEntity.TotalAmount;
                        invoiceEntity.AmountDue = 0;
                    }

                    await _context.SaveChangesAsync();

                    // Publish domain renewed event
                    await _eventPublisher.PublishAsync(new DomainRenewedEvent
                    {
                        AggregateId = domain.Id,
                        DomainName = domain.Name,
                        PreviousExpirationDate = previousExpiration,
                        NewExpirationDate = domain.ExpirationDate,
                        RenewalPrice = domain.RenewalPrice ?? 0,
                        RenewalYears = 1
                    });

                    _log.Information("Domain {DomainName} renewed successfully until {ExpirationDate}", 
                        domain.Name, domain.ExpirationDate);

                    return WorkflowResult.Success(domainId, "Domain renewed successfully");
                }
                else
                {
                    _log.Error("Registrar renewal failed for domain {DomainName}", domain.Name);
                    return WorkflowResult.Failed("Registrar renewal failed");
                }
            }
            else
            {
                _log.Warning("Payment failed for domain renewal {DomainName}", domain.Name);
                await SendRenewalReminderEmailAsync(domain, invoice);
                return WorkflowResult.Failed("Payment failed");
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Auto-renewal failed for domain ID: {DomainId}", domainId);
            return WorkflowResult.Failed(ex.Message);
        }
    }

    private async Task<bool> HasActivePaymentMethodAsync(int customerId)
    {
        return await _context.CustomerPaymentMethods
            .AnyAsync(pm => pm.CustomerId == customerId && pm.IsDefault);
    }

    private async Task<InvoiceDto> GenerateRenewalInvoiceAsync(DomainEntity domain)
    {
        var invoiceNumber = await GenerateInvoiceNumberAsync();

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            CustomerId = domain.CustomerId,
            Status = InvoiceStatus.Draft,
            IssueDate = DateTime.UtcNow,
            DueDate = domain.ExpirationDate,
            SubTotal = domain.RenewalPrice ?? 12.99m,
            TaxAmount = 0,
            TotalAmount = domain.RenewalPrice ?? 12.99m,
            AmountDue = domain.RenewalPrice ?? 12.99m,
            AmountPaid = 0,
            CurrencyCode = "EUR",
            DisplayCurrencyCode = "EUR",
            BaseCurrencyCode = "EUR",
            TaxRate = 0,
            TaxName = "VAT",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        // Add invoice line
        var invoiceLine = new InvoiceLine
        {
            InvoiceId = invoice.Id,
            Description = $"Domain Renewal - {domain.Name}",
            Quantity = 1,
            UnitPrice = domain.RenewalPrice ?? 12.99m,
            TaxRate = 0,
            TaxAmount = 0,
            TotalPrice = domain.RenewalPrice ?? 12.99m,
            TotalWithTax = domain.RenewalPrice ?? 12.99m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.InvoiceLines.Add(invoiceLine);
        await _context.SaveChangesAsync();

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            Status = invoice.Status,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            TotalAmount = invoice.TotalAmount
        };
    }

    private async Task SendRenewalReminderEmailAsync(DomainEntity domain, InvoiceDto? invoice)
    {
        await _emailService.QueueEmailAsync(new QueueEmailDto
        {
            To = domain.Customer.Email,
            Subject = $"Domain Renewal Reminder - {domain.Name}",
            BodyHtml = $"Your domain {domain.Name} expires on {domain.ExpirationDate:yyyy-MM-dd}. " +
                   $"Please renew to avoid service interruption."
        });
    }

    private async Task<string> GenerateInvoiceNumberAsync()
    {
        var lastInvoice = await _context.Invoices
            .OrderByDescending(i => i.Id)
            .FirstOrDefaultAsync();

        var nextNumber = (lastInvoice?.Id ?? 0) + 1;
        return $"INV-{DateTime.UtcNow.Year}-{nextNumber:D5}";
    }
}
