using ISPAdmin.Data;
using ISPAdmin.Data.Enums;
using ISPAdmin.Workflow.Domain.Events.OrderEvents;
using ISPAdmin.Workflow.Domain.Services;
using ISPAdmin.Workflow.Domain.StateMachines;
using Microsoft.EntityFrameworkCore;

namespace ISPAdmin.Workflow.Domain.Workflows;

/// <summary>
/// Orchestrates the order provisioning workflow
/// </summary>
public class OrderProvisioningWorkflow : IOrderProvisioningWorkflow
{
    private readonly ApplicationDbContext _context;
    private readonly IDomainEventPublisher _eventPublisher;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<OrderProvisioningWorkflow>();

    public OrderProvisioningWorkflow(
        ApplicationDbContext context,
        IDomainEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task<WorkflowResult> ProvisionAsync(int orderId)
    {
        try
        {
            _log.Information("Starting provisioning workflow for order ID: {OrderId}", orderId);

            var order = await _context.Orders
                .Include(o => o.Service)
                    .ThenInclude(s => s.ServiceType)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return WorkflowResult.Failed($"Order with ID {orderId} not found");
            }

            // Validate state transition
            if (!OrderStateMachine.CanTransition(order.Status, OrderTransition.Activate))
            {
                return WorkflowResult.Failed($"Cannot activate order in status {order.Status}");
            }

            // Provision based on service type
            var serviceType = order.Service?.ServiceType?.Name ?? "Unknown";

            switch (serviceType)
            {
                case "Domain Registration":
                    // Domain provisioning is handled by DomainRegistrationWorkflow
                    _log.Information("Domain provisioning handled by DomainRegistrationWorkflow");
                    break;

                case "Hosting":
                    await ProvisionHostingAsync(order);
                    break;

                case "Email":
                    await ProvisionEmailAsync(order);
                    break;

                default:
                    _log.Warning("Unknown service type {ServiceType} for order {OrderId}", 
                        serviceType, orderId);
                    break;
            }

            // Update order status
            order.Status = OrderStateMachine.Transition(order.Status, OrderTransition.Activate);
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Publish order activated event
            await _eventPublisher.PublishAsync(new OrderActivatedEvent
            {
                AggregateId = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                ServiceId = order.ServiceId,
                ActivatedAt = DateTime.UtcNow
            });

            _log.Information("Order {OrderNumber} provisioned and activated successfully", order.OrderNumber);

            return WorkflowResult.Success(orderId, "Order provisioned successfully");
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Provisioning workflow failed for order ID: {OrderId}", orderId);
            return WorkflowResult.Failed(ex.Message);
        }
    }

    private async Task ProvisionHostingAsync(Data.Entities.Order order)
    {
        _log.Information("Provisioning hosting for order {OrderId}", order.Id);

        // TODO: Implement hosting provisioning logic
        // - Create hosting account
        // - Setup control panel access
        // - Configure DNS
        // - Send credentials to customer

        await Task.CompletedTask;
    }

    private async Task ProvisionEmailAsync(Data.Entities.Order order)
    {
        _log.Information("Provisioning email service for order {OrderId}", order.Id);

        // TODO: Implement email provisioning logic
        // - Create email accounts
        // - Setup mailboxes
        // - Configure SMTP/IMAP access
        // - Send credentials to customer

        await Task.CompletedTask;
    }
}
