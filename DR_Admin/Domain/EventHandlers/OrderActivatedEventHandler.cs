using ISPAdmin.Domain.Events.OrderEvents;
using ISPAdmin.Domain.Services;
using ISPAdmin.DTOs;
using ISPAdmin.Services;

namespace ISPAdmin.Domain.EventHandlers;

/// <summary>
/// Handles the OrderActivated event
/// </summary>
public class OrderActivatedEventHandler : IDomainEventHandler<OrderActivatedEvent>
{
    private readonly IEmailQueueService _emailService;
    private readonly ICustomerService _customerService;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<OrderActivatedEventHandler>();

    public OrderActivatedEventHandler(
        IEmailQueueService emailService,
        ICustomerService customerService)
    {
        _emailService = emailService;
        _customerService = customerService;
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

            // Send activation confirmation email
            await _emailService.QueueEmailAsync(new QueueEmailDto
            {
                To = customer.Email,
                Subject = $"Service Activated - Order {@event.OrderNumber}",
                BodyHtml = BuildActivationEmailBody(@event)
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

    private string BuildActivationEmailBody(OrderActivatedEvent @event)
    {
        return $@"
Hello,

Your service order {@event.OrderNumber} has been successfully activated!

Activation Details:
- Order Number: {@event.OrderNumber}
- Activated Date: {@event.ActivatedAt:yyyy-MM-dd HH:mm:ss} UTC

You can now start using your service. Please visit your customer portal for more details.

Thank you for your business!

Best regards,
Your Service Team
";
    }
}
