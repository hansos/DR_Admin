using ISPAdmin.Data;
using ISPAdmin.Domain.Events.InvoiceEvents;
using ISPAdmin.Domain.Services;
using ISPAdmin.Domain.Workflows;
using Microsoft.EntityFrameworkCore;

namespace ISPAdmin.Domain.EventHandlers;

/// <summary>
/// Handles the InvoicePaid event
/// </summary>
public class InvoicePaidEventHandler : IDomainEventHandler<InvoicePaidEvent>
{
    private readonly ApplicationDbContext _context;
    private readonly IDomainRegistrationWorkflow _domainRegistrationWorkflow;
    private readonly IOrderProvisioningWorkflow _orderProvisioningWorkflow;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<InvoicePaidEventHandler>();

    public InvoicePaidEventHandler(
        ApplicationDbContext context,
        IDomainRegistrationWorkflow domainRegistrationWorkflow,
        IOrderProvisioningWorkflow orderProvisioningWorkflow)
    {
        _context = context;
        _domainRegistrationWorkflow = domainRegistrationWorkflow;
        _orderProvisioningWorkflow = orderProvisioningWorkflow;
    }

    public async Task HandleAsync(InvoicePaidEvent @event)
    {
        try
        {
            _log.Information("Handling InvoicePaid event for invoice {InvoiceNumber}", 
                @event.InvoiceNumber);

            // Get invoice with related data
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceLines)
                .FirstOrDefaultAsync(i => i.Id == @event.AggregateId);

            if (invoice == null)
            {
                _log.Warning("Invoice {InvoiceId} not found", @event.AggregateId);
                return;
            }

            // Find pending orders for this customer to process
            // Since InvoiceLine doesn't have OrderId, we match by customer and pending status
            var pendingOrders = await _context.Orders
                .Include(o => o.Service)
                    .ThenInclude(s => s.ServiceType)
                .Where(o => o.CustomerId == invoice.CustomerId && 
                           o.Status == Data.Enums.OrderStatus.Pending)
                .ToListAsync();

            if (!pendingOrders.Any())
            {
                _log.Information("No pending orders found for customer {CustomerId}", invoice.CustomerId);
                return;
            }

            // Process each pending order
            foreach (var order in pendingOrders)
            {
                var serviceType = order.Service?.ServiceType?.Name ?? "Unknown";

                // Route to appropriate workflow based on service type
                if (serviceType == "Domain Registration")
                {
                    _log.Information("Triggering domain registration workflow for order {OrderId}", 
                        order.Id);
                    await _domainRegistrationWorkflow.OnPaymentReceivedAsync(order.Id, invoice.Id);
                }
                else
                {
                    _log.Information("Triggering provisioning workflow for order {OrderId}", 
                        order.Id);
                    await _orderProvisioningWorkflow.ProvisionAsync(order.Id);
                }
            }

            _log.Information("InvoicePaid event handled successfully for invoice {InvoiceNumber}", 
                @event.InvoiceNumber);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error handling InvoicePaid event for invoice {InvoiceNumber}", 
                @event.InvoiceNumber);
            throw;
        }
    }
}
