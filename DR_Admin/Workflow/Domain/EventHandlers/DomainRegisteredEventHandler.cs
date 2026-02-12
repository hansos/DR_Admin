using ISPAdmin.DTOs;
using ISPAdmin.Services;
using ISPAdmin.Workflow.Domain.Events.DomainEvents;
using ISPAdmin.Workflow.Domain.Services;
using MessagingTemplateLib.Templating;
using MessagingTemplateLib.Models;
using MessagingTemplateLib;

namespace ISPAdmin.Workflow.Domain.EventHandlers;

/// <summary>
/// Handles the DomainRegistered event
/// </summary>
public class DomainRegisteredEventHandler : IDomainEventHandler<DomainRegisteredEvent>
{
    private readonly IEmailQueueService _emailService;
    private readonly ICustomerService _customerService;
    private readonly MessagingService _messagingService;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<DomainRegisteredEventHandler>();

    public DomainRegisteredEventHandler(
        IEmailQueueService emailService,
        ICustomerService customerService,
        MessagingService messagingService)
    {
        _emailService = emailService;
        _customerService = customerService;
        _messagingService = messagingService;
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

            // Prepare template model
            var model = new DomainRegisteredModel
            {
                DomainName = @event.DomainName,
                RegistrationDate = @event.OccurredAt.ToString("yyyy-MM-dd"),
                ExpirationDate = @event.ExpirationDate.ToString("yyyy-MM-dd"),
                AutoRenew = @event.AutoRenew ? "Enabled" : "Disabled",
                CustomerPortalUrl = "https://portal.example.com/domains" // TODO: Get from configuration
            };

            // Render email from template
            var emailBody = _messagingService.RenderMessage("DomainRegistered", MessageChannel.EmailHtml, model);

            // Send welcome email
            await _emailService.QueueEmailAsync(new QueueEmailDto
            {
                To = customer.Email,
                Subject = $"Domain Registration Successful - {@event.DomainName}",
                BodyHtml = emailBody
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
}
