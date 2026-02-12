using ISPAdmin.DTOs;
using ISPAdmin.Services;
using ISPAdmin.Workflow.Domain.Events.DomainEvents;
using ISPAdmin.Workflow.Domain.Services;
using MessagingTemplateLib.Templating;
using MessagingTemplateLib.Models;
using MessagingTemplateLib;

namespace ISPAdmin.Workflow.Domain.EventHandlers;

/// <summary>
/// Handles the DomainExpired event
/// </summary>
public class DomainExpiredEventHandler : IDomainEventHandler<DomainExpiredEvent>
{
    private readonly IEmailQueueService _emailService;
    private readonly ICustomerService _customerService;
    private readonly MessagingService _messagingService;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<DomainExpiredEventHandler>();

    public DomainExpiredEventHandler(
        IEmailQueueService emailService,
        ICustomerService customerService,
        MessagingService messagingService)
    {
        _emailService = emailService;
        _customerService = customerService;
        _messagingService = messagingService;
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

            // Prepare template model
            var model = new DomainExpiredModel
            {
                DomainName = @event.DomainName,
                ExpiredAt = @event.ExpiredAt.ToString("yyyy-MM-dd"),
                AutoRenewEnabled = @event.AutoRenewEnabled ? "Enabled" : "Disabled",
                RenewUrl = $"https://portal.example.com/domains/{@event.DomainName}/renew" // TODO: Get from configuration
            };

            // Render email from template
            var emailBody = _messagingService.RenderMessage("DomainExpired", MessageChannel.EmailHtml, model);

            // Send expiration notification
            await _emailService.QueueEmailAsync(new QueueEmailDto
            {
                To = customer.Email,
                Subject = $"URGENT: Domain Expired - {@event.DomainName}",
                BodyHtml = emailBody
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
}
