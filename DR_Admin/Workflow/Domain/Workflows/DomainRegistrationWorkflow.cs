using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.Workflow.Domain.Events;
using ISPAdmin.Workflow.Domain.Events.DomainEvents;
using ISPAdmin.Workflow.Domain.Events.OrderEvents;
using ISPAdmin.Workflow.Domain.StateMachines;
using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.EntityFrameworkCore;
using DomainRegistrationLib.Interfaces;
using DomainRegistrationLib.Models;
using DomainRegistrationLib.Factories;
using RegisteredDomainEntity = ISPAdmin.Data.Entities.RegisteredDomain;
using ISPAdmin.Workflow.Domain.Services;
using System.Text.Json;

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
    private static readonly ContactRoleType[] RequiredContactRoles =
    [
        ContactRoleType.Registrant,
        ContactRoleType.Administrative,
        ContactRoleType.Technical,
        ContactRoleType.Billing
    ];

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
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = null;

        try
        {
            _log.Information("Processing payment received callback for order {OrderId}", orderId);

            transaction = await _context.Database.BeginTransactionAsync();

            var order = await _context.Orders
                .Include(o => o.Service)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return WorkflowResult.Failed($"Order with ID {orderId} not found");
            }

            if (!order.ServiceId.HasValue)
            {
                return WorkflowResult.Failed("Order does not have an associated service");
            }

            var registrar = await _context.Registrars.FindAsync(
                await GetRegistrarIdFromServiceAsync(order.ServiceId.Value));

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
            var domain = new RegisteredDomainEntity
            {
                Name = order.Service.Name,
                CustomerId = order.CustomerId,
                ServiceId = order.ServiceId,
                RegistrarId = registrar.Id,
                Status = DomainStatus.Active.ToString(),
                RegistrationStatus = DomainRegistrationStatus.Registered,
                RegistrationDate = DateTime.UtcNow,
                RegistrationAttemptCount = 1,
                LastRegistrationAttemptUtc = DateTime.UtcNow,
                NextRegistrationAttemptUtc = null,
                RegistrationError = null,
                ExpirationDate = simulatedExpirationDate, // Use simulated date
                AutoRenew = order.AutoRenew,
                PrivacyProtection = false,
                RegistrationPrice = order.RecurringAmount,
                RenewalPrice = order.RecurringAmount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.RegisteredDomains.Add(domain);

            // Step 3: Update order status
            order.Status = OrderStatus.Active;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Step 4: Ensure related domain data is initialized
            await EnsurePostRegistrationDataAsync(domain);

            // Step 5: Link sold products in this order to the newly created domain
            await LinkSoldProductsToRegisteredDomainAsync(order.Id, domain.Id, order.CustomerId, domain.Name);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Step 6: Publish domain registered event
            await _eventPublisher.PublishAsync(new DomainRegisteredEvent
            {
                AggregateId = domain.Id,
                DomainName = domain.Name,
                CustomerId = domain.CustomerId,
                RegistrarId = domain.RegistrarId,
                ExpirationDate = domain.ExpirationDate!.Value,
                RegistrationPrice = domain.RegistrationPrice ?? 0,
                AutoRenew = domain.AutoRenew
            });

            // Step 7: Publish order activated event
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
            if (transaction != null)
            {
                await transaction.RollbackAsync();
            }

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
        finally
        {
            if (transaction != null)
            {
                await transaction.DisposeAsync();
            }
        }
    }

    private async Task LinkSoldProductsToRegisteredDomainAsync(int orderId, int registeredDomainId, int customerId, string domainName)
    {
        var soldHosting = await _context.SoldHostingPackages
            .Where(x => x.OrderId == orderId && !x.RegisteredDomainId.HasValue)
            .ToListAsync();

        foreach (var item in soldHosting)
        {
            item.RegisteredDomainId = registeredDomainId;
            item.UpdatedAt = DateTime.UtcNow;
        }

        var soldServices = await _context.SoldOptionalServices
            .Where(x => x.OrderId == orderId && !x.RegisteredDomainId.HasValue)
            .ToListAsync();

        foreach (var item in soldServices)
        {
            item.RegisteredDomainId = registeredDomainId;
            item.UpdatedAt = DateTime.UtcNow;
        }

        var candidateHosting = await _context.SoldHostingPackages
            .Where(x => x.CustomerId == customerId && !x.RegisteredDomainId.HasValue)
            .ToListAsync();

        foreach (var item in candidateHosting)
        {
            if (!string.Equals(TryResolveConnectedDomainNameFromNotes(item.Notes), domainName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            item.RegisteredDomainId = registeredDomainId;
            item.UpdatedAt = DateTime.UtcNow;
        }

        var candidateServices = await _context.SoldOptionalServices
            .Where(x => x.CustomerId == customerId && !x.RegisteredDomainId.HasValue)
            .ToListAsync();

        foreach (var item in candidateServices)
        {
            if (!string.Equals(TryResolveConnectedDomainNameFromNotes(item.Notes), domainName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            item.RegisteredDomainId = registeredDomainId;
            item.UpdatedAt = DateTime.UtcNow;
        }
    }

    private static string TryResolveConnectedDomainNameFromNotes(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
        {
            return string.Empty;
        }

        try
        {
            using var document = JsonDocument.Parse(notes);
            var root = document.RootElement;

            if (root.TryGetProperty("connectedDomainName", out var connectedDomain) && connectedDomain.ValueKind == JsonValueKind.String)
            {
                return connectedDomain.GetString() ?? string.Empty;
            }

            if (root.TryGetProperty("domainName", out var domainName) && domainName.ValueKind == JsonValueKind.String)
            {
                return domainName.GetString() ?? string.Empty;
            }
        }
        catch
        {
        }

        return string.Empty;
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

    private async Task EnsurePostRegistrationDataAsync(RegisteredDomainEntity domain)
    {
        if (domain.Customer == null)
        {
            domain.Customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == domain.CustomerId);
        }

        await EnsureDefaultDnsRecordsAsync(domain.Id);
        await EnsureDomainContactsAndAssignmentsAsync(domain);
    }

    private async Task EnsureDefaultDnsRecordsAsync(int domainId)
    {
        var hasDnsRecords = await _context.DnsRecords
            .AnyAsync(r => r.DomainId == domainId && !r.IsDeleted);

        if (hasDnsRecords)
        {
            return;
        }

        var package = await _context.DnsZonePackages
            .Include(p => p.Records)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.IsDefault)
            .ThenBy(p => p.SortOrder)
            .ThenBy(p => p.Id)
            .FirstOrDefaultAsync();

        if (package == null || package.Records.Count == 0)
        {
            return;
        }

        foreach (var templateRecord in package.Records)
        {
            _context.DnsRecords.Add(new DnsRecord
            {
                DomainId = domainId,
                DnsRecordTypeId = templateRecord.DnsRecordTypeId,
                Name = templateRecord.Name,
                Value = templateRecord.Value,
                TTL = templateRecord.TTL,
                Priority = templateRecord.Priority,
                Weight = templateRecord.Weight,
                Port = templateRecord.Port,
                IsPendingSync = true,
                IsDeleted = false
            });
        }
    }

    private async Task EnsureDomainContactsAndAssignmentsAsync(RegisteredDomainEntity domain)
    {
        var activeContactPeople = await _context.ContactPersons
            .Where(cp => cp.CustomerId == domain.CustomerId && cp.IsActive)
            .OrderByDescending(cp => cp.IsPrimary)
            .ThenBy(cp => cp.Id)
            .ToListAsync();

        if (activeContactPeople.Count == 0)
        {
            var fallbackContactPerson = await EnsureFallbackContactPersonAsync(domain);
            activeContactPeople.Add(fallbackContactPerson);
        }

        var existingAssignmentRoles = await _context.DomainContactAssignments
            .Where(a => a.RegisteredDomainId == domain.Id && a.IsActive)
            .Select(a => a.RoleType)
            .Distinct()
            .ToListAsync();

        var existingContactRoles = await _context.DomainContacts
            .Where(c => c.DomainId == domain.Id && c.IsCurrentVersion)
            .Select(c => c.RoleType)
            .Distinct()
            .ToListAsync();

        var assignmentRoleSet = existingAssignmentRoles.ToHashSet();
        var contactRoleSet = existingContactRoles.ToHashSet();

        foreach (var role in RequiredContactRoles)
        {
            var selectedContactPerson = SelectContactPersonForRole(activeContactPeople, role);

            if (selectedContactPerson != null && !assignmentRoleSet.Contains(role))
            {
                _context.DomainContactAssignments.Add(new DomainContactAssignment
                {
                    RegisteredDomainId = domain.Id,
                    ContactPersonId = selectedContactPerson.Id,
                    RoleType = role,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true
                });

                assignmentRoleSet.Add(role);
            }

            if (contactRoleSet.Contains(role))
            {
                continue;
            }

            _context.DomainContacts.Add(BuildDomainContact(domain, selectedContactPerson, role));
            contactRoleSet.Add(role);
        }
    }

    private async Task<ContactPerson> EnsureFallbackContactPersonAsync(RegisteredDomainEntity domain)
    {
        var existingContactPerson = await _context.ContactPersons
            .Where(cp => cp.CustomerId == domain.CustomerId)
            .OrderByDescending(cp => cp.IsPrimary)
            .ThenBy(cp => cp.Id)
            .FirstOrDefaultAsync();

        if (existingContactPerson != null)
        {
            if (!existingContactPerson.IsActive)
            {
                existingContactPerson.IsActive = true;
            }

            return existingContactPerson;
        }

        var firstName = "Customer";
        var lastName = "User";
        var fullName = domain.Customer?.Name?.Trim();

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            var split = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (split.Length > 0)
            {
                firstName = split[0];
                lastName = split.Length > 1 ? string.Join(' ', split.Skip(1)) : "User";
            }
        }

        var contactPerson = new ContactPerson
        {
            CustomerId = domain.CustomerId,
            FirstName = firstName,
            LastName = lastName,
            Email = domain.Customer?.Email ?? string.Empty,
            Phone = domain.Customer?.Phone ?? string.Empty,
            IsPrimary = true,
            IsActive = true,
            IsDefaultOwner = true,
            IsDefaultAdministrator = true,
            IsDefaultTech = true,
            IsDefaultBilling = true,
            IsDomainGlobal = true,
            Notes = $"Auto-created from domain registration for {domain.Name}"
        };

        _context.ContactPersons.Add(contactPerson);
        await _context.SaveChangesAsync();

        return contactPerson;
    }

    private static ContactPerson? SelectContactPersonForRole(
        IReadOnlyList<ContactPerson> contactPeople,
        ContactRoleType role)
    {
        if (contactPeople.Count == 0)
        {
            return null;
        }

        return role switch
        {
            ContactRoleType.Registrant => contactPeople.FirstOrDefault(cp => cp.IsDefaultOwner)
                ?? contactPeople.FirstOrDefault(cp => cp.IsPrimary)
                ?? contactPeople[0],
            ContactRoleType.Administrative => contactPeople.FirstOrDefault(cp => cp.IsDefaultAdministrator)
                ?? contactPeople.FirstOrDefault(cp => cp.IsDefaultOwner)
                ?? contactPeople.FirstOrDefault(cp => cp.IsPrimary)
                ?? contactPeople[0],
            ContactRoleType.Technical => contactPeople.FirstOrDefault(cp => cp.IsDefaultTech)
                ?? contactPeople.FirstOrDefault(cp => cp.IsDefaultAdministrator)
                ?? contactPeople.FirstOrDefault(cp => cp.IsPrimary)
                ?? contactPeople[0],
            ContactRoleType.Billing => contactPeople.FirstOrDefault(cp => cp.IsDefaultBilling)
                ?? contactPeople.FirstOrDefault(cp => cp.IsDefaultOwner)
                ?? contactPeople.FirstOrDefault(cp => cp.IsPrimary)
                ?? contactPeople[0],
            _ => contactPeople[0]
        };
    }

    private static DomainContact BuildDomainContact(
        RegisteredDomainEntity domain,
        ContactPerson? selectedContactPerson,
        ContactRoleType role)
    {
        var firstName = selectedContactPerson?.FirstName;
        var lastName = selectedContactPerson?.LastName;

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            firstName = "Customer";
            lastName = "User";
            var fullName = domain.Customer?.Name?.Trim();
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var split = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (split.Length > 0)
                {
                    firstName = split[0];
                    lastName = split.Length > 1 ? string.Join(' ', split.Skip(1)) : "User";
                }
            }
        }

        return new DomainContact
        {
            DomainId = domain.Id,
            RoleType = role,
            FirstName = firstName,
            LastName = lastName,
            Email = selectedContactPerson?.Email ?? domain.Customer?.Email ?? string.Empty,
            Phone = selectedContactPerson?.Phone ?? domain.Customer?.Phone ?? string.Empty,
            Organization = selectedContactPerson?.Department,
            Address1 = string.Empty,
            City = string.Empty,
            PostalCode = string.Empty,
            CountryCode = "US",
            IsActive = selectedContactPerson?.IsActive ?? true,
            SourceContactPersonId = selectedContactPerson?.Id,
            NeedsSync = true,
            IsCurrentVersion = true,
            IsPrivacyProtected = domain.PrivacyProtection
        };
    }
}

