using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddressTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    NormalizedCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackupSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DatabaseName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Frequency = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastBackupDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextBackupDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillingCycles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DurationInDays = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingCycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ControlPanelTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Version = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlPanelTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    Tld = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Iso3 = table.Column<string>(type: "TEXT", nullable: true),
                    Numeric = table.Column<int>(type: "INTEGER", nullable: true),
                    EnglishName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LocalName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    NormalizedEnglishName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NormalizedLocalName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.UniqueConstraint("AK_Countries_Code", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    AppliesTo = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    MaximumDiscount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MaxUsages = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxUsagesPerCustomer = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowedServiceTypeIdsJson = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    UsageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    InternalNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BaseCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    TargetCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Source = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Markup = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    EffectiveRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    NormalizedCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DnsRecordTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    HasPriority = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasWeight = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasPort = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEditableByUser = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DefaultTTL = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 3600),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnsRecordTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateType = table.Column<int>(type: "INTEGER", nullable: false),
                    FileContent = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    MimeType = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlaceholderVariables = table.Column<string>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRateDownloadLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BaseCurrency = table.Column<string>(type: "TEXT", nullable: false),
                    TargetCurrency = table.Column<string>(type: "TEXT", nullable: true),
                    Source = table.Column<int>(type: "INTEGER", nullable: false),
                    Success = table.Column<bool>(type: "INTEGER", nullable: false),
                    DownloadTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RatesDownloaded = table.Column<int>(type: "INTEGER", nullable: false),
                    RatesAdded = table.Column<int>(type: "INTEGER", nullable: false),
                    RatesUpdated = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    ErrorCode = table.Column<string>(type: "TEXT", nullable: true),
                    DurationMs = table.Column<long>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    IsStartupDownload = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsScheduledDownload = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRateDownloadLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HostingPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DiskSpaceMB = table.Column<int>(type: "INTEGER", nullable: false),
                    BandwidthMB = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailAccounts = table.Column<int>(type: "INTEGER", nullable: false),
                    Databases = table.Column<int>(type: "INTEGER", nullable: false),
                    Domains = table.Column<int>(type: "INTEGER", nullable: false),
                    Subdomains = table.Column<int>(type: "INTEGER", nullable: false),
                    FtpAccounts = table.Column<int>(type: "INTEGER", nullable: false),
                    SslSupport = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackupSupport = table.Column<bool>(type: "INTEGER", nullable: false),
                    DedicatedIp = table.Column<bool>(type: "INTEGER", nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    YearlyPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", nullable: false),
                    Payload = table.Column<string>(type: "TEXT", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    CorrelationId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGateways",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ProviderCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApiKey = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ApiSecret = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ConfigurationJson = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    UseSandbox = table.Column<bool>(type: "INTEGER", nullable: false),
                    WebhookUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    WebhookSecret = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    LogoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    SupportedCurrencies = table.Column<string>(type: "TEXT", nullable: false),
                    FeePercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    FixedFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGateways", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Registrars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContactEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateType = table.Column<int>(type: "INTEGER", nullable: false),
                    ReportEngine = table.Column<string>(type: "TEXT", nullable: false),
                    FileContent = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    MimeType = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataSourceInfo = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultExportFormat = table.Column<string>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResellerCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    CompanyRegistrationNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TaxId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    VatNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DefaultCurrency = table.Column<string>(type: "TEXT", nullable: false),
                    SupportedCurrencies = table.Column<string>(type: "TEXT", nullable: true),
                    ApplyCurrencyMarkup = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultCurrencyMarkup = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ServerType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    HostProvider = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    OperatingSystem = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CpuCores = table.Column<int>(type: "INTEGER", nullable: true),
                    RamMB = table.Column<int>(type: "INTEGER", nullable: true),
                    DiskSpaceGB = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CountryCode = table.Column<string>(type: "TEXT", nullable: false),
                    StateCode = table.Column<string>(type: "TEXT", nullable: true),
                    TaxName = table.Column<string>(type: "TEXT", nullable: false),
                    TaxRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AppliesToSetupFees = table.Column<bool>(type: "INTEGER", nullable: false),
                    AppliesToRecurring = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReverseCharge = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaxAuthority = table.Column<string>(type: "TEXT", nullable: false),
                    TaxRegistrationNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    InternalNotes = table.Column<string>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tlds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Extension = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    RulesUrl = table.Column<string>(type: "TEXT", nullable: false),
                    IsSecondLevel = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultRegistrationYears = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxRegistrationYears = table.Column<int>(type: "INTEGER", nullable: true),
                    RequiresPrivacy = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tlds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorTaxProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VendorId = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorType = table.Column<int>(type: "INTEGER", nullable: false),
                    TaxIdNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TaxResidenceCountry = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    Require1099 = table.Column<bool>(type: "INTEGER", nullable: false),
                    W9OnFile = table.Column<bool>(type: "INTEGER", nullable: false),
                    W9FileUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    WithholdingTaxRate = table.Column<decimal>(type: "TEXT", precision: 5, scale: 4, nullable: true),
                    TaxTreatyExempt = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaxTreatyCountry = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    TaxNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorTaxProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostalCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Region = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Latitude = table.Column<decimal>(type: "TEXT", precision: 10, scale: 7, nullable: true),
                    Longitude = table.Column<decimal>(type: "TEXT", precision: 10, scale: 7, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    NormalizedCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    NormalizedCountryCode = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    NormalizedCity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NormalizedState = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NormalizedRegion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NormalizedDistrict = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostalCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostalCodes_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    TaxId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    VatNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsCompany = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CustomerStatusId = table.Column<int>(type: "INTEGER", nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NormalizedCustomerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Balance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CreditLimit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    BillingEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PreferredPaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PreferredCurrency = table.Column<string>(type: "TEXT", nullable: false),
                    AllowCurrencyOverride = table.Column<bool>(type: "INTEGER", nullable: false),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Customers_CustomerStatuses_CustomerStatusId",
                        column: x => x.CustomerStatusId,
                        principalTable: "CustomerStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrarSelectionPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegistrarId = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    OffersHosting = table.Column<bool>(type: "INTEGER", nullable: false),
                    OffersEmail = table.Column<bool>(type: "INTEGER", nullable: false),
                    OffersSsl = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaxCostDifferenceThreshold = table.Column<decimal>(type: "TEXT", nullable: true),
                    PreferForHostingCustomers = table.Column<bool>(type: "INTEGER", nullable: false),
                    PreferForEmailCustomers = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrarSelectionPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrarSelectionPreferences_Registrars_RegistrarId",
                        column: x => x.RegistrarId,
                        principalTable: "Registrars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResellerCompanyId = table.Column<int>(type: "INTEGER", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    MobilePhone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    NormalizedFirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NormalizedLastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAgents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesAgents_ResellerCompanies_ResellerCompanyId",
                        column: x => x.ResellerCompanyId,
                        principalTable: "ResellerCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ServerControlPanels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ControlPanelTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApiUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    UseHttps = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApiToken = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ApiKey = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AdditionalSettings = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastConnectionTest = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsConnectionHealthy = table.Column<bool>(type: "INTEGER", nullable: true),
                    LastError = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerControlPanels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerControlPanels_ControlPanelTypes_ControlPanelTypeId",
                        column: x => x.ControlPanelTypeId,
                        principalTable: "ControlPanelTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServerControlPanels_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerIpAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerId = table.Column<int>(type: "INTEGER", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false),
                    IpVersion = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AssignedTo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerIpAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerIpAddresses_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrarTlds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegistrarId = table.Column<int>(type: "INTEGER", nullable: false),
                    TldId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    AutoRenew = table.Column<bool>(type: "INTEGER", nullable: false),
                    MinRegistrationYears = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxRegistrationYears = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrarTlds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrarTlds_Registrars_RegistrarId",
                        column: x => x.RegistrarId,
                        principalTable: "Registrars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrarTlds_Tlds_TldId",
                        column: x => x.TldId,
                        principalTable: "Tlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResellerTldDiscounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResellerCompanyId = table.Column<int>(type: "INTEGER", nullable: false),
                    TldId = table.Column<int>(type: "INTEGER", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "TEXT", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    DiscountCurrency = table.Column<string>(type: "TEXT", nullable: true),
                    ApplyToRegistration = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApplyToRenewal = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApplyToTransfer = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerTldDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResellerTldDiscounts_ResellerCompanies_ResellerCompanyId",
                        column: x => x.ResellerCompanyId,
                        principalTable: "ResellerCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResellerTldDiscounts_Tlds_TldId",
                        column: x => x.TldId,
                        principalTable: "Tlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TldSalesPricing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TldId = table.Column<int>(type: "INTEGER", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RegistrationPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    RenewalPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    TransferPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    PrivacyPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    FirstYearRegistrationPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    IsPromotional = table.Column<bool>(type: "INTEGER", nullable: false),
                    PromotionName = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TldSalesPricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TldSalesPricing_Tlds_TldId",
                        column: x => x.TldId,
                        principalTable: "Tlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactPersons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Position = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Department = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    NormalizedFirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NormalizedLastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactPersons_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddressTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PostalCodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddressLine1 = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AddressLine2 = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AddressLine3 = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AddressLine4 = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_AddressTypes_AddressTypeId",
                        column: x => x.AddressTypeId,
                        principalTable: "AddressTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_PostalCodes_PostalCodeId",
                        column: x => x.PostalCodeId,
                        principalTable: "PostalCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCredits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Balance = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCredits_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerPaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentGatewayId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentMethodToken = table.Column<string>(type: "TEXT", nullable: false),
                    Last4Digits = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiryMonth = table.Column<int>(type: "INTEGER", nullable: true),
                    ExpiryYear = table.Column<int>(type: "INTEGER", nullable: true),
                    CardBrand = table.Column<string>(type: "TEXT", nullable: false),
                    CardholderName = table.Column<string>(type: "TEXT", nullable: false),
                    BillingAddressJson = table.Column<string>(type: "TEXT", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPaymentMethods_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerPaymentMethods_PaymentGateways_PaymentGatewayId",
                        column: x => x.PaymentGatewayId,
                        principalTable: "PaymentGateways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTaxProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaxIdNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TaxIdType = table.Column<int>(type: "INTEGER", nullable: false),
                    TaxIdValidated = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaxIdValidationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TaxIdValidationResponse = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    TaxResidenceCountry = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    CustomerType = table.Column<int>(type: "INTEGER", nullable: false),
                    TaxExempt = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaxExemptionReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TaxExemptionCertificateUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTaxProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerTaxProfiles_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrarMailAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    MailAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrarMailAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrarMailAddresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EmailConfirmed = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    NormalizedUsername = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DnsZonePackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ResellerCompanyId = table.Column<int>(type: "INTEGER", nullable: true),
                    SalesAgentId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnsZonePackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DnsZonePackages_ResellerCompanies_ResellerCompanyId",
                        column: x => x.ResellerCompanyId,
                        principalTable: "ResellerCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DnsZonePackages_SalesAgents_SalesAgentId",
                        column: x => x.SalesAgentId,
                        principalTable: "SalesAgents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ServiceTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    BillingCycleId = table.Column<int>(type: "INTEGER", nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    SetupFee = table.Column<decimal>(type: "TEXT", nullable: true),
                    TrialDays = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    Sku = table.Column<string>(type: "TEXT", nullable: false),
                    ResellerCompanyId = table.Column<int>(type: "INTEGER", nullable: true),
                    SalesAgentId = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxQuantity = table.Column<int>(type: "INTEGER", nullable: true),
                    MinQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    SpecificationsJson = table.Column<string>(type: "TEXT", nullable: false),
                    HostingPackageId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_BillingCycles_BillingCycleId",
                        column: x => x.BillingCycleId,
                        principalTable: "BillingCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Services_HostingPackages_HostingPackageId",
                        column: x => x.HostingPackageId,
                        principalTable: "HostingPackages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Services_ResellerCompanies_ResellerCompanyId",
                        column: x => x.ResellerCompanyId,
                        principalTable: "ResellerCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Services_SalesAgents_SalesAgentId",
                        column: x => x.SalesAgentId,
                        principalTable: "SalesAgents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Services_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrarTldCostPricing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegistrarTldId = table.Column<int>(type: "INTEGER", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RegistrationCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    RenewalCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    TransferCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    PrivacyCost = table.Column<decimal>(type: "TEXT", nullable: true),
                    FirstYearRegistrationCost = table.Column<decimal>(type: "TEXT", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrarTldCostPricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrarTldCostPricing_RegistrarTlds_RegistrarTldId",
                        column: x => x.RegistrarTldId,
                        principalTable: "RegistrarTlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethodTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerPaymentMethodId = table.Column<int>(type: "INTEGER", nullable: false),
                    EncryptedToken = table.Column<string>(type: "TEXT", nullable: false),
                    GatewayCustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    GatewayPaymentMethodId = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Last4Digits = table.Column<string>(type: "TEXT", nullable: false),
                    CardBrand = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiryMonth = table.Column<int>(type: "INTEGER", nullable: true),
                    ExpiryYear = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethodTokens_CustomerPaymentMethods_CustomerPaymentMethodId",
                        column: x => x.CustomerPaymentMethodId,
                        principalTable: "CustomerPaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActionType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Details = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    IPAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuoteNumber = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SubTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalSetupFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalRecurring = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", nullable: false),
                    TaxRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxName = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerAddress = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerTaxId = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    TermsAndConditions = table.Column<string>(type: "TEXT", nullable: false),
                    InternalComment = table.Column<string>(type: "TEXT", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RejectionReason = table.Column<string>(type: "TEXT", nullable: false),
                    AcceptanceToken = table.Column<string>(type: "TEXT", nullable: false),
                    PreparedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    CouponId = table.Column<int>(type: "INTEGER", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotes_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quotes_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quotes_Users_PreparedByUserId",
                        column: x => x.PreparedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SentEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    From = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    To = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Cc = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Bcc = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    BodyText = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    BodyHtml = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    MessageId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    MaxRetries = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 3),
                    NextAttemptAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RelatedEntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    Attachments = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SentEmails_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SentEmails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TokenType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TokenValue = table.Column<string>(type: "TEXT", nullable: false),
                    Expiry = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorPayouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VendorId = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorType = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PayoutMethod = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    VendorAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    BaseCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    BaseAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    ExchangeRateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FailureReason = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    FailureCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionReference = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PaymentGatewayResponse = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    RequiresManualIntervention = table.Column<bool>(type: "INTEGER", nullable: false),
                    InterventionReason = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    InterventionResolvedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InterventionResolvedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    InternalNotes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorPayouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorPayouts_Users_InterventionResolvedByUserId",
                        column: x => x.InterventionResolvedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DnsZonePackageRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DnsZonePackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    DnsRecordTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    TTL = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: true),
                    Weight = table.Column<int>(type: "INTEGER", nullable: true),
                    Port = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnsZonePackageRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DnsZonePackageRecords_DnsRecordTypes_DnsRecordTypeId",
                        column: x => x.DnsRecordTypeId,
                        principalTable: "DnsRecordTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DnsZonePackageRecords_DnsZonePackages_DnsZonePackageId",
                        column: x => x.DnsZonePackageId,
                        principalTable: "DnsZonePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HostingAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<int>(type: "INTEGER", nullable: true),
                    ServerControlPanelId = table.Column<int>(type: "INTEGER", nullable: true),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExternalAccountId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: true),
                    ConfigurationJson = table.Column<string>(type: "TEXT", nullable: true),
                    DiskUsageMB = table.Column<int>(type: "INTEGER", nullable: true),
                    BandwidthUsageMB = table.Column<int>(type: "INTEGER", nullable: true),
                    DiskQuotaMB = table.Column<int>(type: "INTEGER", nullable: true),
                    BandwidthLimitMB = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxEmailAccounts = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxDatabases = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxFtpAccounts = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxSubdomains = table.Column<int>(type: "INTEGER", nullable: true),
                    PlanName = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostingAccounts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostingAccounts_ServerControlPanels_ServerControlPanelId",
                        column: x => x.ServerControlPanelId,
                        principalTable: "ServerControlPanels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostingAccounts_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HostingAccounts_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegisteredDomains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    RegistrarId = table.Column<int>(type: "INTEGER", nullable: false),
                    RegistrarTldId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AutoRenew = table.Column<bool>(type: "INTEGER", nullable: false),
                    PrivacyProtection = table.Column<bool>(type: "INTEGER", nullable: false),
                    RegistrationPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    RenewalPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredDomains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisteredDomains_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegisteredDomains_RegistrarTlds_RegistrarTldId",
                        column: x => x.RegistrarTldId,
                        principalTable: "RegistrarTlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegisteredDomains_Registrars_RegistrarId",
                        column: x => x.RegistrarId,
                        principalTable: "Registrars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegisteredDomains_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    BillingCycleId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerPaymentMethodId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextBillingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CurrentPeriodStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", nullable: false),
                    BillingPeriodCount = table.Column<int>(type: "INTEGER", nullable: false),
                    BillingPeriodUnit = table.Column<int>(type: "INTEGER", nullable: false),
                    TrialEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsInTrial = table.Column<bool>(type: "INTEGER", nullable: false),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxRetryAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    LastBillingAttempt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastSuccessfulBilling = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CancellationReason = table.Column<string>(type: "TEXT", nullable: false),
                    PausedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PauseReason = table.Column<string>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    SendEmailNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    AutoRetryFailedPayments = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_BillingCycles_BillingCycleId",
                        column: x => x.BillingCycleId,
                        principalTable: "BillingCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_CustomerPaymentMethods_CustomerPaymentMethodId",
                        column: x => x.CustomerPaymentMethodId,
                        principalTable: "CustomerPaymentMethods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subscriptions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderNumber = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuoteId = table.Column<int>(type: "INTEGER", nullable: true),
                    CouponId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderType = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextBillingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SetupFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    RecurringAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", nullable: false),
                    BaseCurrencyCode = table.Column<string>(type: "TEXT", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "TEXT", nullable: true),
                    ExchangeRateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TrialEndsAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SuspendedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SuspensionReason = table.Column<string>(type: "TEXT", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CancellationReason = table.Column<string>(type: "TEXT", nullable: false),
                    AutoRenew = table.Column<bool>(type: "INTEGER", nullable: false),
                    RenewalReminderSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    InternalComment = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuoteLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuoteId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    BillingCycleId = table.Column<int>(type: "INTEGER", nullable: false),
                    LineNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    SetupFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    RecurringPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Discount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalSetupFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalRecurringPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalWithTax = table.Column<decimal>(type: "TEXT", nullable: false),
                    ServiceNameSnapshot = table.Column<string>(type: "TEXT", nullable: false),
                    BillingCycleNameSnapshot = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuoteLines_BillingCycles_BillingCycleId",
                        column: x => x.BillingCycleId,
                        principalTable: "BillingCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuoteLines_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuoteLines_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HostingDatabases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostingAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    DatabaseName = table.Column<string>(type: "TEXT", nullable: false),
                    DatabaseType = table.Column<string>(type: "TEXT", nullable: false),
                    SizeMB = table.Column<int>(type: "INTEGER", nullable: true),
                    ServerHost = table.Column<string>(type: "TEXT", nullable: true),
                    ServerPort = table.Column<int>(type: "INTEGER", nullable: true),
                    CharacterSet = table.Column<string>(type: "TEXT", nullable: true),
                    Collation = table.Column<string>(type: "TEXT", nullable: true),
                    ExternalDatabaseId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingDatabases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostingDatabases_HostingAccounts_HostingAccountId",
                        column: x => x.HostingAccountId,
                        principalTable: "HostingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HostingDomains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostingAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    DomainName = table.Column<string>(type: "TEXT", nullable: false),
                    DomainType = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentRoot = table.Column<string>(type: "TEXT", nullable: true),
                    SslEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SslExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SslIssuer = table.Column<string>(type: "TEXT", nullable: true),
                    AutoRenewSsl = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExternalDomainId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: true),
                    PhpEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    PhpVersion = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingDomains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostingDomains_HostingAccounts_HostingAccountId",
                        column: x => x.HostingAccountId,
                        principalTable: "HostingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HostingEmailAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostingAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    QuotaMB = table.Column<int>(type: "INTEGER", nullable: true),
                    UsageMB = table.Column<int>(type: "INTEGER", nullable: true),
                    IsForwarderOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    ForwardTo = table.Column<string>(type: "TEXT", nullable: true),
                    AutoResponderEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AutoResponderMessage = table.Column<string>(type: "TEXT", nullable: true),
                    SpamFilterEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SpamScoreThreshold = table.Column<int>(type: "INTEGER", nullable: true),
                    ExternalEmailId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingEmailAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostingEmailAccounts_HostingAccounts_HostingAccountId",
                        column: x => x.HostingAccountId,
                        principalTable: "HostingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HostingFtpAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostingAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    HomeDirectory = table.Column<string>(type: "TEXT", nullable: false),
                    QuotaMB = table.Column<int>(type: "INTEGER", nullable: true),
                    ReadOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    SftpEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    FtpsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExternalFtpId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingFtpAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostingFtpAccounts_HostingAccounts_HostingAccountId",
                        column: x => x.HostingAccountId,
                        principalTable: "HostingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DnsRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DomainId = table.Column<int>(type: "INTEGER", nullable: false),
                    DnsRecordTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    TTL = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: true),
                    Weight = table.Column<int>(type: "INTEGER", nullable: true),
                    Port = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnsRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DnsRecords_DnsRecordTypes_DnsRecordTypeId",
                        column: x => x.DnsRecordTypeId,
                        principalTable: "DnsRecordTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DnsRecords_RegisteredDomains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "RegisteredDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DomainContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContactType = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Organization = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Fax = table.Column<string>(type: "TEXT", nullable: true),
                    Address1 = table.Column<string>(type: "TEXT", nullable: false),
                    Address2 = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedFirstName = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedLastName = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedEmail = table.Column<string>(type: "TEXT", nullable: false),
                    DomainId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DomainContacts_RegisteredDomains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "RegisteredDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NameServers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DomainId = table.Column<int>(type: "INTEGER", nullable: false),
                    Hostname = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: true),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameServers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NameServers_RegisteredDomains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "RegisteredDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CouponUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CouponId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuoteId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponUsages_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CouponUsages_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CouponUsages_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CouponUsages_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SubTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "TEXT", nullable: false),
                    AmountDue = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    TaxRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BaseCurrencyCode = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayCurrencyCode = table.Column<string>(type: "TEXT", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "TEXT", nullable: true),
                    BaseTotalAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    ExchangeRateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CustomerAddress = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerTaxId = table.Column<string>(type: "TEXT", nullable: false),
                    PaymentReference = table.Column<string>(type: "TEXT", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    InternalComment = table.Column<string>(type: "TEXT", nullable: false),
                    SelectedPaymentGatewayId = table.Column<int>(type: "INTEGER", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_PaymentGateways_SelectedPaymentGatewayId",
                        column: x => x.SelectedPaymentGatewayId,
                        principalTable: "PaymentGateways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostingDatabaseUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostingDatabaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    Privileges = table.Column<string>(type: "TEXT", nullable: true),
                    AllowedHosts = table.Column<string>(type: "TEXT", nullable: true),
                    ExternalUserId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingDatabaseUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostingDatabaseUsers_HostingDatabases_HostingDatabaseId",
                        column: x => x.HostingDatabaseId,
                        principalTable: "HostingDatabases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: true),
                    UnitId = table.Column<int>(type: "INTEGER", nullable: true),
                    LineNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    LineType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsGatewayFee = table.Column<bool>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TaxRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalWithTax = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ServiceNameSnapshot = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AccountingCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentIntents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentGatewayId = table.Column<int>(type: "INTEGER", nullable: false),
                    GatewayIntentId = table.Column<string>(type: "TEXT", nullable: false),
                    ClientSecret = table.Column<string>(type: "TEXT", nullable: false),
                    ReturnUrl = table.Column<string>(type: "TEXT", nullable: false),
                    CancelUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    MetadataJson = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorizedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CapturedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FailureReason = table.Column<string>(type: "TEXT", nullable: false),
                    GatewayResponse = table.Column<string>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentIntents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentIntents_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentIntents_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentIntents_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentIntents_PaymentGateways_PaymentGatewayId",
                        column: x => x.PaymentGatewayId,
                        principalTable: "PaymentGateways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceLineId = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorPayoutId = table.Column<int>(type: "INTEGER", nullable: true),
                    VendorType = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorId = table.Column<int>(type: "INTEGER", nullable: true),
                    VendorName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    VendorCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    VendorAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    BaseCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    BaseAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    ExchangeRateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRefundable = table.Column<bool>(type: "INTEGER", nullable: false),
                    RefundPolicy = table.Column<int>(type: "INTEGER", nullable: false),
                    RefundDeadline = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorCosts_InvoiceLines_InvoiceLineId",
                        column: x => x.InvoiceLineId,
                        principalTable: "InvoiceLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorCosts_VendorPayouts_VendorPayoutId",
                        column: x => x.VendorPayoutId,
                        principalTable: "VendorPayouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentIntentId = table.Column<int>(type: "INTEGER", nullable: true),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", maxLength: 50, nullable: false),
                    TransactionId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", nullable: false),
                    BaseCurrencyCode = table.Column<string>(type: "TEXT", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "TEXT", nullable: true),
                    BaseAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    GatewayFeeAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    GatewayFeeCurrency = table.Column<string>(type: "TEXT", nullable: true),
                    ActualExchangeRate = table.Column<decimal>(type: "TEXT", nullable: true),
                    PaymentGatewayId = table.Column<int>(type: "INTEGER", nullable: true),
                    GatewayResponse = table.Column<string>(type: "TEXT", nullable: false),
                    FailureReason = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RefundedAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsAutomatic = table.Column<bool>(type: "INTEGER", nullable: false),
                    InternalNotes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_PaymentGateways_PaymentGatewayId",
                        column: x => x.PaymentGatewayId,
                        principalTable: "PaymentGateways",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_PaymentIntents_PaymentIntentId",
                        column: x => x.PaymentIntentId,
                        principalTable: "PaymentIntents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CreditTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerCreditId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: true),
                    PaymentTransactionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    InternalNotes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_CustomerCredits_CustomerCreditId",
                        column: x => x.CustomerCreditId,
                        principalTable: "CustomerCredits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CreditTransactions_PaymentTransactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CreditTransactions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvoicePayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentTransactionId = table.Column<int>(type: "INTEGER", nullable: false),
                    AmountApplied = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    InvoiceBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    InvoiceTotalAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsFullPayment = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoicePayments_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoicePayments_PaymentTransactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentTransactionId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomerPaymentMethodId = table.Column<int>(type: "INTEGER", nullable: false),
                    AttemptedAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    GatewayResponse = table.Column<string>(type: "TEXT", nullable: false),
                    GatewayTransactionId = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorCode = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: false),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    NextRetryAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RequiresAuthentication = table.Column<bool>(type: "INTEGER", nullable: false),
                    AuthenticationUrl = table.Column<string>(type: "TEXT", nullable: false),
                    AuthenticationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: false),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_CustomerPaymentMethods_CustomerPaymentMethodId",
                        column: x => x.CustomerPaymentMethodId,
                        principalTable: "CustomerPaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_PaymentTransactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaymentTransactionId = table.Column<int>(type: "INTEGER", nullable: false),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RefundTransactionId = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FailureReason = table.Column<string>(type: "TEXT", nullable: false),
                    GatewayResponse = table.Column<string>(type: "TEXT", nullable: false),
                    InitiatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    InternalNotes = table.Column<string>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Refunds_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Refunds_PaymentTransactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Refunds_Users_InitiatedByUserId",
                        column: x => x.InitiatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionBillingHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubscriptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: true),
                    PaymentTransactionId = table.Column<int>(type: "INTEGER", nullable: true),
                    BillingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AmountCharged = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AttemptCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsAutomatic = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProcessedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionBillingHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionBillingHistories_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubscriptionBillingHistories_PaymentTransactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubscriptionBillingHistories_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionBillingHistories_Users_ProcessedByUserId",
                        column: x => x.ProcessedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefundLossAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RefundId = table.Column<int>(type: "INTEGER", nullable: false),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginalInvoiceAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RefundedAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    VendorCostUnrecoverable = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    NetLoss = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    ApprovalStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ApprovedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DenialReason = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InternalNotes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundLossAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefundLossAudits_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundLossAudits_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundLossAudits_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressTypes_Code",
                table: "AddressTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AddressTypes_IsActive",
                table: "AddressTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AddressTypes_IsDefault",
                table: "AddressTypes",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_AddressTypes_NormalizedCode",
                table: "AddressTypes",
                column: "NormalizedCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AddressTypes_SortOrder",
                table: "AddressTypes",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPersons_CustomerId",
                table: "ContactPersons",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPersons_Email",
                table: "ContactPersons",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPersons_IsActive",
                table: "ContactPersons",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPersons_IsPrimary",
                table: "ContactPersons",
                column: "IsPrimary");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPersons_NormalizedFirstName_NormalizedLastName",
                table: "ContactPersons",
                columns: new[] { "NormalizedFirstName", "NormalizedLastName" });

            migrationBuilder.CreateIndex(
                name: "IX_ControlPanelTypes_IsActive",
                table: "ControlPanelTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ControlPanelTypes_Name",
                table: "ControlPanelTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code",
                table: "Countries",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_EnglishName",
                table: "Countries",
                column: "EnglishName");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_NormalizedEnglishName",
                table: "Countries",
                column: "NormalizedEnglishName");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Tld",
                table: "Countries",
                column: "Tld");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_IsActive",
                table: "Coupons",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_NormalizedName",
                table: "Coupons",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_ValidFrom",
                table: "Coupons",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_ValidUntil",
                table: "Coupons",
                column: "ValidUntil");

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsages_CouponId",
                table: "CouponUsages",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsages_CustomerId",
                table: "CouponUsages",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsages_OrderId",
                table: "CouponUsages",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsages_QuoteId",
                table: "CouponUsages",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_CreatedByUserId",
                table: "CreditTransactions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_CustomerCreditId",
                table: "CreditTransactions",
                column: "CustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_InvoiceId",
                table: "CreditTransactions",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_PaymentTransactionId",
                table: "CreditTransactions",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_BaseCurrency_TargetCurrency_EffectiveDate",
                table: "CurrencyExchangeRates",
                columns: new[] { "BaseCurrency", "TargetCurrency", "EffectiveDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_IsActive",
                table: "CurrencyExchangeRates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_AddressTypeId",
                table: "CustomerAddresses",
                column: "AddressTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_CustomerId",
                table: "CustomerAddresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_IsActive",
                table: "CustomerAddresses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_IsPrimary",
                table: "CustomerAddresses",
                column: "IsPrimary");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_PostalCodeId",
                table: "CustomerAddresses",
                column: "PostalCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCredits_CustomerId",
                table: "CustomerCredits",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPaymentMethods_CustomerId",
                table: "CustomerPaymentMethods",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPaymentMethods_PaymentGatewayId",
                table: "CustomerPaymentMethods",
                column: "PaymentGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CountryId",
                table: "Customers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerStatusId",
                table: "Customers",
                column: "CustomerStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IsActive",
                table: "Customers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_NormalizedCustomerName",
                table: "Customers",
                column: "NormalizedCustomerName");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_NormalizedName",
                table: "Customers",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Status",
                table: "Customers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TaxId",
                table: "Customers",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_VatNumber",
                table: "Customers",
                column: "VatNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStatuses_Code",
                table: "CustomerStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStatuses_IsActive",
                table: "CustomerStatuses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStatuses_IsDefault",
                table: "CustomerStatuses",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStatuses_NormalizedCode",
                table: "CustomerStatuses",
                column: "NormalizedCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStatuses_SortOrder",
                table: "CustomerStatuses",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTaxProfiles_CustomerId",
                table: "CustomerTaxProfiles",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTaxProfiles_TaxIdNumber",
                table: "CustomerTaxProfiles",
                column: "TaxIdNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTaxProfiles_TaxResidenceCountry",
                table: "CustomerTaxProfiles",
                column: "TaxResidenceCountry");

            migrationBuilder.CreateIndex(
                name: "IX_DnsRecords_DnsRecordTypeId",
                table: "DnsRecords",
                column: "DnsRecordTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsRecords_DomainId",
                table: "DnsRecords",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsRecords_DomainId_DnsRecordTypeId",
                table: "DnsRecords",
                columns: new[] { "DomainId", "DnsRecordTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_DnsRecordTypes_IsActive",
                table: "DnsRecordTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DnsRecordTypes_Type",
                table: "DnsRecordTypes",
                column: "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackageRecords_DnsRecordTypeId",
                table: "DnsZonePackageRecords",
                column: "DnsRecordTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackageRecords_DnsZonePackageId",
                table: "DnsZonePackageRecords",
                column: "DnsZonePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_IsActive",
                table: "DnsZonePackages",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_IsDefault",
                table: "DnsZonePackages",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_Name",
                table: "DnsZonePackages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_ResellerCompanyId",
                table: "DnsZonePackages",
                column: "ResellerCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_SalesAgentId",
                table: "DnsZonePackages",
                column: "SalesAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_SortOrder",
                table: "DnsZonePackages",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_DomainContacts_DomainId",
                table: "DomainContacts",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingAccounts_CustomerId",
                table: "HostingAccounts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingAccounts_ServerControlPanelId",
                table: "HostingAccounts",
                column: "ServerControlPanelId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingAccounts_ServerId",
                table: "HostingAccounts",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingAccounts_ServiceId",
                table: "HostingAccounts",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingDatabases_HostingAccountId",
                table: "HostingDatabases",
                column: "HostingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingDatabaseUsers_HostingDatabaseId",
                table: "HostingDatabaseUsers",
                column: "HostingDatabaseId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingDomains_HostingAccountId",
                table: "HostingDomains",
                column: "HostingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingEmailAccounts_HostingAccountId",
                table: "HostingEmailAccounts",
                column: "HostingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingFtpAccounts_HostingAccountId",
                table: "HostingFtpAccounts",
                column: "HostingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingPackages_IsActive",
                table: "HostingPackages",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_HostingPackages_Name",
                table: "HostingPackages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_HostingPackages_NormalizedName",
                table: "HostingPackages",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_InvoiceId",
                table: "InvoiceLines",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_ServiceId",
                table: "InvoiceLines",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_UnitId",
                table: "InvoiceLines",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayments_InvoiceId",
                table: "InvoicePayments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayments_PaymentTransactionId",
                table: "InvoicePayments",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerId",
                table: "Invoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_OrderId",
                table: "Invoices",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SelectedPaymentGatewayId",
                table: "Invoices",
                column: "SelectedPaymentGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_NameServers_DomainId",
                table: "NameServers",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_NameServers_DomainId_SortOrder",
                table: "NameServers",
                columns: new[] { "DomainId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CouponId",
                table: "Orders",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_QuoteId",
                table: "Orders",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ServiceId",
                table: "Orders",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_CustomerPaymentMethodId",
                table: "PaymentAttempts",
                column: "CustomerPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_InvoiceId",
                table: "PaymentAttempts",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_PaymentTransactionId",
                table: "PaymentAttempts",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateways_IsActive",
                table: "PaymentGateways",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateways_IsDefault",
                table: "PaymentGateways",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateways_NormalizedName",
                table: "PaymentGateways",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateways_ProviderCode",
                table: "PaymentGateways",
                column: "ProviderCode");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntents_CustomerId",
                table: "PaymentIntents",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntents_InvoiceId",
                table: "PaymentIntents",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntents_OrderId",
                table: "PaymentIntents",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntents_PaymentGatewayId",
                table: "PaymentIntents",
                column: "PaymentGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethodTokens_CustomerPaymentMethodId",
                table: "PaymentMethodTokens",
                column: "CustomerPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_InvoiceId",
                table: "PaymentTransactions",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentGatewayId",
                table: "PaymentTransactions",
                column: "PaymentGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentIntentId",
                table: "PaymentTransactions",
                column: "PaymentIntentId");

            migrationBuilder.CreateIndex(
                name: "IX_PostalCodes_City",
                table: "PostalCodes",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_PostalCodes_Code_CountryCode",
                table: "PostalCodes",
                columns: new[] { "Code", "CountryCode" });

            migrationBuilder.CreateIndex(
                name: "IX_PostalCodes_CountryCode",
                table: "PostalCodes",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_PostalCodes_NormalizedCity",
                table: "PostalCodes",
                column: "NormalizedCity");

            migrationBuilder.CreateIndex(
                name: "IX_PostalCodes_NormalizedCode_NormalizedCountryCode",
                table: "PostalCodes",
                columns: new[] { "NormalizedCode", "NormalizedCountryCode" });

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLines_BillingCycleId",
                table: "QuoteLines",
                column: "BillingCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLines_QuoteId",
                table: "QuoteLines",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteLines_ServiceId",
                table: "QuoteLines",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_CouponId",
                table: "Quotes",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_CustomerId",
                table: "Quotes",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_PreparedByUserId",
                table: "Quotes",
                column: "PreparedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundLossAudits_ApprovalStatus",
                table: "RefundLossAudits",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_RefundLossAudits_ApprovedAt",
                table: "RefundLossAudits",
                column: "ApprovedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefundLossAudits_ApprovedByUserId",
                table: "RefundLossAudits",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundLossAudits_InvoiceId",
                table: "RefundLossAudits",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundLossAudits_RefundId",
                table: "RefundLossAudits",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_InitiatedByUserId",
                table: "Refunds",
                column: "InitiatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_InvoiceId",
                table: "Refunds",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_PaymentTransactionId",
                table: "Refunds",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomains_CustomerId",
                table: "RegisteredDomains",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomains_ExpirationDate",
                table: "RegisteredDomains",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomains_Name",
                table: "RegisteredDomains",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomains_NormalizedName",
                table: "RegisteredDomains",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomains_RegistrarId",
                table: "RegisteredDomains",
                column: "RegistrarId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomains_RegistrarTldId",
                table: "RegisteredDomains",
                column: "RegistrarTldId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomains_ServiceId",
                table: "RegisteredDomains",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomains_Status",
                table: "RegisteredDomains",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarMailAddresses_CustomerId",
                table: "RegistrarMailAddresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarMailAddresses_IsActive",
                table: "RegistrarMailAddresses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarMailAddresses_IsDefault",
                table: "RegistrarMailAddresses",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarMailAddresses_MailAddress",
                table: "RegistrarMailAddresses",
                column: "MailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrars_Code",
                table: "Registrars",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrars_IsActive",
                table: "Registrars",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Registrars_IsDefault",
                table: "Registrars",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_Registrars_Name",
                table: "Registrars",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Registrars_NormalizedName",
                table: "Registrars",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarSelectionPreferences_RegistrarId",
                table: "RegistrarSelectionPreferences",
                column: "RegistrarId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldCostPricing_RegistrarTldId",
                table: "RegistrarTldCostPricing",
                column: "RegistrarTldId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTlds_IsActive",
                table: "RegistrarTlds",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTlds_RegistrarId_TldId",
                table: "RegistrarTlds",
                columns: new[] { "RegistrarId", "TldId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTlds_TldId",
                table: "RegistrarTlds",
                column: "TldId");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerCompanies_Email",
                table: "ResellerCompanies",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerCompanies_IsActive",
                table: "ResellerCompanies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerCompanies_Name",
                table: "ResellerCompanies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerTldDiscounts_ResellerCompanyId",
                table: "ResellerTldDiscounts",
                column: "ResellerCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerTldDiscounts_TldId",
                table: "ResellerTldDiscounts",
                column: "TldId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgents_Email",
                table: "SalesAgents",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgents_IsActive",
                table: "SalesAgents",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgents_NormalizedFirstName_NormalizedLastName",
                table: "SalesAgents",
                columns: new[] { "NormalizedFirstName", "NormalizedLastName" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgents_ResellerCompanyId",
                table: "SalesAgents",
                column: "ResellerCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_CustomerId",
                table: "SentEmails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_From",
                table: "SentEmails",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_MessageId",
                table: "SentEmails",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_NextAttemptAt",
                table: "SentEmails",
                column: "NextAttemptAt");

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_RelatedEntityType_RelatedEntityId",
                table: "SentEmails",
                columns: new[] { "RelatedEntityType", "RelatedEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_SentDate",
                table: "SentEmails",
                column: "SentDate");

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_Status",
                table: "SentEmails",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_Status_NextAttemptAt",
                table: "SentEmails",
                columns: new[] { "Status", "NextAttemptAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_UserId",
                table: "SentEmails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerControlPanels_ControlPanelTypeId",
                table: "ServerControlPanels",
                column: "ControlPanelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerControlPanels_ServerId",
                table: "ServerControlPanels",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerControlPanels_Status",
                table: "ServerControlPanels",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ServerIpAddresses_IpAddress",
                table: "ServerIpAddresses",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_ServerIpAddresses_ServerId",
                table: "ServerIpAddresses",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerIpAddresses_Status",
                table: "ServerIpAddresses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_Name",
                table: "Servers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_ServerType",
                table: "Servers",
                column: "ServerType");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_Status",
                table: "Servers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Services_BillingCycleId",
                table: "Services",
                column: "BillingCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_HostingPackageId",
                table: "Services",
                column: "HostingPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ResellerCompanyId",
                table: "Services",
                column: "ResellerCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_SalesAgentId",
                table: "Services",
                column: "SalesAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceTypeId",
                table: "Services",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionBillingHistories_InvoiceId",
                table: "SubscriptionBillingHistories",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionBillingHistories_PaymentTransactionId",
                table: "SubscriptionBillingHistories",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionBillingHistories_ProcessedByUserId",
                table: "SubscriptionBillingHistories",
                column: "ProcessedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionBillingHistories_SubscriptionId",
                table: "SubscriptionBillingHistories",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_BillingCycleId",
                table: "Subscriptions",
                column: "BillingCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CustomerId",
                table: "Subscriptions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CustomerPaymentMethodId",
                table: "Subscriptions",
                column: "CustomerPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ServiceId",
                table: "Subscriptions",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tlds_Extension",
                table: "Tlds",
                column: "Extension",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tlds_IsActive",
                table: "Tlds",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TldSalesPricing_TldId",
                table: "TldSalesPricing",
                column: "TldId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId",
                table: "Tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_Code",
                table: "Units",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_IsActive",
                table: "Units",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CustomerId",
                table: "Users",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedUsername",
                table: "Users",
                column: "NormalizedUsername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorCosts_InvoiceLineId",
                table: "VendorCosts",
                column: "InvoiceLineId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCosts_IsRefundable",
                table: "VendorCosts",
                column: "IsRefundable");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCosts_Status",
                table: "VendorCosts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCosts_VendorPayoutId",
                table: "VendorCosts",
                column: "VendorPayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCosts_VendorType",
                table: "VendorCosts",
                column: "VendorType");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_InterventionResolvedByUserId",
                table: "VendorPayouts",
                column: "InterventionResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_RequiresManualIntervention",
                table: "VendorPayouts",
                column: "RequiresManualIntervention");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_ScheduledDate",
                table: "VendorPayouts",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_Status",
                table: "VendorPayouts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_VendorId",
                table: "VendorPayouts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_VendorType",
                table: "VendorPayouts",
                column: "VendorType");

            migrationBuilder.CreateIndex(
                name: "IX_VendorTaxProfiles_Require1099",
                table: "VendorTaxProfiles",
                column: "Require1099");

            migrationBuilder.CreateIndex(
                name: "IX_VendorTaxProfiles_VendorId_VendorType",
                table: "VendorTaxProfiles",
                columns: new[] { "VendorId", "VendorType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BackupSchedules");

            migrationBuilder.DropTable(
                name: "ContactPersons");

            migrationBuilder.DropTable(
                name: "CouponUsages");

            migrationBuilder.DropTable(
                name: "CreditTransactions");

            migrationBuilder.DropTable(
                name: "CurrencyExchangeRates");

            migrationBuilder.DropTable(
                name: "CustomerAddresses");

            migrationBuilder.DropTable(
                name: "CustomerTaxProfiles");

            migrationBuilder.DropTable(
                name: "DnsRecords");

            migrationBuilder.DropTable(
                name: "DnsZonePackageRecords");

            migrationBuilder.DropTable(
                name: "DocumentTemplates");

            migrationBuilder.DropTable(
                name: "DomainContacts");

            migrationBuilder.DropTable(
                name: "ExchangeRateDownloadLogs");

            migrationBuilder.DropTable(
                name: "HostingDatabaseUsers");

            migrationBuilder.DropTable(
                name: "HostingDomains");

            migrationBuilder.DropTable(
                name: "HostingEmailAccounts");

            migrationBuilder.DropTable(
                name: "HostingFtpAccounts");

            migrationBuilder.DropTable(
                name: "InvoicePayments");

            migrationBuilder.DropTable(
                name: "NameServers");

            migrationBuilder.DropTable(
                name: "OutboxEvents");

            migrationBuilder.DropTable(
                name: "PaymentAttempts");

            migrationBuilder.DropTable(
                name: "PaymentMethodTokens");

            migrationBuilder.DropTable(
                name: "QuoteLines");

            migrationBuilder.DropTable(
                name: "RefundLossAudits");

            migrationBuilder.DropTable(
                name: "RegistrarMailAddresses");

            migrationBuilder.DropTable(
                name: "RegistrarSelectionPreferences");

            migrationBuilder.DropTable(
                name: "RegistrarTldCostPricing");

            migrationBuilder.DropTable(
                name: "ReportTemplates");

            migrationBuilder.DropTable(
                name: "ResellerTldDiscounts");

            migrationBuilder.DropTable(
                name: "SentEmails");

            migrationBuilder.DropTable(
                name: "ServerIpAddresses");

            migrationBuilder.DropTable(
                name: "SubscriptionBillingHistories");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TaxRules");

            migrationBuilder.DropTable(
                name: "TldSalesPricing");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "VendorCosts");

            migrationBuilder.DropTable(
                name: "VendorTaxProfiles");

            migrationBuilder.DropTable(
                name: "CustomerCredits");

            migrationBuilder.DropTable(
                name: "AddressTypes");

            migrationBuilder.DropTable(
                name: "PostalCodes");

            migrationBuilder.DropTable(
                name: "DnsRecordTypes");

            migrationBuilder.DropTable(
                name: "DnsZonePackages");

            migrationBuilder.DropTable(
                name: "HostingDatabases");

            migrationBuilder.DropTable(
                name: "RegisteredDomains");

            migrationBuilder.DropTable(
                name: "Refunds");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "InvoiceLines");

            migrationBuilder.DropTable(
                name: "VendorPayouts");

            migrationBuilder.DropTable(
                name: "HostingAccounts");

            migrationBuilder.DropTable(
                name: "RegistrarTlds");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "CustomerPaymentMethods");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "ServerControlPanels");

            migrationBuilder.DropTable(
                name: "Registrars");

            migrationBuilder.DropTable(
                name: "Tlds");

            migrationBuilder.DropTable(
                name: "PaymentIntents");

            migrationBuilder.DropTable(
                name: "ControlPanelTypes");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "PaymentGateways");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "BillingCycles");

            migrationBuilder.DropTable(
                name: "HostingPackages");

            migrationBuilder.DropTable(
                name: "SalesAgents");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "ResellerCompanies");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "CustomerStatuses");
        }
    }
}
