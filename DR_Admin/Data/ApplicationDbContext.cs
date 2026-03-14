using Microsoft.EntityFrameworkCore;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.Utilities;
using OperatingSystem = ISPAdmin.Data.Entities.OperatingSystem;

namespace ISPAdmin.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public override int SaveChanges()
    {
        TrackRegisteredDomainHistories();
        AssignCustomerReferenceNumbers();
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        TrackRegisteredDomainHistories();
        await AssignCustomerReferenceNumbersAsync(cancellationToken);
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void TrackRegisteredDomainHistories()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => e.Entity is not RegisteredDomainHistory)
            .ToList();

        if (entries.Count == 0)
            return;

        var occurredAt = DateTime.UtcNow;
        var histories = new List<RegisteredDomainHistory>();

        foreach (var entry in entries)
        {
            switch (entry.Entity)
            {
                case RegisteredDomain domain:
                    AddRegisteredDomainLifecycleHistory(entry, domain, histories, occurredAt);
                    break;

                case DnsRecord dnsRecord:
                    histories.Add(CreateHistoryEntry(
                        dnsRecord.DomainId,
                        RegisteredDomainHistoryActionType.DnsChange,
                        $"DNS record {entry.State}",
                        $"Record '{dnsRecord.Name}' with value '{dnsRecord.Value}' was {entry.State.ToString().ToLowerInvariant()}.",
                        nameof(DnsRecord),
                        dnsRecord.Id,
                        occurredAt));
                    break;

                case DomainContact domainContact:
                    histories.Add(CreateHistoryEntry(
                        domainContact.DomainId,
                        RegisteredDomainHistoryActionType.ContactPersonChange,
                        $"Domain contact {entry.State}",
                        $"Contact '{domainContact.FirstName} {domainContact.LastName}' ({domainContact.RoleType}) was {entry.State.ToString().ToLowerInvariant()}.",
                        nameof(DomainContact),
                        domainContact.Id,
                        occurredAt));
                    break;

                case DomainContactAssignment domainContactAssignment:
                    histories.Add(CreateHistoryEntry(
                        domainContactAssignment.RegisteredDomainId,
                        RegisteredDomainHistoryActionType.ContactPersonChange,
                        $"Domain contact assignment {entry.State}",
                        $"Contact assignment for role '{domainContactAssignment.RoleType}' was {entry.State.ToString().ToLowerInvariant()}.",
                        nameof(DomainContactAssignment),
                        domainContactAssignment.Id,
                        occurredAt));
                    break;

                case PaymentTransaction paymentTransaction:
                    foreach (var domainId in ResolveRegisteredDomainIdsByInvoiceId(paymentTransaction.InvoiceId))
                    {
                        histories.Add(CreateHistoryEntry(
                            domainId,
                            RegisteredDomainHistoryActionType.Payment,
                            $"Payment transaction {entry.State}",
                            $"Payment transaction '{paymentTransaction.TransactionId}' ({paymentTransaction.Status}) was {entry.State.ToString().ToLowerInvariant()} with amount {paymentTransaction.Amount} {paymentTransaction.CurrencyCode}.",
                            nameof(PaymentTransaction),
                            paymentTransaction.Id,
                            occurredAt));
                    }
                    break;

                case SentEmail sentEmail:
                    AddSentEmailHistory(entry, sentEmail, histories, occurredAt);
                    break;
            }
        }

        if (histories.Count > 0)
        {
            RegisteredDomainHistories.AddRange(histories);
        }
    }

    private void AddRegisteredDomainLifecycleHistory(
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry,
        RegisteredDomain domain,
        List<RegisteredDomainHistory> histories,
        DateTime occurredAt)
    {
        if (entry.State == EntityState.Added)
        {
            histories.Add(new RegisteredDomainHistory
            {
                RegisteredDomain = domain,
                ActionType = RegisteredDomainHistoryActionType.Registration,
                Action = "Domain registered",
                Details = $"Domain '{domain.Name}' was registered in the system.",
                SourceEntityType = nameof(RegisteredDomain),
                SourceEntityId = domain.Id > 0 ? domain.Id : null,
                OccurredAt = occurredAt,
                CreatedAt = occurredAt,
                UpdatedAt = occurredAt
            });

            return;
        }

        if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
        {
            histories.Add(CreateHistoryEntry(
                domain.Id,
                RegisteredDomainHistoryActionType.DomainChange,
                $"Domain {entry.State}",
                $"Domain '{domain.Name}' was {entry.State.ToString().ToLowerInvariant()}.",
                nameof(RegisteredDomain),
                domain.Id,
                occurredAt));
        }
    }

    private void AddSentEmailHistory(
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry,
        SentEmail sentEmail,
        List<RegisteredDomainHistory> histories,
        DateTime occurredAt)
    {
        var wasSent = sentEmail.Status == EmailStatus.Sent;
        var statusProperty = entry.Property(nameof(SentEmail.Status));
        var changedToSent = entry.State == EntityState.Modified
            && statusProperty.IsModified
            && string.Equals(sentEmail.Status, EmailStatus.Sent, StringComparison.OrdinalIgnoreCase);

        if (entry.State == EntityState.Added && !wasSent)
            return;

        if (entry.State == EntityState.Modified && !changedToSent)
            return;

        foreach (var domainId in ResolveRegisteredDomainIdsForRelatedEntity(sentEmail.RelatedEntityType, sentEmail.RelatedEntityId))
        {
            histories.Add(CreateHistoryEntry(
                domainId,
                RegisteredDomainHistoryActionType.MessageSent,
                "Message sent",
                $"Email '{sentEmail.Subject}' was sent to '{sentEmail.To}'.",
                nameof(SentEmail),
                sentEmail.Id,
                occurredAt));
        }
    }

    private RegisteredDomainHistory CreateHistoryEntry(
        int registeredDomainId,
        RegisteredDomainHistoryActionType actionType,
        string action,
        string details,
        string sourceEntityType,
        int? sourceEntityId,
        DateTime occurredAt)
    {
        return new RegisteredDomainHistory
        {
            RegisteredDomainId = registeredDomainId,
            ActionType = actionType,
            Action = action,
            Details = details,
            SourceEntityType = sourceEntityType,
            SourceEntityId = sourceEntityId,
            OccurredAt = occurredAt,
            CreatedAt = occurredAt,
            UpdatedAt = occurredAt
        };
    }

    private List<int> ResolveRegisteredDomainIdsForRelatedEntity(string? relatedEntityType, int? relatedEntityId)
    {
        if (string.IsNullOrWhiteSpace(relatedEntityType) || !relatedEntityId.HasValue)
            return [];

        return relatedEntityType.Trim().ToLowerInvariant() switch
        {
            "registereddomain" => [relatedEntityId.Value],
            "dnsrecord" => DnsRecords
                .AsNoTracking()
                .Where(x => x.Id == relatedEntityId.Value)
                .Select(x => x.DomainId)
                .Distinct()
                .ToList(),
            "domaincontact" => DomainContacts
                .AsNoTracking()
                .Where(x => x.Id == relatedEntityId.Value)
                .Select(x => x.DomainId)
                .Distinct()
                .ToList(),
            "domaincontactassignment" => DomainContactAssignments
                .AsNoTracking()
                .Where(x => x.Id == relatedEntityId.Value)
                .Select(x => x.RegisteredDomainId)
                .Distinct()
                .ToList(),
            "invoice" => ResolveRegisteredDomainIdsByInvoiceId(relatedEntityId.Value),
            "order" => ResolveRegisteredDomainIdsByOrderId(relatedEntityId.Value),
            _ => []
        };
    }

    private List<int> ResolveRegisteredDomainIdsByInvoiceId(int invoiceId)
    {
        var orderId = Invoices
            .AsNoTracking()
            .Where(i => i.Id == invoiceId)
            .Select(i => i.OrderId)
            .FirstOrDefault();

        if (!orderId.HasValue)
            return [];

        return ResolveRegisteredDomainIdsByOrderId(orderId.Value);
    }

    private List<int> ResolveRegisteredDomainIdsByOrderId(int orderId)
    {
        var fromHostingPackages = SoldHostingPackages
            .AsNoTracking()
            .Where(x => x.OrderId == orderId && x.RegisteredDomainId.HasValue)
            .Select(x => x.RegisteredDomainId!.Value);

        var fromOptionalServices = SoldOptionalServices
            .AsNoTracking()
            .Where(x => x.OrderId == orderId && x.RegisteredDomainId.HasValue)
            .Select(x => x.RegisteredDomainId!.Value);

        return fromHostingPackages
            .Concat(fromOptionalServices)
            .Distinct()
            .ToList();
    }

    private void AssignCustomerReferenceNumbers()
    {
        const string settingKey = "PNR";
        const long defaultFirstReferenceNumber = 1001;

        var newCustomers = ChangeTracker.Entries<Customer>()
            .Where(e => e.State == EntityState.Added && e.Entity.ReferenceNumber <= 0)
            .Select(e => e.Entity)
            .ToList();

        if (newCustomers.Count == 0)
            return;

        var setting = SystemSettings.Local.FirstOrDefault(s => s.Key == settingKey)
            ?? SystemSettings.FirstOrDefault(s => s.Key == settingKey);

        var now = DateTime.UtcNow;
        long nextReferenceNumber;

        if (setting == null)
        {
            nextReferenceNumber = defaultFirstReferenceNumber;
            setting = new SystemSetting
            {
                Key = settingKey,
                Value = string.Empty,
                Description = "The next customer reference number (PNR) to assign. Auto-incremented on each new customer creation.",
                IsSystemKey = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            SystemSettings.Add(setting);
        }
        else if (!long.TryParse(setting.Value, out nextReferenceNumber) || nextReferenceNumber <= 0)
        {
            nextReferenceNumber = defaultFirstReferenceNumber;
        }

        foreach (var customer in newCustomers)
        {
            customer.ReferenceNumber = nextReferenceNumber;
            nextReferenceNumber++;
        }

        setting.Value = nextReferenceNumber.ToString();
        setting.UpdatedAt = now;
    }

    private async Task AssignCustomerReferenceNumbersAsync(CancellationToken cancellationToken)
    {
        const string settingKey = "PNR";
        const long defaultFirstReferenceNumber = 1001;

        var newCustomers = ChangeTracker.Entries<Customer>()
            .Where(e => e.State == EntityState.Added && e.Entity.ReferenceNumber <= 0)
            .Select(e => e.Entity)
            .ToList();

        if (newCustomers.Count == 0)
            return;

        var setting = SystemSettings.Local.FirstOrDefault(s => s.Key == settingKey)
            ?? await SystemSettings.FirstOrDefaultAsync(s => s.Key == settingKey, cancellationToken);

        var now = DateTime.UtcNow;
        long nextReferenceNumber;

        if (setting == null)
        {
            nextReferenceNumber = defaultFirstReferenceNumber;
            setting = new SystemSetting
            {
                Key = settingKey,
                Value = string.Empty,
                Description = "The next customer reference number (PNR) to assign. Auto-incremented on each new customer creation.",
                IsSystemKey = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            SystemSettings.Add(setting);
        }
        else if (!long.TryParse(setting.Value, out nextReferenceNumber) || nextReferenceNumber <= 0)
        {
            nextReferenceNumber = defaultFirstReferenceNumber;
        }

        foreach (var customer in newCustomers)
        {
            customer.ReferenceNumber = nextReferenceNumber;
            nextReferenceNumber++;
        }

        setting.Value = nextReferenceNumber.ToString();
        setting.UpdatedAt = now;
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<IEntityBase>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
                NormalizeEntity(entry.Entity);
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                NormalizeEntity(entry.Entity);
            }
        }
    }

    private void NormalizeEntity(object entity)
    {
        switch (entity)
        {
            case Country country:
                country.NormalizedEnglishName = NormalizationHelper.Normalize(country.EnglishName) ?? string.Empty;
                country.NormalizedLocalName = NormalizationHelper.Normalize(country.LocalName) ?? string.Empty;
                break;

            case Coupon coupon:
                coupon.NormalizedName = NormalizationHelper.Normalize(coupon.Name) ?? string.Empty;
                break;

            case Customer customer:
                // CountryCode/property removed; only normalize names
                customer.NormalizedName = NormalizationHelper.Normalize(customer.Name) ?? string.Empty;
                customer.NormalizedCustomerName = NormalizationHelper.Normalize(customer.CustomerName);
                break;

            case Entities.RegisteredDomain domain:
                domain.NormalizedName = NormalizationHelper.Normalize(domain.Name) ?? string.Empty;
                break;

            case HostingPackage hostingPackage:
                hostingPackage.NormalizedName = NormalizationHelper.Normalize(hostingPackage.Name) ?? string.Empty;
                break;

            case PaymentGateway paymentGateway:
                paymentGateway.NormalizedName = NormalizationHelper.Normalize(paymentGateway.Name) ?? string.Empty;
                break;

            case PaymentInstrument paymentInstrument:
                paymentInstrument.NormalizedCode = NormalizationHelper.Normalize(paymentInstrument.Code) ?? string.Empty;
                paymentInstrument.NormalizedName = NormalizationHelper.Normalize(paymentInstrument.Name) ?? string.Empty;
                break;

            case PostalCode postalCode:
                postalCode.NormalizedCode = NormalizationHelper.Normalize(postalCode.Code) ?? string.Empty;
                postalCode.NormalizedCountryCode = NormalizationHelper.Normalize(postalCode.CountryCode) ?? string.Empty;
                postalCode.NormalizedCity = NormalizationHelper.Normalize(postalCode.City) ?? string.Empty;
                postalCode.NormalizedState = NormalizationHelper.Normalize(postalCode.State);
                postalCode.NormalizedRegion = NormalizationHelper.Normalize(postalCode.Region);
                postalCode.NormalizedDistrict = NormalizationHelper.Normalize(postalCode.District);
                break;

            case Registrar registrar:
                registrar.NormalizedName = NormalizationHelper.Normalize(registrar.Name) ?? string.Empty;
                break;

            case Role role:
                role.Code = NormalizationHelper.Normalize(role.Code) ?? string.Empty;
                break;

            case SalesAgent salesAgent:
                salesAgent.NormalizedFirstName = NormalizationHelper.Normalize(salesAgent.FirstName) ?? string.Empty;
                salesAgent.NormalizedLastName = NormalizationHelper.Normalize(salesAgent.LastName) ?? string.Empty;
                break;

            case User user:
                user.NormalizedUsername = NormalizationHelper.Normalize(user.Username) ?? string.Empty;
                break;

            case Unit unit:
                unit.Code = NormalizationHelper.Normalize(unit.Code) ?? string.Empty;
                break;

            case ContactPerson contactPerson:
                contactPerson.NormalizedFirstName = NormalizationHelper.Normalize(contactPerson.FirstName) ?? string.Empty;
                contactPerson.NormalizedLastName = NormalizationHelper.Normalize(contactPerson.LastName) ?? string.Empty;
                break;

            case CustomerStatus customerStatus:
                customerStatus.NormalizedCode = NormalizationHelper.Normalize(customerStatus.Code) ?? string.Empty;
                customerStatus.NormalizedName = NormalizationHelper.Normalize(customerStatus.Name) ?? string.Empty;
                break;

            case Currency currency:
                currency.NormalizedCode = NormalizationHelper.Normalize(currency.Code) ?? string.Empty;
                currency.NormalizedName = NormalizationHelper.Normalize(currency.Name) ?? string.Empty;
                break;

            case AddressType addressType:
                addressType.NormalizedCode = NormalizationHelper.Normalize(addressType.Code) ?? string.Empty;
                addressType.NormalizedName = NormalizationHelper.Normalize(addressType.Name) ?? string.Empty;
                break;

            case DomainContact domainContact:
                domainContact.NormalizedFirstName = NormalizationHelper.Normalize(domainContact.FirstName) ?? string.Empty;
                domainContact.NormalizedLastName = NormalizationHelper.Normalize(domainContact.LastName) ?? string.Empty;
                domainContact.NormalizedEmail = NormalizationHelper.Normalize(domainContact.Email) ?? string.Empty;
                break;
        }
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerStatus> CustomerStatuses { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }
    public DbSet<CustomerInternalNote> CustomerInternalNotes { get; set; }
    public DbSet<CustomerChange> CustomerChanges { get; set; }
    public DbSet<AddressType> AddressTypes { get; set; }
    public DbSet<ContactPerson> ContactPersons { get; set; }
    public DbSet<RegistrarMailAddress> RegistrarMailAddresses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }
    public DbSet<BillingCycle> BillingCycles { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    public DbSet<SoldHostingPackage> SoldHostingPackages { get; set; }
    public DbSet<SoldOptionalService> SoldOptionalServices { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLine> InvoiceLines { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<PaymentGateway> PaymentGateways { get; set; }
    public DbSet<PaymentInstrument> PaymentInstruments { get; set; }
    public DbSet<PaymentInstrumentGateway> PaymentInstrumentGateways { get; set; }
    public DbSet<RegisteredDomain> RegisteredDomains { get; set; }
    public DbSet<RegisteredDomainHistory> RegisteredDomainHistories { get; set; }
    public DbSet<DomainContact> DomainContacts { get; set; }
    public DbSet<DomainContactAssignment> DomainContactAssignments { get; set; }
    public DbSet<Tld> Tlds { get; set; }
    public DbSet<TldRegistryRule> TldRegistryRules { get; set; }
    public DbSet<Registrar> Registrars { get; set; }
    public DbSet<RegistrarTld> RegistrarTlds { get; set; }
    public DbSet<DnsRecordType> DnsRecordTypes { get; set; }
    public DbSet<DnsRecord> DnsRecords { get; set; }
    public DbSet<NameServer> NameServers { get; set; }
    public DbSet<NameServerDomain> NameServerDomains { get; set; }
    public DbSet<DnsZonePackage> DnsZonePackages { get; set; }
    public DbSet<DnsZonePackageRecord> DnsZonePackageRecords { get; set; }
    public DbSet<DnsZonePackageControlPanel> DnsZonePackageControlPanels { get; set; }
    public DbSet<DnsZonePackageServer> DnsZonePackageServers { get; set; }
    public DbSet<HostingAccount> HostingAccounts { get; set; }
    public DbSet<HostingDomain> HostingDomains { get; set; }
    public DbSet<HostingEmailAccount> HostingEmailAccounts { get; set; }
    public DbSet<HostingDatabase> HostingDatabases { get; set; }
    public DbSet<HostingDatabaseUser> HostingDatabaseUsers { get; set; }
    public DbSet<HostingFtpAccount> HostingFtpAccounts { get; set; }
    public DbSet<Server> Servers { get; set; }
    public DbSet<ServerType> ServerTypes { get; set; }
    public DbSet<HostProvider> HostProviders { get; set; }
    public DbSet<OperatingSystem> OperatingSystems { get; set; }
    public DbSet<ServerIpAddress> ServerIpAddresses { get; set; }
    public DbSet<ControlPanelType> ControlPanelTypes { get; set; }
    public DbSet<ServerControlPanel> ServerControlPanels { get; set; }
    public DbSet<HostingPackage> HostingPackages { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<LoginHistory> LoginHistories { get; set; }
    public DbSet<SupportTicket> SupportTickets { get; set; }
    public DbSet<SupportTicketMessage> SupportTicketMessages { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<ProfitMarginSetting> ProfitMarginSettings { get; set; }
    public DbSet<MyCompany> MyCompanies { get; set; }
    public DbSet<BackupSchedule> BackupSchedules { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<PostalCode> PostalCodes { get; set; }
    public DbSet<ResellerCompany> ResellerCompanies { get; set; }
    public DbSet<SalesAgent> SalesAgents { get; set; }
    public DbSet<SentEmail> SentEmails { get; set; }
    public DbSet<DocumentTemplate> DocumentTemplates { get; set; }
    public DbSet<ReportTemplate> ReportTemplates { get; set; }
    
    // Sales and Payment Flow entities
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<QuoteLine> QuoteLines { get; set; }
    public DbSet<QuoteRevision> QuoteRevisions { get; set; }
    public DbSet<PaymentIntent> PaymentIntents { get; set; }
    public DbSet<CustomerPaymentMethod> CustomerPaymentMethods { get; set; }
    public DbSet<Refund> Refunds { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<CouponUsage> CouponUsages { get; set; }
    public DbSet<TaxRule> TaxRules { get; set; }
    public DbSet<TaxCategory> TaxCategories { get; set; }
    public DbSet<TaxDeterminationEvidence> TaxDeterminationEvidences { get; set; }
    public DbSet<TaxJurisdiction> TaxJurisdictions { get; set; }
    public DbSet<TaxRegistration> TaxRegistrations { get; set; }
    public DbSet<OrderTaxSnapshot> OrderTaxSnapshots { get; set; }
    public DbSet<CustomerCredit> CustomerCredits { get; set; }
    public DbSet<CreditTransaction> CreditTransactions { get; set; }
    
    // Recurring Billing entities
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SubscriptionBillingHistory> SubscriptionBillingHistories { get; set; }
    
    // Currency entities
    public DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; }
    public DbSet<ExchangeRateDownloadLog> ExchangeRateDownloadLogs { get; set; }
    
    // TLD Pricing entities
    public DbSet<RegistrarTldCostPricing> RegistrarTldCostPricing { get; set; }
    public DbSet<TldSalesPricing> TldSalesPricing { get; set; }
    public DbSet<ResellerTldDiscount> ResellerTldDiscounts { get; set; }
    public DbSet<RegistrarSelectionPreference> RegistrarSelectionPreferences { get; set; }
    public DbSet<RegistrarTldPriceDownloadSession> RegistrarTldPriceDownloadSessions { get; set; }
    public DbSet<RegistrarTldPriceChangeLog> RegistrarTldPriceChangeLogs { get; set; }
    
    // Financial tracking entities
    public DbSet<CustomerTaxProfile> CustomerTaxProfiles { get; set; }
    public DbSet<VendorCost> VendorCosts { get; set; }
    public DbSet<RefundLossAudit> RefundLossAudits { get; set; }
    public DbSet<VendorPayout> VendorPayouts { get; set; }
    public DbSet<VendorTaxProfile> VendorTaxProfiles { get; set; }
    
    // Payment processing entities
    public DbSet<PaymentAttempt> PaymentAttempts { get; set; }
    public DbSet<PaymentMethodToken> PaymentMethodTokens { get; set; }
    public DbSet<InvoicePayment> InvoicePayments { get; set; }
    
    // Domain Lifecycle Workflow entities
    public DbSet<OutboxEvent> OutboxEvents { get; set; }

    // Email communication entities
    public DbSet<CommunicationThread> CommunicationThreads { get; set; }
    public DbSet<CommunicationMessage> CommunicationMessages { get; set; }
    public DbSet<CommunicationParticipant> CommunicationParticipants { get; set; }
    public DbSet<CommunicationAttachment> CommunicationAttachments { get; set; }
    public DbSet<CommunicationStatusEvent> CommunicationStatusEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReferenceNumber);
            entity.Property(e => e.CustomerNumber);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.CountryCode).HasMaxLength(2);
            // Address fields removed from Customer entity
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.TaxId).HasMaxLength(50);
            entity.Property(e => e.VatNumber).HasMaxLength(50);
            entity.Property(e => e.IsSelfRegistered);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.BillingEmail).HasMaxLength(200);
            entity.Property(e => e.PreferredPaymentMethod).HasMaxLength(50);
            entity.Property(e => e.NormalizedName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NormalizedCustomerName).HasMaxLength(200);
            // ContactPerson removed; contact persons are stored in ContactPersons table

            entity.HasIndex(e => e.ReferenceNumber).IsUnique();
            entity.HasIndex(e => e.CustomerNumber).IsUnique().HasFilter(null);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.CountryCode);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.TaxId);
            entity.HasIndex(e => e.VatNumber);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsSelfRegistered);
            entity.HasIndex(e => e.NormalizedName);
            entity.HasIndex(e => e.NormalizedCustomerName);
            
            // Country relationship removed from Customer - addresses are managed via CustomerAddress -> PostalCode -> Country
            
            entity.HasOne(e => e.CustomerStatus)
                .WithMany()
                .HasForeignKey(e => e.CustomerStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TldRegistryRule configuration
        modelBuilder.Entity<TldRegistryRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(2000);

            entity.HasIndex(e => e.TldId);
            entity.HasIndex(e => e.IsActive);

            entity.HasOne(e => e.Tld)
                .WithMany(t => t.RegistryRules)
                .HasForeignKey(e => e.TldId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CustomerInternalNote configuration
        modelBuilder.Entity<CustomerInternalNote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Note).IsRequired().HasMaxLength(4000);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.InternalNotes)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // CustomerChange configuration
        modelBuilder.Entity<CustomerChange>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChangeType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FieldName).HasMaxLength(100);
            entity.Property(e => e.OldValue).HasMaxLength(2000);
            entity.Property(e => e.NewValue).HasMaxLength(2000);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.ChangedAt);
            entity.HasIndex(e => e.ChangeType);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Changes)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ChangedByUser)
                .WithMany()
                .HasForeignKey(e => e.ChangedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // CustomerStatus configuration
        modelBuilder.Entity<CustomerStatus>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Color).HasMaxLength(20);
            entity.Property(e => e.Priority);
            entity.Property(e => e.IsSystem);
            entity.Property(e => e.NormalizedCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.NormalizedName).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.NormalizedCode).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDefault);
            entity.HasIndex(e => e.SortOrder);
            entity.HasIndex(e => e.IsSystem);
        });

        // Currency configuration
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Symbol).HasMaxLength(10);
            entity.Property(e => e.NormalizedCode).IsRequired().HasMaxLength(3);
            entity.Property(e => e.NormalizedName).IsRequired().HasMaxLength(100);

            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.NormalizedCode).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDefault);
            entity.HasIndex(e => e.IsCustomerCurrency);
            entity.HasIndex(e => e.IsProviderCurrency);
            entity.HasIndex(e => e.SortOrder);
        });

        // AddressType configuration
        modelBuilder.Entity<AddressType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.NormalizedCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.NormalizedName).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.NormalizedCode).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDefault);
            entity.HasIndex(e => e.SortOrder);
        });

        // CustomerAddress configuration
        modelBuilder.Entity<CustomerAddress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AddressLine1).IsRequired().HasMaxLength(500);
            entity.Property(e => e.AddressLine2).HasMaxLength(500);
            entity.Property(e => e.AddressLine3).HasMaxLength(500);
            entity.Property(e => e.AddressLine4).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.AddressTypeId);
            entity.HasIndex(e => e.PostalCodeId);
            entity.HasIndex(e => e.IsPrimary);
            entity.HasIndex(e => e.IsActive);
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.CustomerAddresses)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.AddressType)
                .WithMany(at => at.CustomerAddresses)
                .HasForeignKey(e => e.AddressTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.PostalCode)
                .WithMany()
                .HasForeignKey(e => e.PostalCodeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ContactPerson configuration
        modelBuilder.Entity<ContactPerson>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.NormalizedFirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NormalizedLastName).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.IsPrimary);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.NormalizedFirstName, e.NormalizedLastName });
            
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RegistrarMailAddress configuration
        modelBuilder.Entity<RegistrarMailAddress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MailAddress).IsRequired().HasMaxLength(200);
            
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.IsDefault);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.MailAddress).IsUnique();
            
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NormalizedUsername).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AuthenticatorKey).HasMaxLength(200);
            
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.NormalizedUsername).IsUnique();
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Users)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        // UserRole configuration (Many-to-Many)
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Token configuration
        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TokenType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TokenValue).IsRequired();
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Tokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Service configuration
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            
            entity.HasOne(e => e.ServiceType)
                .WithMany(st => st.Services)
                .HasForeignKey(e => e.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.BillingCycle)
                .WithMany(bc => bc.Services)
                .HasForeignKey(e => e.BillingCycleId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.ResellerCompany)
                .WithMany(r => r.Services)
                .HasForeignKey(e => e.ResellerCompanyId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(e => e.SalesAgent)
                .WithMany(s => s.Services)
                .HasForeignKey(e => e.SalesAgentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ServiceType configuration
        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // BillingCycle configuration
        modelBuilder.Entity<BillingCycle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        // Subscription configuration
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasIndex(e => e.PaymentGatewayId);

            entity.HasOne(e => e.PaymentGateway)
                .WithMany()
                .HasForeignKey(e => e.PaymentGatewayId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Service)
                .WithMany(s => s.Orders)
                .HasForeignKey(e => e.ServiceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.OrderLines)
                .WithOne(ol => ol.Order)
                .HasForeignKey(ol => ol.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderLine configuration
        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.OrderId, e.LineNumber });
        });

        // SoldHostingPackage configuration
        modelBuilder.Entity<SoldHostingPackage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BillingCycle).IsRequired().HasMaxLength(20);
            entity.Property(e => e.SetupFee).HasPrecision(18, 2);
            entity.Property(e => e.RecurringPrice).HasPrecision(18, 2);
            entity.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3);
            entity.Property(e => e.ConfigurationSnapshotJson).HasMaxLength(4000);
            entity.Property(e => e.Notes).HasMaxLength(2000);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.HostingPackageId);
            entity.HasIndex(e => e.RegisteredDomainId);
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.NextBillingDate);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.SoldHostingPackages)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.HostingPackage)
                .WithMany(p => p.SoldHostingPackages)
                .HasForeignKey(e => e.HostingPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RegisteredDomain)
                .WithMany(d => d.SoldHostingPackages)
                .HasForeignKey(e => e.RegisteredDomainId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Order)
                .WithMany(o => o.SoldHostingPackages)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.OrderLine)
                .WithMany(ol => ol.SoldHostingPackages)
                .HasForeignKey(e => e.OrderLineId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // SoldOptionalService configuration
        modelBuilder.Entity<SoldOptionalService>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BillingCycle).IsRequired().HasMaxLength(20);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            entity.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3);
            entity.Property(e => e.ConfigurationSnapshotJson).HasMaxLength(4000);
            entity.Property(e => e.Notes).HasMaxLength(2000);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.ServiceId);
            entity.HasIndex(e => e.RegisteredDomainId);
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.NextBillingDate);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.SoldOptionalServices)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Service)
                .WithMany(s => s.SoldOptionalServices)
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RegisteredDomain)
                .WithMany(d => d.SoldOptionalServices)
                .HasForeignKey(e => e.RegisteredDomainId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Order)
                .WithMany(o => o.SoldOptionalServices)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.OrderLine)
                .WithMany(ol => ol.SoldOptionalServices)
                .HasForeignKey(e => e.OrderLineId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.OrderTaxSnapshotId);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CurrencyCode).HasMaxLength(3);
            entity.Property(e => e.TaxName).HasMaxLength(50);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.PaymentMethod).HasMaxLength(100);
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Order)
                .WithMany(o => o.Invoices)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.OrderTaxSnapshot)
                .WithMany()
                .HasForeignKey(e => e.OrderTaxSnapshotId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // InvoiceLine configuration
        modelBuilder.Entity<InvoiceLine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            entity.Property(e => e.Discount).HasPrecision(18, 2);
            entity.Property(e => e.TaxRate).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.TotalWithTax).HasPrecision(18, 2);
            entity.Property(e => e.ServiceNameSnapshot).HasMaxLength(200);
            entity.Property(e => e.AccountingCode).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasOne(e => e.Invoice)
                .WithMany(i => i.InvoiceLines)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(e => e.Unit)
                .WithMany(u => u.InvoiceLines)
                .HasForeignKey(e => e.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PaymentTransaction configuration
        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Invoice)
                .WithMany(i => i.PaymentTransactions)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Domain configuration
        modelBuilder.Entity<RegisteredDomain>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RegistrationStatus)
                .HasConversion<int>()
                .HasDefaultValue(DomainRegistrationStatus.PendingPayment);
            entity.Property(e => e.RegistrationError).HasMaxLength(2000);
            entity.Property(e => e.RegistrationPrice).HasPrecision(18, 2);
            entity.Property(e => e.RenewalPrice).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.NormalizedName).IsRequired().HasMaxLength(255);
            
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.RegistrationStatus);
            entity.HasIndex(e => e.ExpirationDate);
            entity.HasIndex(e => e.NextRegistrationAttemptUtc);
            entity.HasIndex(e => e.NormalizedName).IsUnique();
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.RegisteredDomains)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Registrar)
                .WithMany(r => r.RegisteredDomains)
                .HasForeignKey(e => e.RegistrarId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.RegistrarTld)
                .WithMany(rt => rt.RegisteredDomains)
                .HasForeignKey(e => e.RegistrarTldId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // RegisteredDomainHistory configuration
        modelBuilder.Entity<RegisteredDomainHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ActionType)
                .HasConversion<int>();
            entity.Property(e => e.Details).HasMaxLength(4000);
            entity.Property(e => e.SourceEntityType).HasMaxLength(200);

            entity.HasIndex(e => e.RegisteredDomainId);
            entity.HasIndex(e => e.ActionType);
            entity.HasIndex(e => e.OccurredAt);
            entity.HasIndex(e => new { e.RegisteredDomainId, e.OccurredAt });

            entity.HasOne(e => e.RegisteredDomain)
                .WithMany(d => d.RegisteredDomainHistories)
                .HasForeignKey(e => e.RegisteredDomainId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.PerformedByUser)
                .WithMany()
                .HasForeignKey(e => e.PerformedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // DomainContact configuration
        modelBuilder.Entity<DomainContact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Organization).HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Fax).HasMaxLength(50);
            entity.Property(e => e.Address1).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Address2).HasMaxLength(500);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.PostalCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CountryCode).IsRequired().HasMaxLength(2);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.NormalizedFirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NormalizedLastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NormalizedEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RegistrarContactId).HasMaxLength(200);
            entity.Property(e => e.RegistrarType).HasMaxLength(100);

            entity.HasIndex(e => e.DomainId);
            entity.HasIndex(e => e.RoleType);
            entity.HasIndex(e => e.SourceContactPersonId);
            entity.HasIndex(e => e.NeedsSync);
            entity.HasIndex(e => e.IsCurrentVersion);
            entity.HasIndex(e => new { e.DomainId, e.RoleType, e.IsCurrentVersion });
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.NormalizedEmail);

            entity.HasOne(e => e.Domain)
                .WithMany(d => d.DomainContacts)
                .HasForeignKey(e => e.DomainId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SourceContactPerson)
                .WithMany(cp => cp.SourcedDomainContacts)
                .HasForeignKey(e => e.SourceContactPersonId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // DomainContactAssignment configuration
        modelBuilder.Entity<DomainContactAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasIndex(e => e.RegisteredDomainId);
            entity.HasIndex(e => e.ContactPersonId);
            entity.HasIndex(e => new { e.RegisteredDomainId, e.RoleType, e.IsActive });
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.AssignedAt);

            entity.HasOne(e => e.RegisteredDomain)
                .WithMany(rd => rd.DomainContactAssignments)
                .HasForeignKey(e => e.RegisteredDomainId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ContactPerson)
                .WithMany(cp => cp.DomainContactAssignments)
                .HasForeignKey(e => e.ContactPersonId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // DnsRecordType configuration
        modelBuilder.Entity<DnsRecordType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsEditableByUser).HasDefaultValue(true);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.DefaultTTL).HasDefaultValue(3600);
            
            entity.HasIndex(e => e.Type).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        // TaxCategory configuration
        modelBuilder.Entity<TaxCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CountryCode).IsRequired().HasMaxLength(2);
            entity.Property(e => e.StateCode).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasIndex(e => new { e.CountryCode, e.StateCode, e.Code }).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CountryCode);
        });

        // DnsRecord configuration
        modelBuilder.Entity<DnsRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(1000);
            
            entity.HasIndex(e => e.DomainId);
            entity.HasIndex(e => e.DnsRecordTypeId);
            entity.HasIndex(e => new { e.DomainId, e.DnsRecordTypeId });
            
            entity.HasOne(e => e.Domain)
                .WithMany(d => d.DnsRecords)
                .HasForeignKey(e => e.DomainId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.DnsRecordType)
                .WithMany(t => t.DnsRecords)
                .HasForeignKey(e => e.DnsRecordTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // NameServer configuration
        modelBuilder.Entity<NameServer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Hostname).IsRequired().HasMaxLength(255);
            entity.Property(e => e.IpAddress).HasMaxLength(45); // To support IPv6
            
            entity.HasIndex(e => e.ServerId);
            entity.HasIndex(e => e.SortOrder);
            entity.HasIndex(e => e.Hostname);
            
            entity.HasOne(e => e.Server)
                .WithMany(s => s.NameServers)
                .HasForeignKey(e => e.ServerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // NameServerDomain configuration
        modelBuilder.Entity<NameServerDomain>(entity =>
        {
            entity.HasKey(e => new { e.NameServerId, e.DomainId });

            entity.HasIndex(e => e.DomainId);

            entity.HasOne(e => e.NameServer)
                .WithMany(ns => ns.NameServerDomains)
                .HasForeignKey(e => e.NameServerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Domain)
                .WithMany(d => d.NameServerDomains)
                .HasForeignKey(e => e.DomainId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DnsZonePackage configuration
        modelBuilder.Entity<DnsZonePackage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDefault);
            entity.HasIndex(e => e.SortOrder);
            
            entity.HasOne(e => e.ResellerCompany)
                .WithMany(r => r.DnsZonePackages)
                .HasForeignKey(e => e.ResellerCompanyId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(e => e.SalesAgent)
                .WithMany(s => s.DnsZonePackages)
                .HasForeignKey(e => e.SalesAgentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // DnsZonePackageRecord configuration
        modelBuilder.Entity<DnsZonePackageRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.ValueSourceType).IsRequired().HasMaxLength(50).HasDefaultValue(DnsTemplateValueSourceType.Manual);
            entity.Property(e => e.ValueSourceReference).HasMaxLength(250);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasIndex(e => e.DnsZonePackageId);
            entity.HasIndex(e => e.DnsRecordTypeId);

            entity.HasOne(e => e.DnsZonePackage)
                .WithMany(p => p.Records)
                .HasForeignKey(e => e.DnsZonePackageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.DnsRecordType)
                .WithMany()
                .HasForeignKey(e => e.DnsRecordTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // DnsZonePackageControlPanel configuration (M2M: DnsZonePackage <-> ServerControlPanel)
        modelBuilder.Entity<DnsZonePackageControlPanel>(entity =>
        {
            entity.HasKey(e => new { e.DnsZonePackageId, e.ServerControlPanelId });

            entity.HasIndex(e => e.DnsZonePackageId);
            entity.HasIndex(e => e.ServerControlPanelId);

            entity.HasOne(e => e.DnsZonePackage)
                .WithMany(p => p.ControlPanels)
                .HasForeignKey(e => e.DnsZonePackageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ServerControlPanel)
                .WithMany(cp => cp.DnsZonePackages)
                .HasForeignKey(e => e.ServerControlPanelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DnsZonePackageServer configuration (M2M: DnsZonePackage <-> Server)
        modelBuilder.Entity<DnsZonePackageServer>(entity =>
        {
            entity.HasKey(e => new { e.DnsZonePackageId, e.ServerId });

            entity.HasIndex(e => e.DnsZonePackageId);
            entity.HasIndex(e => e.ServerId);

            entity.HasOne(e => e.DnsZonePackage)
                .WithMany(p => p.Servers)
                .HasForeignKey(e => e.DnsZonePackageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Server)
                .WithMany(s => s.DnsZonePackages)
                .HasForeignKey(e => e.ServerId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // HostingAccount configuration
        modelBuilder.Entity<HostingAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Provider).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.HostingAccounts)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Server)
                .WithMany(s => s.HostingAccounts)
                .HasForeignKey(e => e.ServerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.ServerControlPanel)
                .WithMany()
                .HasForeignKey(e => e.ServerControlPanelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ServerType configuration
        modelBuilder.Entity<ServerType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        // HostProvider configuration
        modelBuilder.Entity<HostProvider>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.WebsiteUrl).HasMaxLength(500);

            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        // OperatingSystem configuration
        modelBuilder.Entity<OperatingSystem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Version).HasMaxLength(50);

            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        // Server configuration
        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(2000);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ServerTypeId);

            entity.HasOne(e => e.ServerType)
                .WithMany(st => st.Servers)
                .HasForeignKey(e => e.ServerTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.HostProvider)
                .WithMany(hp => hp.Servers)
                .HasForeignKey(e => e.HostProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.OperatingSystem)
                .WithMany(os => os.Servers)
                .HasForeignKey(e => e.OperatingSystemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ServerIpAddress configuration
        modelBuilder.Entity<ServerIpAddress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IpAddress).IsRequired().HasMaxLength(45); // IPv6 max length
            entity.Property(e => e.IpVersion).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AssignedTo).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasIndex(e => e.IpAddress);
            entity.HasIndex(e => e.ServerId);
            entity.HasIndex(e => e.Status);
            
            entity.HasOne(e => e.Server)
                .WithMany(s => s.IpAddresses)
                .HasForeignKey(e => e.ServerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ControlPanelType configuration
        modelBuilder.Entity<ControlPanelType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Version).HasMaxLength(50);
            entity.Property(e => e.WebsiteUrl).HasMaxLength(500);
            
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        // ServerControlPanel configuration
        modelBuilder.Entity<ServerControlPanel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ApiUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ApiToken).HasMaxLength(500);
            entity.Property(e => e.ApiKey).HasMaxLength(500);
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.AdditionalSettings).HasMaxLength(4000);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastError).HasMaxLength(2000);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            
            entity.HasIndex(e => e.ServerId);
            entity.HasIndex(e => e.ControlPanelTypeId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IpAddressId);

            entity.HasOne(e => e.Server)
                .WithMany(s => s.ControlPanels)
                .HasForeignKey(e => e.ServerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ControlPanelType)
                .WithMany(c => c.ServerControlPanels)
                .HasForeignKey(e => e.ControlPanelTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.IpAddress)
                .WithMany(ip => ip.ControlPanels)
                .HasForeignKey(e => e.IpAddressId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // HostingPackage configuration
        modelBuilder.Entity<HostingPackage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.MonthlyPrice).HasPrecision(18, 2);
            entity.Property(e => e.YearlyPrice).HasPrecision(18, 2);
            entity.Property(e => e.NormalizedName).IsRequired().HasMaxLength(200);
            
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.NormalizedName);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ActionType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityId).HasMaxLength(50);
            entity.Property(e => e.Details).HasMaxLength(2000);
            entity.Property(e => e.IPAddress).HasMaxLength(50);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // LoginHistory configuration
        modelBuilder.Entity<LoginHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Identifier).IsRequired().HasMaxLength(200);
            entity.Property(e => e.IPAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.FailureReason).HasMaxLength(500);

            entity.HasIndex(e => e.AttemptedAt);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsSuccessful);

            entity.HasOne(e => e.User)
                .WithMany(u => u.LoginHistories)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // SupportTicket configuration
        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Priority).IsRequired().HasMaxLength(20);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.LastMessageAt);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.SupportTickets)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany(u => u.CreatedSupportTickets)
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedToUser)
                .WithMany(u => u.AssignedSupportTickets)
                .HasForeignKey(e => e.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // SupportTicketMessage configuration
        modelBuilder.Entity<SupportTicketMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SenderRole).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(4000);

            entity.HasIndex(e => e.SupportTicketId);
            entity.HasIndex(e => e.SenderUserId);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.SupportTicket)
                .WithMany(t => t.Messages)
                .HasForeignKey(e => e.SupportTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SenderUser)
                .WithMany(u => u.SupportTicketMessages)
                .HasForeignKey(e => e.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // SystemSetting configuration
        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).HasMaxLength(1000);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsSystemKey).HasDefaultValue(false);
            entity.HasIndex(e => e.Key).IsUnique();
        });

        // ProfitMarginSetting configuration
        modelBuilder.Entity<ProfitMarginSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductClass).IsRequired();
            entity.Property(e => e.ProfitPercent).HasPrecision(7, 2);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasIndex(e => e.ProductClass).IsUnique();
        });

        // MyCompany configuration
        modelBuilder.Entity<MyCompany>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.LegalName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.AddressLine1).HasMaxLength(300);
            entity.Property(e => e.AddressLine2).HasMaxLength(300);
            entity.Property(e => e.PostalCode).HasMaxLength(50);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.CountryCode).HasMaxLength(2);
            entity.Property(e => e.OrganizationNumber).HasMaxLength(100);
            entity.Property(e => e.TaxId).HasMaxLength(100);
            entity.Property(e => e.VatNumber).HasMaxLength(100);
            entity.Property(e => e.InvoiceEmail).HasMaxLength(200);
            entity.Property(e => e.Website).HasMaxLength(300);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.LetterheadFooter).HasMaxLength(2000);
        });

        // BackupSchedule configuration
        modelBuilder.Entity<BackupSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DatabaseName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Frequency).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
        });

        // Country configuration
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(2);
            entity.Property(e => e.Tld).IsRequired().HasMaxLength(10);
            entity.Property(e => e.EnglishName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LocalName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NormalizedEnglishName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NormalizedLocalName).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Tld);
            entity.HasIndex(e => e.EnglishName);
            entity.HasIndex(e => e.NormalizedEnglishName);
        });

        // Tld configuration
        modelBuilder.Entity<Tld>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Extension).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasIndex(e => e.Extension).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        // Registrar configuration
        modelBuilder.Entity<Registrar>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ContactEmail).HasMaxLength(200);
            entity.Property(e => e.ContactPhone).HasMaxLength(50);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.NormalizedName).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDefault);
            entity.HasIndex(e => e.NormalizedName);
        });

        // RegistrarTld configuration
        modelBuilder.Entity<RegistrarTld>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasIndex(e => new { e.RegistrarId, e.TldId }).IsUnique();
            entity.HasIndex(e => e.IsActive);
            
            entity.HasOne(e => e.Registrar)
                .WithMany(r => r.RegistrarTlds)
                .HasForeignKey(e => e.RegistrarId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Tld)
                .WithMany(t => t.RegistrarTlds)
                .HasForeignKey(e => e.TldId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PostalCode configuration
        modelBuilder.Entity<PostalCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CountryCode).IsRequired().HasMaxLength(2);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Region).HasMaxLength(100);
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.Latitude).HasPrecision(10, 7);
            entity.Property(e => e.Longitude).HasPrecision(10, 7);
            entity.Property(e => e.NormalizedCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.NormalizedCountryCode).IsRequired().HasMaxLength(2);
            entity.Property(e => e.NormalizedCity).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NormalizedState).HasMaxLength(100);
            entity.Property(e => e.NormalizedRegion).HasMaxLength(100);
            entity.Property(e => e.NormalizedDistrict).HasMaxLength(100);
            
            entity.HasIndex(e => new { e.Code, e.CountryCode });
            entity.HasIndex(e => e.City);
            entity.HasIndex(e => new { e.NormalizedCode, e.NormalizedCountryCode });
            entity.HasIndex(e => e.NormalizedCity);
            
            entity.HasOne(e => e.Country)
                .WithMany(c => c.PostalCodes)
                .HasForeignKey(e => e.CountryCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Unit configuration
        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        // ResellerCompany configuration
        modelBuilder.Entity<ResellerCompany>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContactPerson).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.CountryCode).HasMaxLength(2);
            entity.Property(e => e.CompanyRegistrationNumber).HasMaxLength(100);
            entity.Property(e => e.TaxId).HasMaxLength(50);
            entity.Property(e => e.VatNumber).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Email);
        });

        // SalesAgent configuration
        modelBuilder.Entity<SalesAgent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.MobilePhone).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.NormalizedFirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NormalizedLastName).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => e.ResellerCompanyId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => new { e.NormalizedFirstName, e.NormalizedLastName });
            
            entity.HasOne(e => e.ResellerCompany)
                .WithMany(r => r.SalesAgents)
                .HasForeignKey(e => e.ResellerCompanyId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // SentEmail configuration
        modelBuilder.Entity<SentEmail>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.From).IsRequired().HasMaxLength(200);
            entity.Property(e => e.To).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Cc).HasMaxLength(1000);
            entity.Property(e => e.Bcc).HasMaxLength(1000);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            entity.Property(e => e.BodyText).HasMaxLength(int.MaxValue);
            entity.Property(e => e.BodyHtml).HasMaxLength(int.MaxValue);
            entity.Property(e => e.MessageId).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue(EmailStatus.Pending);
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            entity.Property(e => e.RetryCount).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.MaxRetries).IsRequired().HasDefaultValue(3);
            entity.Property(e => e.Provider).HasMaxLength(100);
            entity.Property(e => e.RelatedEntityType).HasMaxLength(100);
            entity.Property(e => e.Attachments).HasMaxLength(4000);
            
            entity.HasIndex(e => e.SentDate);
            entity.HasIndex(e => e.MessageId);
            entity.HasIndex(e => e.From);
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.NextAttemptAt);
            entity.HasIndex(e => new { e.Status, e.NextAttemptAt });
            entity.HasIndex(e => new { e.RelatedEntityType, e.RelatedEntityId });
            
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // CommunicationThread configuration
        modelBuilder.Entity<CommunicationThread>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            entity.Property(e => e.RelatedEntityType).HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue(CommunicationThreadStatus.Open);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.LastMessageAtUtc);
            entity.HasIndex(e => new { e.RelatedEntityType, e.RelatedEntityId });

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // CommunicationMessage configuration
        modelBuilder.Entity<CommunicationMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Direction).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ExternalMessageId).HasMaxLength(255);
            entity.Property(e => e.InternetMessageId).HasMaxLength(255);
            entity.Property(e => e.FromAddress).IsRequired().HasMaxLength(320);
            entity.Property(e => e.ToAddresses).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.CcAddresses).HasMaxLength(2000);
            entity.Property(e => e.BccAddresses).HasMaxLength(2000);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            entity.Property(e => e.BodyText).HasMaxLength(int.MaxValue);
            entity.Property(e => e.BodyHtml).HasMaxLength(int.MaxValue);
            entity.Property(e => e.Provider).HasMaxLength(100);
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasIndex(e => e.CommunicationThreadId);
            entity.HasIndex(e => e.SentEmailId);
            entity.HasIndex(e => e.ExternalMessageId);
            entity.HasIndex(e => e.InternetMessageId);
            entity.HasIndex(e => e.IsRead);
            entity.HasIndex(e => e.ReceivedAtUtc);
            entity.HasIndex(e => e.SentAtUtc);

            entity.HasOne(e => e.CommunicationThread)
                .WithMany(t => t.Messages)
                .HasForeignKey(e => e.CommunicationThreadId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SentEmail)
                .WithMany()
                .HasForeignKey(e => e.SentEmailId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // CommunicationParticipant configuration
        modelBuilder.Entity<CommunicationParticipant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmailAddress).IsRequired().HasMaxLength(320);
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);

            entity.HasIndex(e => e.CommunicationThreadId);
            entity.HasIndex(e => e.EmailAddress);
            entity.HasIndex(e => e.Role);
            entity.HasIndex(e => new { e.CommunicationThreadId, e.EmailAddress, e.Role });

            entity.HasOne(e => e.CommunicationThread)
                .WithMany(t => t.Participants)
                .HasForeignKey(e => e.CommunicationThreadId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CommunicationAttachment configuration
        modelBuilder.Entity<CommunicationAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(260);
            entity.Property(e => e.ContentType).HasMaxLength(200);
            entity.Property(e => e.StoragePath).HasMaxLength(1000);
            entity.Property(e => e.InlineContentId).HasMaxLength(255);

            entity.HasIndex(e => e.CommunicationMessageId);

            entity.HasOne(e => e.CommunicationMessage)
                .WithMany(m => m.Attachments)
                .HasForeignKey(e => e.CommunicationMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CommunicationStatusEvent configuration
        modelBuilder.Entity<CommunicationStatusEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Details).HasMaxLength(2000);
            entity.Property(e => e.Source).HasMaxLength(100);

            entity.HasIndex(e => e.CommunicationMessageId);
            entity.HasIndex(e => e.OccurredAtUtc);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.CommunicationMessage)
                .WithMany(m => m.StatusEvents)
                .HasForeignKey(e => e.CommunicationMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CurrencyExchangeRate configuration
        modelBuilder.Entity<CurrencyExchangeRate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BaseCurrency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.TargetCurrency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Rate).IsRequired().HasPrecision(18, 6);
            entity.Property(e => e.EffectiveRate).IsRequired().HasPrecision(18, 6);
            entity.Property(e => e.Markup).HasPrecision(5, 2);
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            // Indexes for performance
            entity.HasIndex(e => new { e.BaseCurrency, e.TargetCurrency, e.EffectiveDate })
                .HasDatabaseName("IX_CurrencyExchangeRates_BaseCurrency_TargetCurrency_EffectiveDate");
            entity.HasIndex(e => e.IsActive);
        });

        // PaymentGateway configuration
        modelBuilder.Entity<PaymentGateway>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProviderCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PaymentInstrument).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ApiKey).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ApiSecret).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ConfigurationJson).HasMaxLength(4000);
            entity.Property(e => e.WebhookUrl).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.NormalizedName).IsRequired().HasMaxLength(200);
            
            entity.HasIndex(e => e.ProviderCode);
            entity.HasIndex(e => e.PaymentInstrument);
            entity.HasIndex(e => e.PaymentInstrumentId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDefault);
            entity.HasIndex(e => e.NormalizedName);

            entity.HasOne(e => e.PaymentInstrumentEntity)
                .WithMany(i => i.PaymentGateways)
                .HasForeignKey(e => e.PaymentInstrumentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PaymentInstrument configuration
        modelBuilder.Entity<PaymentInstrument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NormalizedCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.NormalizedName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.NormalizedCode).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.DisplayOrder);
            entity.HasIndex(e => e.DefaultGatewayId);

            entity.HasOne(e => e.DefaultGateway)
                .WithMany()
                .HasForeignKey(e => e.DefaultGatewayId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PaymentInstrumentGateway configuration
        modelBuilder.Entity<PaymentInstrumentGateway>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.PaymentInstrumentId, e.PaymentGatewayId }).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDefault);
            entity.HasIndex(e => e.Priority);

            entity.HasOne(e => e.PaymentInstrument)
                .WithMany(i => i.PaymentGatewayMappings)
                .HasForeignKey(e => e.PaymentInstrumentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.PaymentGateway)
                .WithMany(g => g.PaymentInstrumentMappings)
                .HasForeignKey(e => e.PaymentGatewayId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Coupon configuration
        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Value).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.RecurringYears);
            entity.Property(e => e.MinimumAmount).HasPrecision(18, 2);
            entity.Property(e => e.MaximumDiscount).HasPrecision(18, 2);
            entity.Property(e => e.AllowedServiceTypeIdsJson).HasMaxLength(1000);
            entity.Property(e => e.InternalNotes).HasMaxLength(2000);
            entity.Property(e => e.NormalizedName).IsRequired().HasMaxLength(200);
            
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.ValidFrom);
            entity.HasIndex(e => e.ValidUntil);
            entity.HasIndex(e => e.NormalizedName);
        });

        // TaxDeterminationEvidence configuration
        modelBuilder.Entity<TaxDeterminationEvidence>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BuyerCountryCode).IsRequired().HasMaxLength(2);
            entity.Property(e => e.BuyerStateCode).HasMaxLength(20);
            entity.Property(e => e.BillingCountryCode).IsRequired().HasMaxLength(2);
            entity.Property(e => e.IpAddress).HasMaxLength(64);
            entity.Property(e => e.BuyerTaxId).HasMaxLength(100);
            entity.Property(e => e.VatValidationProvider).HasMaxLength(100);
            entity.Property(e => e.VatValidationRawResponse).HasMaxLength(4000);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.CapturedAt);
            entity.HasIndex(e => e.BuyerCountryCode);
        });

        // TaxJurisdiction configuration
        modelBuilder.Entity<TaxJurisdiction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CountryCode).IsRequired().HasMaxLength(2);
            entity.Property(e => e.StateCode).HasMaxLength(20);
            entity.Property(e => e.TaxAuthority).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TaxCurrencyCode).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Notes).HasMaxLength(2000);

            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.CountryCode);
            entity.HasIndex(e => new { e.CountryCode, e.StateCode });
            entity.HasIndex(e => e.IsActive);
        });

        // TaxRegistration configuration
        modelBuilder.Entity<TaxRegistration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LegalEntityName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(2000);

            entity.HasIndex(e => e.TaxJurisdictionId);
            entity.HasIndex(e => e.RegistrationNumber);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.TaxJurisdictionId, e.RegistrationNumber }).IsUnique();

            entity.HasOne(e => e.TaxJurisdiction)
                .WithMany(j => j.TaxRegistrations)
                .HasForeignKey(e => e.TaxJurisdictionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TaxRule configuration
        modelBuilder.Entity<TaxRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CountryCode).IsRequired().HasMaxLength(2);
            entity.Property(e => e.StateCode).HasMaxLength(20);
            entity.Property(e => e.TaxName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TaxCategory).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TaxRate).HasPrecision(18, 6);
            entity.Property(e => e.TaxAuthority).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TaxRegistrationNumber).HasMaxLength(100);
            entity.Property(e => e.InternalNotes).HasMaxLength(2000);

            entity.HasIndex(e => e.CountryCode);
            entity.HasIndex(e => new { e.CountryCode, e.StateCode });
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.EffectiveFrom);
            entity.HasIndex(e => e.EffectiveUntil);
            entity.HasIndex(e => e.TaxJurisdictionId);
            entity.HasIndex(e => e.TaxCategoryId);
            entity.HasIndex(e => e.TaxCategory);

            entity.HasOne(e => e.TaxJurisdiction)
                .WithMany(j => j.TaxRules)
                .HasForeignKey(e => e.TaxJurisdictionId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.TaxCategoryEntity)
                .WithMany(c => c.TaxRules)
                .HasForeignKey(e => e.TaxCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // OrderTaxSnapshot configuration
        modelBuilder.Entity<OrderTaxSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BuyerCountryCode).IsRequired().HasMaxLength(2);
            entity.Property(e => e.BuyerStateCode).HasMaxLength(20);
            entity.Property(e => e.BuyerTaxId).HasMaxLength(100);
            entity.Property(e => e.TaxCurrencyCode).IsRequired().HasMaxLength(3);
            entity.Property(e => e.DisplayCurrencyCode).IsRequired().HasMaxLength(3);
            entity.Property(e => e.ExchangeRate).HasPrecision(18, 8);
            entity.Property(e => e.NetAmount).HasPrecision(18, 6);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 6);
            entity.Property(e => e.GrossAmount).HasPrecision(18, 6);
            entity.Property(e => e.AppliedTaxRate).HasPrecision(18, 6);
            entity.Property(e => e.AppliedTaxName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RuleVersion).HasMaxLength(100);
            entity.Property(e => e.IdempotencyKey).HasMaxLength(100);
            entity.Property(e => e.CalculationInputsJson).HasMaxLength(8000);

            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.TaxJurisdictionId);
            entity.HasIndex(e => e.BuyerCountryCode);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.OrderId, e.IdempotencyKey }).IsUnique();
            entity.HasIndex(e => e.TaxDeterminationEvidenceId);

            entity.HasOne(e => e.Order)
                .WithMany(o => o.TaxSnapshots)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.TaxJurisdiction)
                .WithMany()
                .HasForeignKey(e => e.TaxJurisdictionId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.TaxDeterminationEvidence)
                .WithMany()
                .HasForeignKey(e => e.TaxDeterminationEvidenceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // CustomerTaxProfile configuration
        modelBuilder.Entity<CustomerTaxProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TaxIdNumber).HasMaxLength(100);
            entity.Property(e => e.TaxIdValidationResponse).HasMaxLength(4000);
            entity.Property(e => e.TaxResidenceCountry).IsRequired().HasMaxLength(2);
            entity.Property(e => e.TaxExemptionReason).HasMaxLength(500);
            entity.Property(e => e.TaxExemptionCertificateUrl).HasMaxLength(500);
            
            entity.HasIndex(e => e.CustomerId).IsUnique();
            entity.HasIndex(e => e.TaxIdNumber);
            entity.HasIndex(e => e.TaxResidenceCountry);
            
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // VendorCost configuration
        modelBuilder.Entity<VendorCost>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VendorName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.VendorCurrency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.VendorAmount).HasPrecision(18, 2);
            entity.Property(e => e.BaseCurrency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.BaseAmount).HasPrecision(18, 2);
            entity.Property(e => e.ExchangeRate).HasPrecision(18, 6);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            
            entity.HasIndex(e => e.InvoiceLineId);
            entity.HasIndex(e => e.VendorPayoutId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.VendorType);
            entity.HasIndex(e => e.IsRefundable);
            
            entity.HasOne(e => e.InvoiceLine)
                .WithMany(i => i.VendorCosts)
                .HasForeignKey(e => e.InvoiceLineId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.VendorPayout)
                .WithMany(p => p.VendorCosts)
                .HasForeignKey(e => e.VendorPayoutId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // RefundLossAudit configuration
        modelBuilder.Entity<RefundLossAudit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OriginalInvoiceAmount).HasPrecision(18, 2);
            entity.Property(e => e.RefundedAmount).HasPrecision(18, 2);
            entity.Property(e => e.VendorCostUnrecoverable).HasPrecision(18, 2);
            entity.Property(e => e.NetLoss).HasPrecision(18, 2);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.DenialReason).HasMaxLength(2000);
            entity.Property(e => e.InternalNotes).HasMaxLength(4000);
            
            entity.HasIndex(e => e.RefundId);
            entity.HasIndex(e => e.InvoiceId);
            entity.HasIndex(e => e.ApprovalStatus);
            entity.HasIndex(e => e.ApprovedAt);
            
            entity.HasOne(e => e.Refund)
                .WithMany()
                .HasForeignKey(e => e.RefundId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Invoice)
                .WithMany()
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.ApprovedByUser)
                .WithMany()
                .HasForeignKey(e => e.ApprovedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // VendorPayout configuration
        modelBuilder.Entity<VendorPayout>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VendorName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.VendorCurrency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.VendorAmount).HasPrecision(18, 2);
            entity.Property(e => e.BaseCurrency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.BaseAmount).HasPrecision(18, 2);
            entity.Property(e => e.ExchangeRate).HasPrecision(18, 6);
            entity.Property(e => e.FailureReason).HasMaxLength(2000);
            entity.Property(e => e.TransactionReference).HasMaxLength(200);
            entity.Property(e => e.PaymentGatewayResponse).HasMaxLength(4000);
            entity.Property(e => e.InterventionReason).HasMaxLength(2000);
            entity.Property(e => e.InternalNotes).HasMaxLength(4000);
            
            entity.HasIndex(e => e.VendorId);
            entity.HasIndex(e => e.VendorType);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ScheduledDate);
            entity.HasIndex(e => e.RequiresManualIntervention);
            
            entity.HasOne(e => e.InterventionResolvedByUser)
                .WithMany()
                .HasForeignKey(e => e.InterventionResolvedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // VendorTaxProfile configuration
        modelBuilder.Entity<VendorTaxProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TaxIdNumber).HasMaxLength(100);
            entity.Property(e => e.TaxResidenceCountry).IsRequired().HasMaxLength(2);
            entity.Property(e => e.W9FileUrl).HasMaxLength(500);
            entity.Property(e => e.WithholdingTaxRate).HasPrecision(5, 4);
            entity.Property(e => e.TaxTreatyCountry).HasMaxLength(2);
            entity.Property(e => e.TaxNotes).HasMaxLength(2000);
            
            entity.HasIndex(e => new { e.VendorId, e.VendorType }).IsUnique();
            entity.HasIndex(e => e.Require1099);
        });

        // RegistrarTldPriceDownloadSession configuration
        modelBuilder.Entity<RegistrarTldPriceDownloadSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TriggerSource).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);

            entity.HasIndex(e => new { e.RegistrarId, e.StartedAtUtc });
            entity.HasIndex(e => e.Success);

            entity.HasOne(e => e.Registrar)
                .WithMany(r => r.PriceDownloadSessions)
                .HasForeignKey(e => e.RegistrarId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RegistrarTldPriceChangeLog configuration
        modelBuilder.Entity<RegistrarTldPriceChangeLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChangeSource).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ChangedBy).HasMaxLength(100);
            entity.Property(e => e.OldCurrency).HasMaxLength(3);
            entity.Property(e => e.NewCurrency).HasMaxLength(3);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.Property(e => e.OldRegistrationCost).HasPrecision(18, 4);
            entity.Property(e => e.NewRegistrationCost).HasPrecision(18, 4);
            entity.Property(e => e.OldRenewalCost).HasPrecision(18, 4);
            entity.Property(e => e.NewRenewalCost).HasPrecision(18, 4);
            entity.Property(e => e.OldTransferCost).HasPrecision(18, 4);
            entity.Property(e => e.NewTransferCost).HasPrecision(18, 4);

            entity.HasIndex(e => e.RegistrarTldId);
            entity.HasIndex(e => e.DownloadSessionId);
            entity.HasIndex(e => e.ChangedAtUtc);
            entity.HasIndex(e => e.ChangeSource);

            entity.HasOne(e => e.RegistrarTld)
                .WithMany(rt => rt.PriceChangeLogs)
                .HasForeignKey(e => e.RegistrarTldId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.DownloadSession)
                .WithMany(s => s.PriceChangeLogs)
                .HasForeignKey(e => e.DownloadSessionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // QuoteRevision configuration
        modelBuilder.Entity<QuoteRevision>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ActionType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SnapshotJson).IsRequired().HasMaxLength(int.MaxValue);
            entity.Property(e => e.PdfFileName).HasMaxLength(255);
            entity.Property(e => e.PdfFilePath).HasMaxLength(1000);
            entity.Property(e => e.ContentHash).HasMaxLength(256);
            entity.Property(e => e.Notes).HasMaxLength(2000);

            entity.HasIndex(e => e.QuoteId);
            entity.HasIndex(e => new { e.QuoteId, e.RevisionNumber }).IsUnique();
            entity.HasIndex(e => e.ActionType);

            entity.HasOne(e => e.Quote)
                .WithMany(q => q.Revisions)
                .HasForeignKey(e => e.QuoteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Invoice configuration update for SelectedPaymentGatewayId
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasOne(e => e.SelectedPaymentGateway)
                .WithMany()
                .HasForeignKey(e => e.SelectedPaymentGatewayId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
