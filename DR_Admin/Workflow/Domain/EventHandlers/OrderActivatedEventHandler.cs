using ISPAdmin.DTOs;
using ISPAdmin.Services;
using ISPAdmin.Workflow.Domain.Events.OrderEvents;
using ISPAdmin.Workflow.Domain.Services;
using MessagingTemplateLib.Templating;
using MessagingTemplateLib.Models;
using MessagingTemplateLib;
using ISPAdmin.Data;
using Microsoft.EntityFrameworkCore;

namespace ISPAdmin.Workflow.Domain.EventHandlers;

/// <summary>
/// Handles the OrderActivated event
/// </summary>
public class OrderActivatedEventHandler : IDomainEventHandler<OrderActivatedEvent>
{
    private readonly IEmailQueueService _emailService;
    private readonly ICustomerService _customerService;
    private readonly MessagingService _messagingService;
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<OrderActivatedEventHandler>();

    public OrderActivatedEventHandler(
        IEmailQueueService emailService,
        ICustomerService customerService,
        MessagingService messagingService,
        ApplicationDbContext context)
    {
        _emailService = emailService;
        _customerService = customerService;
        _messagingService = messagingService;
        _context = context;
    }

    public async Task HandleAsync(OrderActivatedEvent @event)
    {
        try
        {
            _log.Information("Handling OrderActivated event for order {OrderNumber}", 
                @event.OrderNumber);

            // Get customer
            var customer = await _customerService.GetCustomerByIdAsync(@event.CustomerId);

            if (customer == null)
            {
                _log.Warning("Customer {CustomerId} not found for order activation event", 
                    @event.CustomerId);
                return;
            }

            // Get service name from database
            var service = await _context.Services
                .Include(s => s.ServiceType)
                .FirstOrDefaultAsync(s => s.Id == @event.ServiceId);

            var serviceName = service?.ServiceType?.Name ?? service?.Name ?? "Your Service";

            // Create template model
            var model = new OrderActivatedModel
            {
                OrderNumber = @event.OrderNumber,
                ServiceName = serviceName,
                ActivatedAt = @event.ActivatedAt.ToString("yyyy-MM-dd HH:mm:ss") + " UTC",
                CustomerPortalUrl = "https://portal.example.com/orders" // TODO: Get from configuration
            };

            // Render both HTML and plain text versions
            var emailBodyHtml = _messagingService.RenderMessage("OrderActivated", MessageChannel.EmailHtml, model);
            var emailBodyText = _messagingService.RenderMessage("OrderActivated", MessageChannel.EmailText, model);

            // Send activation confirmation email
            await _emailService.QueueEmailAsync(new QueueEmailDto
            {
                To = customer.Email,
                Subject = $"Service Activated - Order {@event.OrderNumber}",
                BodyHtml = emailBodyHtml,
                BodyText = emailBodyText
            });

            _log.Information("OrderActivated event handled successfully for order {OrderNumber}", 
                @event.OrderNumber);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error handling OrderActivated event for order {OrderNumber}", 
                @event.OrderNumber);
            throw;
        }
    }
}
