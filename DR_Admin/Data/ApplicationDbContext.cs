using Microsoft.EntityFrameworkCore;
using ISPAdmin.Data.Entities;

namespace ISPAdmin.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
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
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }
    public DbSet<BillingCycle> BillingCycles { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLine> InvoiceLines { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<Domain> Domains { get; set; }
    public DbSet<Tld> Tlds { get; set; }
    public DbSet<Registrar> Registrars { get; set; }
    public DbSet<RegistrarTld> RegistrarTlds { get; set; }
    public DbSet<DnsRecordType> DnsRecordTypes { get; set; }
    public DbSet<DnsRecord> DnsRecords { get; set; }
    public DbSet<DnsZonePackage> DnsZonePackages { get; set; }
    public DbSet<DnsZonePackageRecord> DnsZonePackageRecords { get; set; }
    public DbSet<HostingAccount> HostingAccounts { get; set; }
    public DbSet<Server> Servers { get; set; }
    public DbSet<ServerIpAddress> ServerIpAddresses { get; set; }
    public DbSet<ControlPanelType> ControlPanelTypes { get; set; }
    public DbSet<ServerControlPanel> ServerControlPanels { get; set; }
    public DbSet<HostingPackage> HostingPackages { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<BackupSchedule> BackupSchedules { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<PostalCode> PostalCodes { get; set; }
    public DbSet<ResellerCompany> ResellerCompanies { get; set; }
    public DbSet<SalesAgent> SalesAgents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.CountryCode).HasMaxLength(2);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.TaxId).HasMaxLength(50);
            entity.Property(e => e.VatNumber).HasMaxLength(50);
            entity.Property(e => e.ContactPerson).HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.BillingEmail).HasMaxLength(200);
            entity.Property(e => e.PreferredPaymentMethod).HasMaxLength(50);
            
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.TaxId);
            entity.HasIndex(e => e.VatNumber);
            entity.HasIndex(e => e.IsActive);
            
            entity.HasOne(e => e.Country)
                .WithMany(c => c.Customers)
                .HasForeignKey(e => e.CountryCode)
                .HasPrincipalKey(c => c.Code)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
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
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CurrencyCode).HasMaxLength(3);
            entity.Property(e => e.TaxName).HasMaxLength(50);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.PaymentMethod).HasMaxLength(100);
            
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
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
        modelBuilder.Entity<Domain>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RegistrationPrice).HasPrecision(18, 2);
            entity.Property(e => e.RenewalPrice).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ExpirationDate);
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Domains)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Registrar)
                .WithMany(r => r.Domains)
                .HasForeignKey(e => e.RegistrarId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.RegistrarTld)
                .WithMany(rt => rt.Domains)
                .HasForeignKey(e => e.RegistrarTldId)
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

        // Server configuration
        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ServerType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.HostProvider).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.OperatingSystem).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ServerType);
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
            
            entity.HasOne(e => e.Server)
                .WithMany(s => s.ControlPanels)
                .HasForeignKey(e => e.ServerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.ControlPanelType)
                .WithMany(c => c.ServerControlPanels)
                .HasForeignKey(e => e.ControlPanelTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // HostingPackage configuration
        modelBuilder.Entity<HostingPackage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.MonthlyPrice).HasPrecision(18, 2);
            entity.Property(e => e.YearlyPrice).HasPrecision(18, 2);
            
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
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

        // SystemSetting configuration
        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).HasMaxLength(1000);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Key).IsUnique();
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
            
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Tld);
            entity.HasIndex(e => e.EnglishName);
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
            
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
        });

        // RegistrarTld configuration
        modelBuilder.Entity<RegistrarTld>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RegistrationCost).HasPrecision(18, 2);
            entity.Property(e => e.RegistrationPrice).HasPrecision(18, 2);
            entity.Property(e => e.RenewalCost).HasPrecision(18, 2);
            entity.Property(e => e.RenewalPrice).HasPrecision(18, 2);
            entity.Property(e => e.TransferCost).HasPrecision(18, 2);
            entity.Property(e => e.TransferPrice).HasPrecision(18, 2);
            entity.Property(e => e.PrivacyCost).HasPrecision(18, 2);
            entity.Property(e => e.PrivacyPrice).HasPrecision(18, 2);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasIndex(e => new { e.RegistrarId, e.TldId }).IsUnique();
            entity.HasIndex(e => e.IsAvailable);
            
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
            
            entity.HasIndex(e => new { e.Code, e.CountryCode });
            entity.HasIndex(e => e.City);
            
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
            
            entity.HasIndex(e => e.ResellerCompanyId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Email);
            
            entity.HasOne(e => e.ResellerCompany)
                .WithMany(r => r.SalesAgents)
                .HasForeignKey(e => e.ResellerCompanyId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
