using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.Domain.Events;
using ISPAdmin.Domain.Events.DomainEvents;
using ISPAdmin.Domain.Events.OrderEvents;
using ISPAdmin.Domain.StateMachines;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.EntityFrameworkCore;
using DomainRegistrationLib.Interfaces;
using DomainRegistrationLib.Models;
using DomainRegistrationLib.Factories;
using DomainEntity = ISPAdmin.Data.Entities.Domain;
using ISPAdmin.Workflow.Domain.Services;

namespace ISPAdmin.Workflow.Domain.Workflows;

/// <summary>
/// Orchestrates the complete domain registration workflow
/// </summary>
public class DomainRegistrationWorkflow : IDomainRegistrationWorkflow
{
    private readonly ApplicationDbContext _context;
    private readonly IDomainEventPublisher _eventPublisher;
    private readonly IInvoiceService _invoiceService;
    private readonly IServiceService _serviceService;
    private readonly IOrderService _orderService;
    private readonly DomainRegistrarFactory _registrarFactory;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<DomainRegistrationWorkflow>();

    public DomainRegistrationWorkflow(
        ApplicationDbContext context,
        IDomainEventPublisher eventPublisher,
        IInvoiceService invoiceService,
        IServiceService serviceService,
        IOrderService orderService,
        DomainRegistrarFactory registrarFactory)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _invoiceService = invoiceService;
        _serviceService = serviceService;
        _orderService = orderService;
        _registrarFactory = registrarFactory;
    }

    public async Task<WorkflowResult> ExecuteAsync(DomainRegistrationWorkflowInput input)
    {
        var correlationId = Guid.NewGuid().ToString();

        try
        {
            _log.Information("Starting domain registration workflow for domain: {DomainName}", input.DomainName);

            // Step 1: Get registrar
            var registrar = await _context.Registrars
                .FirstOrDefaultAsync(r => r.Id == input.RegistrarId);

            if (registrar == null)
            {
                return WorkflowResult.Failed($"Registrar with ID {input.RegistrarId} not found");
            }

            // Step 2: Check domain availability (TODO: Enable when registrar is configured)
            // var domainRegistrar = _registrarFactory.CreateRegistrar();
            // var availability = await domainRegistrar.CheckAvailabilityAsync(input.DomainName);
            // if (!availability.IsAvailable)
            // {
            //     return WorkflowResult.Failed($"Domain {input.DomainName} is not available for registration");
            // }
            
            // For now, assume domain is available
            _log.Information("Skipping availability check - registrar not configured");

            // Step 3: Create or get service
            int serviceId;
            if (input.ServiceId.HasValue)
            {
                serviceId = input.ServiceId.Value;
            }
            else
            {
                var service = await CreateDomainServiceAsync(input);
                serviceId = service.Id;
            }

            // Step 4: Create order
            var orderDto = await _orderService.CreateOrderAsync(new CreateOrderDto
            {
                CustomerId = input.CustomerId,
                ServiceId = serviceId,
                OrderType = input.OrderType,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddYears(input.Years),
                NextBillingDate = DateTime.UtcNow.AddYears(input.Years),
                SetupFee = 0,
                RecurringAmount = await GetDomainPriceAsync(input.DomainName, input.RegistrarId, input.Years),
                AutoRenew = input.AutoRenew
            });

            // Step 5: Generate invoice
            var invoice = await GenerateInvoiceForOrderAsync(orderDto.Id);

            // Step 6: Publish OrderCreated event
            await _eventPublisher.PublishAsync(new OrderCreatedEvent
            {
                AggregateId = orderDto.Id,
                CorrelationId = correlationId,
                OrderNumber = orderDto.OrderNumber,
                CustomerId = input.CustomerId,
                ServiceId = serviceId,
                InvoiceId = invoice.Id,
                OrderType = input.OrderType,
                TotalAmount = invoice.TotalAmount
            });

            _log.Information("Domain registration workflow initiated successfully for order {OrderNumber}", 
                orderDto.OrderNumber);

            return WorkflowResult.Success(orderDto.Id, 
                "Order created successfully. Awaiting payment.", 
                correlationId);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Domain registration workflow failed for domain {DomainName}", input.DomainName);
            
            await _eventPublisher.PublishAsync(new WorkflowFailedEvent
            {
                CorrelationId = correlationId,
                WorkflowType = "DomainRegistration",
                Reason = ex.Message,
                StackTrace = ex.StackTrace
            });

            return WorkflowResult.Failed(ex.Message, correlationId);
        }
    }

    public async Task<WorkflowResult> OnPaymentReceivedAsync(int orderId, int invoiceId)
    {
        try
        {
            _log.Information("Processing payment received callback for order {OrderId}", orderId);

            var order = await _context.Orders
                .Include(o => o.Service)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return WorkflowResult.Failed($"Order with ID {orderId} not found");
            }

            var registrar = await _context.Registrars.FindAsync(
                await GetRegistrarIdFromServiceAsync(order.ServiceId));

            if (registrar == null)
            {
                return WorkflowResult.Failed("Registrar configuration not found");
            }

            // Step 1: Register domain with registrar (TODO: Enable when configured)
            // var domainRegistrar = _registrarFactory.CreateRegistrar();
            // var registrationRequest = await BuildRegistrationRequestAsync(order);
            // var registrationResult = await domainRegistrar.RegisterDomainAsync(registrationRequest);
            // if (!registrationResult.Success)
            // {
            //     throw new Exception($"Domain registration failed: {registrationResult.ErrorMessage}");
            // }
            
            // For now, simulate successful registration
            _log.Information("Simulating domain registration - registrar not configured");
            var simulatedExpirationDate = DateTime.UtcNow.AddYears(1);

            // Step 2: Create domain entity
            var domain = new DomainEntity
            {
                Name = order.Service.Name,
                CustomerId = order.CustomerId,
                ServiceId = order.ServiceId,
                RegistrarId = registrar.Id,
                Status = DomainStatus.Active.ToString(),
                RegistrationDate = DateTime.UtcNow,
                ExpirationDate = simulatedExpirationDate, // Use simulated date
                AutoRenew = order.AutoRenew,
                PrivacyProtection = false,
                RegistrationPrice = order.RecurringAmount,
                RenewalPrice = order.RecurringAmount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Domains.Add(domain);

            // Step 3: Update order status
            order.Status = OrderStatus.Active;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Step 4: Publish domain registered event
            await _eventPublisher.PublishAsync(new DomainRegisteredEvent
            {
                AggregateId = domain.Id,
                DomainName = domain.Name,
                CustomerId = domain.CustomerId,
                RegistrarId = domain.RegistrarId,
                ExpirationDate = domain.ExpirationDate,
                RegistrationPrice = domain.RegistrationPrice ?? 0,
                AutoRenew = domain.AutoRenew
            });

            // Step 5: Publish order activated event
            await _eventPublisher.PublishAsync(new OrderActivatedEvent
            {
                AggregateId = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                ServiceId = order.ServiceId,
                ActivatedAt = DateTime.UtcNow
            });

            _log.Information("Domain {DomainName} registered successfully", domain.Name);

            return WorkflowResult.Success(domain.Id, "Domain registered successfully");
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Payment processing failed for order {OrderId}", orderId);

            // Update order status to suspended
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = OrderStatus.Suspended;
                order.Notes = $"Registration failed: {ex.Message}";
                await _context.SaveChangesAsync();

                await _eventPublisher.PublishAsync(new OrderSuspendedEvent
                {
                    AggregateId = order.Id,
                    OrderNumber = order.OrderNumber,
                    Reason = ex.Message
                });
            }

            return WorkflowResult.Failed(ex.Message);
        }
    }

    private async Task<Service> CreateDomainServiceAsync(DomainRegistrationWorkflowInput input)
    {
        var domainServiceType = await _context.ServiceTypes
            .FirstOrDefaultAsync(st => st.Name == "Domain Registration");

        if (domainServiceType == null)
        {
            domainServiceType = new ServiceType
            {
                Name = "Domain Registration",
                Description = "Domain registration and management",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.ServiceTypes.Add(domainServiceType);
            await _context.SaveChangesAsync();
        }

        var annualBillingCycle = await _context.BillingCycles
            .FirstOrDefaultAsync(bc => bc.Name == "Annually");

        var service = new Service
        {
            Name = input.DomainName,
            Description = $"Domain registration for {input.DomainName}",
            ServiceTypeId = domainServiceType.Id,
            BillingCycleId = annualBillingCycle?.Id ?? 1,
            Price = await GetDomainPriceAsync(input.DomainName, input.RegistrarId, input.Years),
            SetupFee = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return service;
    }

    private async Task<decimal> GetDomainPriceAsync(string domainName, int registrarId, int years)
    {
        // TODO: Implement pricing logic based on TLD and registrar
        // For now, return a default price
        return 12.99m * years;
    }

    private async Task<int> GetRegistrarIdFromServiceAsync(int serviceId)
    {
        // TODO: Get registrar from service metadata or configuration
        // For now, return default registrar
        var defaultRegistrar = await _context.Registrars.FirstOrDefaultAsync();
        return defaultRegistrar?.Id ?? 1;
    }

    private async Task<DomainRegistrationRequest> BuildRegistrationRequestAsync(Order order)
    {
        var customer = await _context.Customers.FindAsync(order.CustomerId);
        
        return new DomainRegistrationRequest
        {
            DomainName = order.Service.Name,
            Years = 1,
            AutoRenew = order.AutoRenew,
            PrivacyProtection = false,
            RegistrantContact = new ContactInformation
            {
                FirstName = customer?.Name.Split(' ').FirstOrDefault() ?? "First",
                LastName = customer?.Name.Split(' ').LastOrDefault() ?? "Last",
                Email = customer?.Email ?? "customer@example.com",
                Phone = customer?.Phone ?? "+1.0000000000",
                // Address components should be taken from CustomerAddress/PostalCode records
                Address1 = "123 Main St",
                City = "City",
                State = "State",
                PostalCode = "00000",
                Country = "US"
            }
        };
    }

    private async Task<InvoiceDto> GenerateInvoiceForOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Service)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Order {orderId} not found");
        }

        var invoiceNumber = await GenerateInvoiceNumberAsync();

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            CustomerId = order.CustomerId,
            Status = InvoiceStatus.Draft,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            SubTotal = order.SetupFee + order.RecurringAmount,
            TaxAmount = 0,
            TotalAmount = order.SetupFee + order.RecurringAmount,
            AmountDue = order.SetupFee + order.RecurringAmount,
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
            ServiceId = order.ServiceId,
            Description = $"Domain Registration - {order.Service.Name}",
            Quantity = 1,
            UnitPrice = order.RecurringAmount,
            TaxRate = 0,
            TaxAmount = 0,
            TotalPrice = order.RecurringAmount,
            TotalWithTax = order.RecurringAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.InvoiceLines.Add(invoiceLine);
        await _context.SaveChangesAsync();

        // Map to DTO
        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            Status = invoice.Status,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            TotalAmount = invoice.TotalAmount,
            AmountPaid = invoice.AmountPaid,
            AmountDue = invoice.AmountDue,
            CurrencyCode = invoice.CurrencyCode
        };
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
