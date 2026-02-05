using ISPAdmin.DTOs;
using ISPAdmin.Services;
using ISPAdmin.Workflow.Domain.Events.DomainEvents;
using ISPAdmin.Workflow.Domain.Services;

namespace ISPAdmin.Workflow.Domain.EventHandlers;

/// <summary>
/// Handles the DomainRegistered event
/// </summary>
public class DomainRegisteredEventHandler : IDomainEventHandler<DomainRegisteredEvent>
{
    private readonly IEmailQueueService _emailService;
    private readonly ICustomerService _customerService;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<DomainRegisteredEventHandler>();

    public DomainRegisteredEventHandler(
        IEmailQueueService emailService,
        ICustomerService customerService)
    {
        _emailService = emailService;
        _customerService = customerService;
    }

    public async Task HandleAsync(DomainRegisteredEvent @event)
    {
        try
        {
            _log.Information("Handling DomainRegistered event for domain {DomainName}", @event.DomainName);

            // Get customer email
            var customer = await _customerService.GetCustomerByIdAsync(@event.CustomerId);
            
            if (customer == null)
            {
                _log.Warning("Customer {CustomerId} not found for domain registration event", 
                    @event.CustomerId);
                return;
            }

            // Send welcome email
            await _emailService.QueueEmailAsync(new QueueEmailDto
            {
                To = customer.Email,
                Subject = $"Domain Registration Successful - {@event.DomainName}",
                BodyHtml = BuildWelcomeEmailBody(@event)
            });

            // TODO: Additional side effects
            // - Create DNS zone
            // - Setup default DNS records
            // - Send to monitoring system
            // - Update CRM

            _log.Information("DomainRegistered event handled successfully for domain {DomainName}", 
                @event.DomainName);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error handling DomainRegistered event for domain {DomainName}", 
                @event.DomainName);
            throw;
        }
    }

    private string BuildWelcomeEmailBody(DomainRegisteredEvent @event)
    {
        return $@"
Hello,

Your domain {@event.DomainName} has been successfully registered!

Domain Details:
- Domain Name: {@event.DomainName}
- Registration Date: {@event.OccurredAt:yyyy-MM-dd}
- Expiration Date: {@event.ExpirationDate:yyyy-MM-dd}
- Auto-Renew: {@event.AutoRenew}

You can manage your domain through your customer portal.

Thank you for choosing our services!

Best regards,
Your Domain Registration Team
";
    }
}
