using ISPAdmin.Domain.Events.DomainEvents;
using ISPAdmin.Domain.Services;
using ISPAdmin.DTOs;
using ISPAdmin.Services;

namespace ISPAdmin.Domain.EventHandlers;

/// <summary>
/// Handles the DomainExpired event
/// </summary>
public class DomainExpiredEventHandler : IDomainEventHandler<DomainExpiredEvent>
{
    private readonly IEmailQueueService _emailService;
    private readonly ICustomerService _customerService;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<DomainExpiredEventHandler>();

    public DomainExpiredEventHandler(
        IEmailQueueService emailService,
        ICustomerService customerService)
    {
        _emailService = emailService;
        _customerService = customerService;
    }

    public async Task HandleAsync(DomainExpiredEvent @event)
    {
        try
        {
            _log.Information("Handling DomainExpired event for domain {DomainName}", 
                @event.DomainName);

            // Get customer
            var customer = await _customerService.GetCustomerByIdAsync(@event.CustomerId);

            if (customer == null)
            {
                _log.Warning("Customer {CustomerId} not found for domain expiration event", 
                    @event.CustomerId);
                return;
            }

            // Send expiration notification
            await _emailService.QueueEmailAsync(new QueueEmailDto
            {
                To = customer.Email,
                Subject = $"URGENT: Domain Expired - {@event.DomainName}",
                BodyHtml = BuildExpirationEmailBody(@event)
            });

            _log.Information("DomainExpired event handled successfully for domain {DomainName}", 
                @event.DomainName);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error handling DomainExpired event for domain {DomainName}", 
                @event.DomainName);
            throw;
        }
    }

    private string BuildExpirationEmailBody(DomainExpiredEvent @event)
    {
        return $@"
URGENT NOTICE

Your domain {@event.DomainName} has EXPIRED!

Expiration Date: {@event.ExpiredAt:yyyy-MM-dd}
Auto-Renew Status: {@event.AutoRenewEnabled}

IMMEDIATE ACTION REQUIRED:
To prevent permanent loss of your domain, please renew it immediately through your customer portal.

Your domain may enter a redemption period where recovery fees apply. Act now to avoid additional costs.

Please contact support if you need assistance.

Best regards,
Your Domain Registration Team
";
    }
}
