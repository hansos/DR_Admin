using System.Text.Json;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Domain.EventHandlers;
using ISPAdmin.Workflow.Domain.Events;
using ISPAdmin.Workflow.Domain.Events.DomainEvents;
using ISPAdmin.Workflow.Domain.Events.InvoiceEvents;
using ISPAdmin.Workflow.Domain.Events.OrderEvents;
using ISPAdmin.Workflow.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ISPAdmin.BackgroundServices;

/// <summary>
/// Background service that processes events from the outbox table
/// Ensures reliable event delivery and handler execution
/// </summary>
public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<OutboxProcessorService>();
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10);
    private const int MaxRetries = 5;
    private const int BatchSize = 100;

    public OutboxProcessorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.Information("Outbox Processor Service starting");

        // Wait a bit before starting
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingEventsAsync(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in outbox processor");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        _log.Information("Outbox Processor Service stopping");
    }

    private async Task ProcessPendingEventsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Get pending events
        var pendingEvents = await context.OutboxEvents
            .Where(e => e.ProcessedAt == null && e.RetryCount < MaxRetries)
            .OrderBy(e => e.OccurredAt)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (pendingEvents.Count == 0)
            return;

        _log.Information("Processing {Count} pending outbox events", pendingEvents.Count);

        foreach (var outboxEvent in pendingEvents)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await DispatchToHandlersAsync(scope, outboxEvent);

                // Mark as processed
                outboxEvent.ProcessedAt = DateTime.UtcNow;
                outboxEvent.ErrorMessage = null;

                _log.Information("Successfully processed event {EventId} of type {EventType}", 
                    outboxEvent.EventId, outboxEvent.EventType);
            }
            catch (Exception ex)
            {
                outboxEvent.RetryCount++;
                outboxEvent.ErrorMessage = $"{ex.Message}\n{ex.StackTrace}";

                _log.Error(ex, "Error processing event {EventId} of type {EventType} (retry {RetryCount}/{MaxRetries})", 
                    outboxEvent.EventId, outboxEvent.EventType, outboxEvent.RetryCount, MaxRetries);
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task DispatchToHandlersAsync(IServiceScope scope, OutboxEvent outboxEvent)
    {
        // Deserialize based on event type
        IDomainEvent? domainEvent = outboxEvent.EventType switch
        {
            "DomainRegistered" => JsonSerializer.Deserialize<DomainRegisteredEvent>(outboxEvent.Payload),
            "DomainRenewed" => JsonSerializer.Deserialize<DomainRenewedEvent>(outboxEvent.Payload),
            "DomainExpired" => JsonSerializer.Deserialize<DomainExpiredEvent>(outboxEvent.Payload),
            "DomainSuspended" => JsonSerializer.Deserialize<DomainSuspendedEvent>(outboxEvent.Payload),
            "DomainTransferred" => JsonSerializer.Deserialize<DomainTransferredEvent>(outboxEvent.Payload),
            
            "OrderCreated" => JsonSerializer.Deserialize<OrderCreatedEvent>(outboxEvent.Payload),
            "OrderActivated" => JsonSerializer.Deserialize<OrderActivatedEvent>(outboxEvent.Payload),
            "OrderSuspended" => JsonSerializer.Deserialize<OrderSuspendedEvent>(outboxEvent.Payload),
            "OrderCancelled" => JsonSerializer.Deserialize<OrderCancelledEvent>(outboxEvent.Payload),
            
            "InvoiceGenerated" => JsonSerializer.Deserialize<InvoiceGeneratedEvent>(outboxEvent.Payload),
            "InvoicePaid" => JsonSerializer.Deserialize<InvoicePaidEvent>(outboxEvent.Payload),
            "InvoiceOverdue" => JsonSerializer.Deserialize<InvoiceOverdueEvent>(outboxEvent.Payload),
            
            _ => null
        };

        if (domainEvent == null)
        {
            _log.Warning("Unknown event type {EventType} or deserialization failed", 
                outboxEvent.EventType);
            return;
        }

        // Dispatch to appropriate handlers
        await DispatchEventAsync(scope, domainEvent);
    }

    private async Task DispatchEventAsync(IServiceScope scope, IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case DomainRegisteredEvent e:
                var domainRegisteredHandler = scope.ServiceProvider
                    .GetRequiredService<IDomainEventHandler<DomainRegisteredEvent>>();
                await domainRegisteredHandler.HandleAsync(e);
                break;

            case DomainExpiredEvent e:
                var domainExpiredHandler = scope.ServiceProvider
                    .GetRequiredService<IDomainEventHandler<DomainExpiredEvent>>();
                await domainExpiredHandler.HandleAsync(e);
                break;

            case OrderActivatedEvent e:
                var orderActivatedHandler = scope.ServiceProvider
                    .GetRequiredService<IDomainEventHandler<OrderActivatedEvent>>();
                await orderActivatedHandler.HandleAsync(e);
                break;

            case InvoicePaidEvent e:
                var invoicePaidHandler = scope.ServiceProvider
                    .GetRequiredService<IDomainEventHandler<InvoicePaidEvent>>();
                await invoicePaidHandler.HandleAsync(e);
                break;

            // Add more handlers as needed
            default:
                _log.Information("No handler registered for event type {EventType}", 
                    domainEvent.EventType);
                break;
        }
    }
}
