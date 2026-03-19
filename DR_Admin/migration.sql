CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "AddressTypes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AddressTypes" PRIMARY KEY AUTOINCREMENT,
    "Code" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "SortOrder" INTEGER NOT NULL,
    "NormalizedCode" TEXT NOT NULL,
    "NormalizedName" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "BackupSchedules" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_BackupSchedules" PRIMARY KEY AUTOINCREMENT,
    "DatabaseName" TEXT NOT NULL,
    "Frequency" TEXT NOT NULL,
    "LastBackupDate" TEXT NOT NULL,
    "NextBackupDate" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "BillingCycles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_BillingCycles" PRIMARY KEY AUTOINCREMENT,
    "Code" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "DurationInDays" INTEGER NOT NULL,
    "Description" TEXT NOT NULL,
    "SortOrder" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "ControlPanelTypes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ControlPanelTypes" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "DisplayName" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Version" TEXT NULL,
    "WebsiteUrl" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "Countries" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Countries" PRIMARY KEY AUTOINCREMENT,
    "Code" varchar(2) NOT NULL,
    "Tld" TEXT NOT NULL,
    "Iso3" TEXT NULL,
    "Numeric" INTEGER NULL,
    "EnglishName" TEXT NOT NULL,
    "LocalName" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "NormalizedEnglishName" TEXT NOT NULL,
    "NormalizedLocalName" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "AK_Countries_Code" UNIQUE ("Code")
);

CREATE TABLE "Coupons" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Coupons" PRIMARY KEY AUTOINCREMENT,
    "Code" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "NormalizedName" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Type" INTEGER NOT NULL,
    "Value" TEXT NOT NULL,
    "AppliesTo" INTEGER NOT NULL,
    "RecurrenceType" INTEGER NOT NULL,
    "RecurringYears" INTEGER NULL,
    "MinimumAmount" TEXT NULL,
    "MaximumDiscount" TEXT NULL,
    "ValidFrom" TEXT NOT NULL,
    "ValidUntil" TEXT NOT NULL,
    "MaxUsages" INTEGER NULL,
    "MaxUsagesPerCustomer" INTEGER NULL,
    "IsActive" INTEGER NOT NULL,
    "AllowedServiceTypeIdsJson" TEXT NULL,
    "UsageCount" INTEGER NOT NULL,
    "InternalNotes" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "Currencies" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Currencies" PRIMARY KEY AUTOINCREMENT,
    "Code" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Symbol" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "IsCustomerCurrency" INTEGER NOT NULL,
    "IsProviderCurrency" INTEGER NOT NULL,
    "SortOrder" INTEGER NOT NULL,
    "NormalizedCode" TEXT NOT NULL,
    "NormalizedName" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "CurrencyExchangeRates" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CurrencyExchangeRates" PRIMARY KEY AUTOINCREMENT,
    "BaseCurrency" TEXT NOT NULL,
    "TargetCurrency" TEXT NOT NULL,
    "Rate" TEXT NOT NULL,
    "EffectiveDate" TEXT NOT NULL,
    "ExpiryDate" TEXT NULL,
    "Source" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Markup" TEXT NOT NULL,
    "EffectiveRate" TEXT NOT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "CustomerStatuses" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CustomerStatuses" PRIMARY KEY AUTOINCREMENT,
    "Code" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Color" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "SortOrder" INTEGER NOT NULL,
    "Priority" INTEGER NULL,
    "IsSystem" INTEGER NOT NULL,
    "NormalizedCode" TEXT NOT NULL,
    "NormalizedName" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "DnsRecordTypes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DnsRecordTypes" PRIMARY KEY AUTOINCREMENT,
    "Type" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "HasPriority" INTEGER NOT NULL,
    "HasWeight" INTEGER NOT NULL,
    "HasPort" INTEGER NOT NULL,
    "IsEditableByUser" INTEGER NOT NULL DEFAULT 1,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "DefaultTTL" INTEGER NOT NULL DEFAULT 3600,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "DocumentTemplates" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DocumentTemplates" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "TemplateType" INTEGER NOT NULL,
    "FileContent" BLOB NOT NULL,
    "FileName" TEXT NOT NULL,
    "FileSize" INTEGER NOT NULL,
    "MimeType" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "PlaceholderVariables" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "ExchangeRateDownloadLogs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ExchangeRateDownloadLogs" PRIMARY KEY AUTOINCREMENT,
    "BaseCurrency" TEXT NOT NULL,
    "TargetCurrency" TEXT NULL,
    "Source" INTEGER NOT NULL,
    "Success" INTEGER NOT NULL,
    "DownloadTimestamp" TEXT NOT NULL,
    "RatesDownloaded" INTEGER NOT NULL,
    "RatesAdded" INTEGER NOT NULL,
    "RatesUpdated" INTEGER NOT NULL,
    "ErrorMessage" TEXT NULL,
    "ErrorCode" TEXT NULL,
    "DurationMs" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "IsStartupDownload" INTEGER NOT NULL,
    "IsScheduledDownload" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "HostingPackages" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HostingPackages" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "NormalizedName" TEXT NOT NULL,
    "DiskSpaceMB" INTEGER NOT NULL,
    "BandwidthMB" INTEGER NOT NULL,
    "EmailAccounts" INTEGER NOT NULL,
    "Databases" INTEGER NOT NULL,
    "Domains" INTEGER NOT NULL,
    "Subdomains" INTEGER NOT NULL,
    "FtpAccounts" INTEGER NOT NULL,
    "SslSupport" INTEGER NOT NULL,
    "BackupSupport" INTEGER NOT NULL,
    "DedicatedIp" INTEGER NOT NULL,
    "MonthlyPrice" TEXT NOT NULL,
    "YearlyPrice" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "HostProviders" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HostProviders" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "DisplayName" TEXT NOT NULL,
    "Description" TEXT NULL,
    "WebsiteUrl" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "MyCompanies" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MyCompanies" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "LegalName" TEXT NULL,
    "Email" TEXT NULL,
    "Phone" TEXT NULL,
    "AddressLine1" TEXT NULL,
    "AddressLine2" TEXT NULL,
    "PostalCode" TEXT NULL,
    "City" TEXT NULL,
    "State" TEXT NULL,
    "CountryCode" TEXT NULL,
    "OrganizationNumber" TEXT NULL,
    "TaxId" TEXT NULL,
    "VatNumber" TEXT NULL,
    "InvoiceEmail" TEXT NULL,
    "Website" TEXT NULL,
    "LogoUrl" TEXT NULL,
    "LetterheadFooter" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "OperatingSystems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OperatingSystems" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "DisplayName" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Version" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "OutboxEvents" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OutboxEvents" PRIMARY KEY AUTOINCREMENT,
    "EventId" TEXT NOT NULL,
    "EventType" TEXT NOT NULL,
    "Payload" TEXT NOT NULL,
    "OccurredAt" TEXT NOT NULL,
    "ProcessedAt" TEXT NULL,
    "RetryCount" INTEGER NOT NULL,
    "ErrorMessage" TEXT NULL,
    "CorrelationId" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "ProfitMarginSettings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ProfitMarginSettings" PRIMARY KEY AUTOINCREMENT,
    "ProductClass" INTEGER NOT NULL,
    "ProfitPercent" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "Registrars" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Registrars" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Code" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "ContactEmail" TEXT NULL,
    "ContactPhone" TEXT NULL,
    "Website" TEXT NULL,
    "Notes" TEXT NULL,
    "IsDefault" INTEGER NOT NULL,
    "NormalizedName" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "ReportTemplates" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ReportTemplates" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "TemplateType" INTEGER NOT NULL,
    "ReportEngine" TEXT NOT NULL,
    "FileContent" BLOB NOT NULL,
    "FileName" TEXT NOT NULL,
    "FileSize" INTEGER NOT NULL,
    "MimeType" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "DataSourceInfo" TEXT NOT NULL,
    "Version" TEXT NOT NULL,
    "Tags" TEXT NOT NULL,
    "DefaultExportFormat" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "ResellerCompanies" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ResellerCompanies" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "ContactPerson" TEXT NULL,
    "Email" TEXT NULL,
    "Phone" TEXT NULL,
    "Address" TEXT NULL,
    "City" TEXT NULL,
    "State" TEXT NULL,
    "PostalCode" TEXT NULL,
    "CountryCode" TEXT NULL,
    "CompanyRegistrationNumber" TEXT NULL,
    "TaxId" TEXT NULL,
    "VatNumber" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "DefaultCurrency" TEXT NOT NULL,
    "SupportedCurrencies" TEXT NULL,
    "ApplyCurrencyMarkup" INTEGER NOT NULL,
    "DefaultCurrencyMarkup" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "Roles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Roles" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Code" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "ServerTypes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ServerTypes" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "DisplayName" TEXT NOT NULL,
    "Description" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "ServiceTypes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ServiceTypes" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "SystemSettings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_SystemSettings" PRIMARY KEY AUTOINCREMENT,
    "Key" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "IsSystemKey" INTEGER NOT NULL DEFAULT 0,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "TaxCategories" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TaxCategories" PRIMARY KEY AUTOINCREMENT,
    "Code" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "CountryCode" TEXT NOT NULL,
    "StateCode" TEXT NULL,
    "Description" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "TaxDeterminationEvidences" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TaxDeterminationEvidences" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NULL,
    "OrderId" INTEGER NULL,
    "BuyerCountryCode" TEXT NOT NULL,
    "BuyerStateCode" TEXT NULL,
    "BillingCountryCode" TEXT NOT NULL,
    "IpAddress" TEXT NOT NULL,
    "BuyerTaxId" TEXT NOT NULL,
    "BuyerTaxIdValidated" INTEGER NOT NULL,
    "VatValidationProvider" TEXT NOT NULL,
    "VatValidationRawResponse" TEXT NOT NULL,
    "ExchangeRateSource" INTEGER NULL,
    "CapturedAt" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "TaxJurisdictions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TaxJurisdictions" PRIMARY KEY AUTOINCREMENT,
    "Code" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "CountryCode" TEXT NOT NULL,
    "StateCode" TEXT NULL,
    "TaxAuthority" TEXT NOT NULL,
    "TaxCurrencyCode" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "Tlds" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tlds" PRIMARY KEY AUTOINCREMENT,
    "Extension" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "RulesUrl" TEXT NOT NULL,
    "IsSecondLevel" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "DefaultRegistrationYears" INTEGER NULL,
    "MaxRegistrationYears" INTEGER NULL,
    "RequiresPrivacy" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "Units" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Units" PRIMARY KEY AUTOINCREMENT,
    "Code" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "VendorTaxProfiles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_VendorTaxProfiles" PRIMARY KEY AUTOINCREMENT,
    "VendorId" INTEGER NOT NULL,
    "VendorType" INTEGER NOT NULL,
    "TaxIdNumber" TEXT NULL,
    "TaxResidenceCountry" TEXT NOT NULL,
    "Require1099" INTEGER NOT NULL,
    "W9OnFile" INTEGER NOT NULL,
    "W9FileUrl" TEXT NULL,
    "WithholdingTaxRate" TEXT NULL,
    "TaxTreatyExempt" INTEGER NOT NULL,
    "TaxTreatyCountry" TEXT NULL,
    "TaxNotes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

CREATE TABLE "PostalCodes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PostalCodes" PRIMARY KEY AUTOINCREMENT,
    "Code" TEXT NOT NULL,
    "CountryCode" varchar(2) NOT NULL,
    "City" TEXT NOT NULL,
    "State" TEXT NULL,
    "Region" TEXT NULL,
    "District" TEXT NULL,
    "Latitude" TEXT NULL,
    "Longitude" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "NormalizedCode" TEXT NOT NULL,
    "NormalizedCountryCode" TEXT NOT NULL,
    "NormalizedCity" TEXT NOT NULL,
    "NormalizedState" TEXT NULL,
    "NormalizedRegion" TEXT NULL,
    "NormalizedDistrict" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_PostalCodes_Countries_CountryCode" FOREIGN KEY ("CountryCode") REFERENCES "Countries" ("Code") ON DELETE RESTRICT
);

CREATE TABLE "Customers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Customers" PRIMARY KEY AUTOINCREMENT,
    "ReferenceNumber" INTEGER NOT NULL,
    "CustomerNumber" INTEGER NULL,
    "Name" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Phone" TEXT NOT NULL,
    "CountryCode" TEXT NULL,
    "CustomerName" TEXT NULL,
    "TaxId" TEXT NULL,
    "VatNumber" TEXT NULL,
    "IsCompany" INTEGER NOT NULL,
    "IsSelfRegistered" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Status" TEXT NOT NULL,
    "CustomerStatusId" INTEGER NULL,
    "NormalizedName" TEXT NOT NULL,
    "NormalizedCustomerName" TEXT NULL,
    "Balance" TEXT NOT NULL,
    "CreditLimit" TEXT NOT NULL,
    "Notes" TEXT NULL,
    "BillingEmail" TEXT NULL,
    "PreferredPaymentMethod" TEXT NULL,
    "PreferredCurrency" TEXT NOT NULL,
    "AllowCurrencyOverride" INTEGER NOT NULL,
    "CountryId" INTEGER NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Customers_Countries_CountryId" FOREIGN KEY ("CountryId") REFERENCES "Countries" ("Id"),
    CONSTRAINT "FK_Customers_CustomerStatuses_CustomerStatusId" FOREIGN KEY ("CustomerStatusId") REFERENCES "CustomerStatuses" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "RegistrarSelectionPreferences" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RegistrarSelectionPreferences" PRIMARY KEY AUTOINCREMENT,
    "RegistrarId" INTEGER NOT NULL,
    "Priority" INTEGER NOT NULL,
    "OffersHosting" INTEGER NOT NULL,
    "OffersEmail" INTEGER NOT NULL,
    "OffersSsl" INTEGER NOT NULL,
    "MaxCostDifferenceThreshold" TEXT NULL,
    "PreferForHostingCustomers" INTEGER NOT NULL,
    "PreferForEmailCustomers" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_RegistrarSelectionPreferences_Registrars_RegistrarId" FOREIGN KEY ("RegistrarId") REFERENCES "Registrars" ("Id") ON DELETE CASCADE
);

CREATE TABLE "RegistrarTldPriceDownloadSessions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RegistrarTldPriceDownloadSessions" PRIMARY KEY AUTOINCREMENT,
    "RegistrarId" INTEGER NOT NULL,
    "StartedAtUtc" TEXT NOT NULL,
    "CompletedAtUtc" TEXT NULL,
    "TriggerSource" TEXT NOT NULL,
    "Success" INTEGER NOT NULL,
    "TldsProcessed" INTEGER NOT NULL,
    "PriceChangesDetected" INTEGER NOT NULL,
    "Message" TEXT NULL,
    "ErrorMessage" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_RegistrarTldPriceDownloadSessions_Registrars_RegistrarId" FOREIGN KEY ("RegistrarId") REFERENCES "Registrars" ("Id") ON DELETE CASCADE
);

CREATE TABLE "SalesAgents" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_SalesAgents" PRIMARY KEY AUTOINCREMENT,
    "ResellerCompanyId" INTEGER NULL,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Email" TEXT NULL,
    "Phone" TEXT NULL,
    "MobilePhone" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "NormalizedFirstName" TEXT NOT NULL,
    "NormalizedLastName" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_SalesAgents_ResellerCompanies_ResellerCompanyId" FOREIGN KEY ("ResellerCompanyId") REFERENCES "ResellerCompanies" ("Id") ON DELETE SET NULL
);

CREATE TABLE "Servers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Servers" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "ServerTypeId" INTEGER NOT NULL,
    "HostProviderId" INTEGER NULL,
    "Location" TEXT NULL,
    "OperatingSystemId" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL,
    "CpuCores" INTEGER NULL,
    "RamMB" INTEGER NULL,
    "DiskSpaceGB" INTEGER NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Servers_HostProviders_HostProviderId" FOREIGN KEY ("HostProviderId") REFERENCES "HostProviders" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Servers_OperatingSystems_OperatingSystemId" FOREIGN KEY ("OperatingSystemId") REFERENCES "OperatingSystems" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Servers_ServerTypes_ServerTypeId" FOREIGN KEY ("ServerTypeId") REFERENCES "ServerTypes" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "TaxRegistrations" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TaxRegistrations" PRIMARY KEY AUTOINCREMENT,
    "TaxJurisdictionId" INTEGER NOT NULL,
    "LegalEntityName" TEXT NOT NULL,
    "RegistrationNumber" TEXT NOT NULL,
    "EffectiveFrom" TEXT NOT NULL,
    "EffectiveUntil" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_TaxRegistrations_TaxJurisdictions_TaxJurisdictionId" FOREIGN KEY ("TaxJurisdictionId") REFERENCES "TaxJurisdictions" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "TaxRules" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TaxRules" PRIMARY KEY AUTOINCREMENT,
    "TaxJurisdictionId" INTEGER NULL,
    "TaxCategoryId" INTEGER NULL,
    "CountryCode" TEXT NOT NULL,
    "StateCode" TEXT NULL,
    "TaxName" TEXT NOT NULL,
    "TaxCategory" TEXT NOT NULL,
    "TaxRate" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "EffectiveFrom" TEXT NOT NULL,
    "EffectiveUntil" TEXT NULL,
    "AppliesToSetupFees" INTEGER NOT NULL,
    "AppliesToRecurring" INTEGER NOT NULL,
    "ReverseCharge" INTEGER NOT NULL,
    "TaxAuthority" TEXT NOT NULL,
    "TaxRegistrationNumber" TEXT NOT NULL,
    "Priority" INTEGER NOT NULL,
    "InternalNotes" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_TaxRules_TaxCategories_TaxCategoryId" FOREIGN KEY ("TaxCategoryId") REFERENCES "TaxCategories" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_TaxRules_TaxJurisdictions_TaxJurisdictionId" FOREIGN KEY ("TaxJurisdictionId") REFERENCES "TaxJurisdictions" ("Id") ON DELETE SET NULL
);

CREATE TABLE "RegistrarTlds" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RegistrarTlds" PRIMARY KEY AUTOINCREMENT,
    "RegistrarId" INTEGER NOT NULL,
    "TldId" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "AutoRenew" INTEGER NOT NULL,
    "MinRegistrationYears" INTEGER NULL,
    "MaxRegistrationYears" INTEGER NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_RegistrarTlds_Registrars_RegistrarId" FOREIGN KEY ("RegistrarId") REFERENCES "Registrars" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_RegistrarTlds_Tlds_TldId" FOREIGN KEY ("TldId") REFERENCES "Tlds" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "ResellerTldDiscounts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ResellerTldDiscounts" PRIMARY KEY AUTOINCREMENT,
    "ResellerCompanyId" INTEGER NOT NULL,
    "TldId" INTEGER NOT NULL,
    "EffectiveFrom" TEXT NOT NULL,
    "EffectiveTo" TEXT NULL,
    "DiscountPercentage" TEXT NULL,
    "DiscountAmount" TEXT NULL,
    "DiscountCurrency" TEXT NULL,
    "ApplyToRegistration" INTEGER NOT NULL,
    "ApplyToRenewal" INTEGER NOT NULL,
    "ApplyToTransfer" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "CreatedBy" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_ResellerTldDiscounts_ResellerCompanies_ResellerCompanyId" FOREIGN KEY ("ResellerCompanyId") REFERENCES "ResellerCompanies" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ResellerTldDiscounts_Tlds_TldId" FOREIGN KEY ("TldId") REFERENCES "Tlds" ("Id") ON DELETE CASCADE
);

CREATE TABLE "TldRegistryRules" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TldRegistryRules" PRIMARY KEY AUTOINCREMENT,
    "TldId" INTEGER NOT NULL,
    "RequireRegistrantContact" INTEGER NOT NULL,
    "RequireAdministrativeContact" INTEGER NOT NULL,
    "RequireTechnicalContact" INTEGER NOT NULL,
    "RequireBillingContact" INTEGER NOT NULL,
    "RequiresAuthCodeForTransfer" INTEGER NOT NULL,
    "TransferLockDays" INTEGER NULL,
    "RenewalGracePeriodDays" INTEGER NULL,
    "RedemptionGracePeriodDays" INTEGER NULL,
    "Notes" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_TldRegistryRules_Tlds_TldId" FOREIGN KEY ("TldId") REFERENCES "Tlds" ("Id") ON DELETE CASCADE
);

CREATE TABLE "TldSalesPricing" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TldSalesPricing" PRIMARY KEY AUTOINCREMENT,
    "TldId" INTEGER NOT NULL,
    "EffectiveFrom" TEXT NOT NULL,
    "EffectiveTo" TEXT NULL,
    "RegistrationPrice" TEXT NOT NULL,
    "RenewalPrice" TEXT NOT NULL,
    "TransferPrice" TEXT NOT NULL,
    "PrivacyPrice" TEXT NULL,
    "FirstYearRegistrationPrice" TEXT NULL,
    "Currency" TEXT NOT NULL,
    "IsPromotional" INTEGER NOT NULL,
    "PromotionName" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "CreatedBy" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_TldSalesPricing_Tlds_TldId" FOREIGN KEY ("TldId") REFERENCES "Tlds" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ContactPersons" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ContactPersons" PRIMARY KEY AUTOINCREMENT,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Phone" TEXT NOT NULL,
    "Position" TEXT NULL,
    "Department" TEXT NULL,
    "IsPrimary" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "IsDefaultOwner" INTEGER NOT NULL,
    "IsDefaultBilling" INTEGER NOT NULL,
    "IsDefaultTech" INTEGER NOT NULL,
    "IsDefaultAdministrator" INTEGER NOT NULL,
    "IsDomainGlobal" INTEGER NOT NULL,
    "NormalizedFirstName" TEXT NOT NULL,
    "NormalizedLastName" TEXT NOT NULL,
    "CustomerId" INTEGER NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_ContactPersons_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CustomerAddresses" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CustomerAddresses" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "AddressTypeId" INTEGER NOT NULL,
    "PostalCodeId" INTEGER NOT NULL,
    "AddressLine1" TEXT NOT NULL,
    "AddressLine2" TEXT NULL,
    "AddressLine3" TEXT NULL,
    "AddressLine4" TEXT NULL,
    "IsPrimary" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CustomerAddresses_AddressTypes_AddressTypeId" FOREIGN KEY ("AddressTypeId") REFERENCES "AddressTypes" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_CustomerAddresses_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CustomerAddresses_PostalCodes_PostalCodeId" FOREIGN KEY ("PostalCodeId") REFERENCES "PostalCodes" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "CustomerCredits" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CustomerCredits" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "Balance" TEXT NOT NULL,
    "CurrencyCode" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CustomerCredits_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CustomerTaxProfiles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CustomerTaxProfiles" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "TaxIdNumber" TEXT NULL,
    "TaxIdType" INTEGER NOT NULL,
    "TaxIdValidated" INTEGER NOT NULL,
    "TaxIdValidationDate" TEXT NULL,
    "TaxIdValidationResponse" TEXT NOT NULL,
    "TaxResidenceCountry" TEXT NOT NULL,
    "CustomerType" INTEGER NOT NULL,
    "TaxExempt" INTEGER NOT NULL,
    "TaxExemptionReason" TEXT NULL,
    "TaxExemptionCertificateUrl" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CustomerTaxProfiles_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "RegistrarMailAddresses" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RegistrarMailAddresses" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "MailAddress" TEXT NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_RegistrarMailAddresses_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NULL,
    "Username" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "EmailConfirmed" TEXT NULL,
    "IsMailTwoFactorEnabled" INTEGER NOT NULL,
    "IsAuthenticatorTwoFactorEnabled" INTEGER NOT NULL,
    "AuthenticatorKey" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "NormalizedUsername" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Users_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "DnsZonePackages" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DnsZonePackages" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "SortOrder" INTEGER NOT NULL,
    "ResellerCompanyId" INTEGER NULL,
    "SalesAgentId" INTEGER NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_DnsZonePackages_ResellerCompanies_ResellerCompanyId" FOREIGN KEY ("ResellerCompanyId") REFERENCES "ResellerCompanies" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_DnsZonePackages_SalesAgents_SalesAgentId" FOREIGN KEY ("SalesAgentId") REFERENCES "SalesAgents" ("Id") ON DELETE SET NULL
);

CREATE TABLE "Services" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Services" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "ServiceTypeId" INTEGER NOT NULL,
    "BillingCycleId" INTEGER NULL,
    "Price" TEXT NULL,
    "SetupFee" TEXT NULL,
    "TrialDays" INTEGER NULL,
    "IsActive" INTEGER NOT NULL,
    "IsFeatured" INTEGER NOT NULL,
    "Sku" TEXT NOT NULL,
    "ResellerCompanyId" INTEGER NULL,
    "SalesAgentId" INTEGER NULL,
    "MaxQuantity" INTEGER NULL,
    "MinQuantity" INTEGER NOT NULL,
    "SortOrder" INTEGER NOT NULL,
    "SpecificationsJson" TEXT NOT NULL,
    "HostingPackageId" INTEGER NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Services_BillingCycles_BillingCycleId" FOREIGN KEY ("BillingCycleId") REFERENCES "BillingCycles" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Services_HostingPackages_HostingPackageId" FOREIGN KEY ("HostingPackageId") REFERENCES "HostingPackages" ("Id"),
    CONSTRAINT "FK_Services_ResellerCompanies_ResellerCompanyId" FOREIGN KEY ("ResellerCompanyId") REFERENCES "ResellerCompanies" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Services_SalesAgents_SalesAgentId" FOREIGN KEY ("SalesAgentId") REFERENCES "SalesAgents" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Services_ServiceTypes_ServiceTypeId" FOREIGN KEY ("ServiceTypeId") REFERENCES "ServiceTypes" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "NameServers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_NameServers" PRIMARY KEY AUTOINCREMENT,
    "ServerId" INTEGER NULL,
    "Hostname" TEXT NOT NULL,
    "IpAddress" TEXT NULL,
    "IsPrimary" INTEGER NOT NULL,
    "SortOrder" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_NameServers_Servers_ServerId" FOREIGN KEY ("ServerId") REFERENCES "Servers" ("Id") ON DELETE SET NULL
);

CREATE TABLE "ServerIpAddresses" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ServerIpAddresses" PRIMARY KEY AUTOINCREMENT,
    "ServerId" INTEGER NOT NULL,
    "IpAddress" TEXT NOT NULL,
    "IpVersion" TEXT NOT NULL,
    "IsPrimary" INTEGER NOT NULL,
    "Status" TEXT NOT NULL,
    "AssignedTo" TEXT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_ServerIpAddresses_Servers_ServerId" FOREIGN KEY ("ServerId") REFERENCES "Servers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "RegistrarTldCostPricing" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RegistrarTldCostPricing" PRIMARY KEY AUTOINCREMENT,
    "RegistrarTldId" INTEGER NOT NULL,
    "EffectiveFrom" TEXT NOT NULL,
    "EffectiveTo" TEXT NULL,
    "RegistrationCost" TEXT NOT NULL,
    "RenewalCost" TEXT NOT NULL,
    "TransferCost" TEXT NOT NULL,
    "PrivacyCost" TEXT NULL,
    "FirstYearRegistrationCost" TEXT NULL,
    "Currency" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "CreatedBy" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_RegistrarTldCostPricing_RegistrarTlds_RegistrarTldId" FOREIGN KEY ("RegistrarTldId") REFERENCES "RegistrarTlds" ("Id") ON DELETE CASCADE
);

CREATE TABLE "RegistrarTldPriceChangeLogs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RegistrarTldPriceChangeLogs" PRIMARY KEY AUTOINCREMENT,
    "RegistrarTldId" INTEGER NOT NULL,
    "DownloadSessionId" INTEGER NULL,
    "ChangeSource" TEXT NOT NULL,
    "ChangedBy" TEXT NULL,
    "ChangedAtUtc" TEXT NOT NULL,
    "OldRegistrationCost" TEXT NULL,
    "NewRegistrationCost" TEXT NULL,
    "OldRenewalCost" TEXT NULL,
    "NewRenewalCost" TEXT NULL,
    "OldTransferCost" TEXT NULL,
    "NewTransferCost" TEXT NULL,
    "OldCurrency" TEXT NULL,
    "NewCurrency" TEXT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_RegistrarTldPriceChangeLogs_RegistrarTldPriceDownloadSessions_DownloadSessionId" FOREIGN KEY ("DownloadSessionId") REFERENCES "RegistrarTldPriceDownloadSessions" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_RegistrarTldPriceChangeLogs_RegistrarTlds_RegistrarTldId" FOREIGN KEY ("RegistrarTldId") REFERENCES "RegistrarTlds" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AuditLogs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AuditLogs" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "ActionType" TEXT NOT NULL,
    "EntityType" TEXT NOT NULL,
    "EntityId" TEXT NOT NULL,
    "Timestamp" TEXT NOT NULL,
    "Details" TEXT NOT NULL,
    "IPAddress" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_AuditLogs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "CommunicationThreads" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CommunicationThreads" PRIMARY KEY AUTOINCREMENT,
    "Subject" TEXT NOT NULL,
    "CustomerId" INTEGER NULL,
    "UserId" INTEGER NULL,
    "RelatedEntityType" TEXT NULL,
    "RelatedEntityId" INTEGER NULL,
    "LastMessageAtUtc" TEXT NULL,
    "Status" TEXT NOT NULL DEFAULT 'Open',
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CommunicationThreads_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_CommunicationThreads_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE SET NULL
);

CREATE TABLE "CustomerChanges" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CustomerChanges" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "ChangeType" TEXT NOT NULL,
    "FieldName" TEXT NULL,
    "OldValue" TEXT NULL,
    "NewValue" TEXT NULL,
    "ChangedAt" TEXT NOT NULL,
    "ChangedByUserId" INTEGER NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CustomerChanges_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CustomerChanges_Users_ChangedByUserId" FOREIGN KEY ("ChangedByUserId") REFERENCES "Users" ("Id") ON DELETE SET NULL
);

CREATE TABLE "CustomerInternalNotes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CustomerInternalNotes" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "Note" TEXT NOT NULL,
    "CreatedByUserId" INTEGER NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CustomerInternalNotes_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CustomerInternalNotes_Users_CreatedByUserId" FOREIGN KEY ("CreatedByUserId") REFERENCES "Users" ("Id") ON DELETE SET NULL
);

CREATE TABLE "LoginHistories" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_LoginHistories" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NULL,
    "Identifier" TEXT NOT NULL,
    "IsSuccessful" INTEGER NOT NULL,
    "AttemptedAt" TEXT NOT NULL,
    "IPAddress" TEXT NOT NULL,
    "UserAgent" TEXT NOT NULL,
    "FailureReason" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_LoginHistories_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE SET NULL
);

CREATE TABLE "Quotes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Quotes" PRIMARY KEY AUTOINCREMENT,
    "QuoteNumber" TEXT NOT NULL,
    "CustomerId" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL,
    "ValidUntil" TEXT NOT NULL,
    "SubTotal" TEXT NOT NULL,
    "TotalSetupFee" TEXT NOT NULL,
    "TotalRecurring" TEXT NOT NULL,
    "TaxAmount" TEXT NOT NULL,
    "TotalAmount" TEXT NOT NULL,
    "CurrencyCode" TEXT NOT NULL,
    "TaxRate" TEXT NOT NULL,
    "TaxName" TEXT NOT NULL,
    "CustomerName" TEXT NOT NULL,
    "CustomerAddress" TEXT NOT NULL,
    "CustomerTaxId" TEXT NOT NULL,
    "Notes" TEXT NOT NULL,
    "TermsAndConditions" TEXT NOT NULL,
    "InternalComment" TEXT NOT NULL,
    "SentAt" TEXT NULL,
    "AcceptedAt" TEXT NULL,
    "RejectedAt" TEXT NULL,
    "RejectionReason" TEXT NOT NULL,
    "AcceptanceToken" TEXT NOT NULL,
    "PreparedByUserId" INTEGER NULL,
    "CouponId" INTEGER NULL,
    "DiscountAmount" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Quotes_Coupons_CouponId" FOREIGN KEY ("CouponId") REFERENCES "Coupons" ("Id"),
    CONSTRAINT "FK_Quotes_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Quotes_Users_PreparedByUserId" FOREIGN KEY ("PreparedByUserId") REFERENCES "Users" ("Id")
);

CREATE TABLE "SentEmails" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_SentEmails" PRIMARY KEY AUTOINCREMENT,
    "SentDate" TEXT NULL,
    "From" TEXT NOT NULL,
    "To" TEXT NOT NULL,
    "Cc" TEXT NULL,
    "Bcc" TEXT NULL,
    "Subject" TEXT NOT NULL,
    "BodyText" TEXT NULL,
    "BodyHtml" TEXT NULL,
    "MessageId" TEXT NOT NULL,
    "Status" TEXT NOT NULL DEFAULT 'Pending',
    "ErrorMessage" TEXT NULL,
    "RetryCount" INTEGER NOT NULL DEFAULT 0,
    "MaxRetries" INTEGER NOT NULL DEFAULT 3,
    "NextAttemptAt" TEXT NULL,
    "Provider" TEXT NULL,
    "CustomerId" INTEGER NULL,
    "UserId" INTEGER NULL,
    "RelatedEntityType" TEXT NULL,
    "RelatedEntityId" INTEGER NULL,
    "Attachments" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_SentEmails_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_SentEmails_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE SET NULL
);

CREATE TABLE "SupportTickets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_SupportTickets" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "CreatedByUserId" INTEGER NOT NULL,
    "AssignedToUserId" INTEGER NULL,
    "AssignedDepartment" TEXT NULL,
    "Subject" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "Priority" TEXT NOT NULL,
    "Source" TEXT NOT NULL,
    "LastMessageAt" TEXT NULL,
    "ClosedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_SupportTickets_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_SupportTickets_Users_AssignedToUserId" FOREIGN KEY ("AssignedToUserId") REFERENCES "Users" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_SupportTickets_Users_CreatedByUserId" FOREIGN KEY ("CreatedByUserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Tokens" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tokens" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "TokenType" TEXT NOT NULL,
    "TokenValue" TEXT NOT NULL,
    "Expiry" TEXT NOT NULL,
    "RevokedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Tokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "UserRoles" (
    "UserId" INTEGER NOT NULL,
    "RoleId" INTEGER NOT NULL,
    CONSTRAINT "PK_UserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_UserRoles_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Roles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserRoles_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "VendorPayouts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_VendorPayouts" PRIMARY KEY AUTOINCREMENT,
    "VendorId" INTEGER NOT NULL,
    "VendorType" INTEGER NOT NULL,
    "VendorName" TEXT NOT NULL,
    "PayoutMethod" INTEGER NOT NULL,
    "VendorCurrency" TEXT NOT NULL,
    "VendorAmount" TEXT NOT NULL,
    "BaseCurrency" TEXT NOT NULL,
    "BaseAmount" TEXT NOT NULL,
    "ExchangeRate" TEXT NOT NULL,
    "ExchangeRateDate" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "ScheduledDate" TEXT NOT NULL,
    "ProcessedDate" TEXT NULL,
    "FailureReason" TEXT NOT NULL,
    "FailureCount" INTEGER NOT NULL,
    "TransactionReference" TEXT NOT NULL,
    "PaymentGatewayResponse" TEXT NOT NULL,
    "RequiresManualIntervention" INTEGER NOT NULL,
    "InterventionReason" TEXT NOT NULL,
    "InterventionResolvedAt" TEXT NULL,
    "InterventionResolvedByUserId" INTEGER NULL,
    "InternalNotes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_VendorPayouts_Users_InterventionResolvedByUserId" FOREIGN KEY ("InterventionResolvedByUserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "DnsZonePackageRecords" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DnsZonePackageRecords" PRIMARY KEY AUTOINCREMENT,
    "DnsZonePackageId" INTEGER NOT NULL,
    "DnsRecordTypeId" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    "ValueSourceType" TEXT NOT NULL DEFAULT 'Manual',
    "ValueSourceReference" TEXT NULL,
    "TTL" INTEGER NOT NULL,
    "Priority" INTEGER NULL,
    "Weight" INTEGER NULL,
    "Port" INTEGER NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_DnsZonePackageRecords_DnsRecordTypes_DnsRecordTypeId" FOREIGN KEY ("DnsRecordTypeId") REFERENCES "DnsRecordTypes" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_DnsZonePackageRecords_DnsZonePackages_DnsZonePackageId" FOREIGN KEY ("DnsZonePackageId") REFERENCES "DnsZonePackages" ("Id") ON DELETE CASCADE
);

CREATE TABLE "DnsZonePackageServers" (
    "DnsZonePackageId" INTEGER NOT NULL,
    "ServerId" INTEGER NOT NULL,
    CONSTRAINT "PK_DnsZonePackageServers" PRIMARY KEY ("DnsZonePackageId", "ServerId"),
    CONSTRAINT "FK_DnsZonePackageServers_DnsZonePackages_DnsZonePackageId" FOREIGN KEY ("DnsZonePackageId") REFERENCES "DnsZonePackages" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_DnsZonePackageServers_Servers_ServerId" FOREIGN KEY ("ServerId") REFERENCES "Servers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "RegisteredDomains" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RegisteredDomains" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "ServiceId" INTEGER NULL,
    "Name" TEXT NOT NULL,
    "RegistrarId" INTEGER NOT NULL,
    "RegistrarTldId" INTEGER NULL,
    "Status" TEXT NOT NULL,
    "RegistrationStatus" INTEGER NOT NULL DEFAULT 0,
    "RegistrationDate" TEXT NULL,
    "RegistrationAttemptCount" INTEGER NOT NULL,
    "LastRegistrationAttemptUtc" TEXT NULL,
    "NextRegistrationAttemptUtc" TEXT NULL,
    "RegistrationError" TEXT NULL,
    "ExpirationDate" TEXT NULL,
    "AutoRenew" INTEGER NOT NULL,
    "PrivacyProtection" INTEGER NOT NULL,
    "RegistrationPrice" TEXT NULL,
    "RenewalPrice" TEXT NULL,
    "Notes" TEXT NULL,
    "NormalizedName" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_RegisteredDomains_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_RegisteredDomains_RegistrarTlds_RegistrarTldId" FOREIGN KEY ("RegistrarTldId") REFERENCES "RegistrarTlds" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_RegisteredDomains_Registrars_RegistrarId" FOREIGN KEY ("RegistrarId") REFERENCES "Registrars" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_RegisteredDomains_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES "Services" ("Id") ON DELETE SET NULL
);

CREATE TABLE "ServerControlPanels" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ServerControlPanels" PRIMARY KEY AUTOINCREMENT,
    "ServerId" INTEGER NOT NULL,
    "ControlPanelTypeId" INTEGER NOT NULL,
    "ApiUrl" TEXT NOT NULL,
    "Port" INTEGER NOT NULL,
    "UseHttps" INTEGER NOT NULL,
    "ApiToken" TEXT NULL,
    "ApiKey" TEXT NULL,
    "Username" TEXT NULL,
    "PasswordHash" TEXT NULL,
    "AdditionalSettings" TEXT NULL,
    "Status" TEXT NOT NULL,
    "LastConnectionTest" TEXT NULL,
    "IsConnectionHealthy" INTEGER NULL,
    "LastError" TEXT NULL,
    "Notes" TEXT NULL,
    "IpAddressId" INTEGER NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_ServerControlPanels_ControlPanelTypes_ControlPanelTypeId" FOREIGN KEY ("ControlPanelTypeId") REFERENCES "ControlPanelTypes" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_ServerControlPanels_ServerIpAddresses_IpAddressId" FOREIGN KEY ("IpAddressId") REFERENCES "ServerIpAddresses" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_ServerControlPanels_Servers_ServerId" FOREIGN KEY ("ServerId") REFERENCES "Servers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CommunicationParticipants" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CommunicationParticipants" PRIMARY KEY AUTOINCREMENT,
    "CommunicationThreadId" INTEGER NOT NULL,
    "EmailAddress" TEXT NOT NULL,
    "DisplayName" TEXT NULL,
    "Role" TEXT NOT NULL,
    "IsPrimary" INTEGER NOT NULL DEFAULT 0,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CommunicationParticipants_CommunicationThreads_CommunicationThreadId" FOREIGN KEY ("CommunicationThreadId") REFERENCES "CommunicationThreads" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Orders" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY AUTOINCREMENT,
    "OrderNumber" TEXT NOT NULL,
    "CustomerId" INTEGER NOT NULL,
    "ServiceId" INTEGER NULL,
    "QuoteId" INTEGER NULL,
    "CouponId" INTEGER NULL,
    "OrderType" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL,
    "StartDate" TEXT NOT NULL,
    "EndDate" TEXT NOT NULL,
    "NextBillingDate" TEXT NOT NULL,
    "SetupFee" TEXT NOT NULL,
    "RecurringAmount" TEXT NOT NULL,
    "DiscountAmount" TEXT NOT NULL,
    "CurrencyCode" TEXT NOT NULL,
    "BaseCurrencyCode" TEXT NOT NULL,
    "ExchangeRate" TEXT NULL,
    "ExchangeRateDate" TEXT NULL,
    "TrialEndsAt" TEXT NULL,
    "SuspendedAt" TEXT NULL,
    "SuspensionReason" TEXT NOT NULL,
    "CancelledAt" TEXT NULL,
    "CancellationReason" TEXT NOT NULL,
    "AutoRenew" INTEGER NOT NULL,
    "RenewalReminderSent" INTEGER NOT NULL,
    "Notes" TEXT NOT NULL,
    "InternalComment" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Orders_Coupons_CouponId" FOREIGN KEY ("CouponId") REFERENCES "Coupons" ("Id"),
    CONSTRAINT "FK_Orders_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Orders_Quotes_QuoteId" FOREIGN KEY ("QuoteId") REFERENCES "Quotes" ("Id"),
    CONSTRAINT "FK_Orders_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES "Services" ("Id") ON DELETE SET NULL
);

CREATE TABLE "QuoteLines" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_QuoteLines" PRIMARY KEY AUTOINCREMENT,
    "QuoteId" INTEGER NOT NULL,
    "ServiceId" INTEGER NULL,
    "BillingCycleId" INTEGER NOT NULL,
    "LineNumber" INTEGER NOT NULL,
    "Description" TEXT NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "SetupFee" TEXT NOT NULL,
    "RecurringPrice" TEXT NOT NULL,
    "Discount" TEXT NOT NULL,
    "TotalSetupFee" TEXT NOT NULL,
    "TotalRecurringPrice" TEXT NOT NULL,
    "TaxRate" TEXT NOT NULL,
    "TaxAmount" TEXT NOT NULL,
    "TotalWithTax" TEXT NOT NULL,
    "ServiceNameSnapshot" TEXT NOT NULL,
    "BillingCycleNameSnapshot" TEXT NOT NULL,
    "Notes" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_QuoteLines_BillingCycles_BillingCycleId" FOREIGN KEY ("BillingCycleId") REFERENCES "BillingCycles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_QuoteLines_Quotes_QuoteId" FOREIGN KEY ("QuoteId") REFERENCES "Quotes" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_QuoteLines_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES "Services" ("Id")
);

CREATE TABLE "QuoteRevisions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_QuoteRevisions" PRIMARY KEY AUTOINCREMENT,
    "QuoteId" INTEGER NOT NULL,
    "RevisionNumber" INTEGER NOT NULL,
    "QuoteStatus" INTEGER NOT NULL,
    "ActionType" TEXT NOT NULL,
    "SnapshotJson" TEXT NOT NULL,
    "PdfFileName" TEXT NOT NULL,
    "PdfFilePath" TEXT NOT NULL,
    "ContentHash" TEXT NOT NULL,
    "Notes" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_QuoteRevisions_Quotes_QuoteId" FOREIGN KEY ("QuoteId") REFERENCES "Quotes" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CommunicationMessages" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CommunicationMessages" PRIMARY KEY AUTOINCREMENT,
    "CommunicationThreadId" INTEGER NOT NULL,
    "Direction" TEXT NOT NULL,
    "ExternalMessageId" TEXT NULL,
    "InternetMessageId" TEXT NULL,
    "FromAddress" TEXT NOT NULL,
    "ToAddresses" TEXT NOT NULL,
    "CcAddresses" TEXT NULL,
    "BccAddresses" TEXT NULL,
    "Subject" TEXT NOT NULL,
    "BodyText" TEXT NULL,
    "BodyHtml" TEXT NULL,
    "Provider" TEXT NULL,
    "SentEmailId" INTEGER NULL,
    "IsRead" INTEGER NOT NULL DEFAULT 0,
    "ReceivedAtUtc" TEXT NULL,
    "SentAtUtc" TEXT NULL,
    "ReadAtUtc" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CommunicationMessages_CommunicationThreads_CommunicationThreadId" FOREIGN KEY ("CommunicationThreadId") REFERENCES "CommunicationThreads" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CommunicationMessages_SentEmails_SentEmailId" FOREIGN KEY ("SentEmailId") REFERENCES "SentEmails" ("Id") ON DELETE SET NULL
);

CREATE TABLE "SupportTicketMessages" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_SupportTicketMessages" PRIMARY KEY AUTOINCREMENT,
    "SupportTicketId" INTEGER NOT NULL,
    "SenderUserId" INTEGER NOT NULL,
    "SenderRole" TEXT NOT NULL,
    "Message" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_SupportTicketMessages_SupportTickets_SupportTicketId" FOREIGN KEY ("SupportTicketId") REFERENCES "SupportTickets" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_SupportTicketMessages_Users_SenderUserId" FOREIGN KEY ("SenderUserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "DnsRecords" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DnsRecords" PRIMARY KEY AUTOINCREMENT,
    "DomainId" INTEGER NOT NULL,
    "DnsRecordTypeId" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    "TTL" INTEGER NOT NULL,
    "Priority" INTEGER NULL,
    "Weight" INTEGER NULL,
    "Port" INTEGER NULL,
    "IsPendingSync" INTEGER NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_DnsRecords_DnsRecordTypes_DnsRecordTypeId" FOREIGN KEY ("DnsRecordTypeId") REFERENCES "DnsRecordTypes" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_DnsRecords_RegisteredDomains_DomainId" FOREIGN KEY ("DomainId") REFERENCES "RegisteredDomains" ("Id") ON DELETE CASCADE
);

CREATE TABLE "DomainContactAssignments" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DomainContactAssignments" PRIMARY KEY AUTOINCREMENT,
    "RegisteredDomainId" INTEGER NOT NULL,
    "ContactPersonId" INTEGER NOT NULL,
    "RoleType" INTEGER NOT NULL,
    "AssignedAt" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_DomainContactAssignments_ContactPersons_ContactPersonId" FOREIGN KEY ("ContactPersonId") REFERENCES "ContactPersons" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_DomainContactAssignments_RegisteredDomains_RegisteredDomainId" FOREIGN KEY ("RegisteredDomainId") REFERENCES "RegisteredDomains" ("Id") ON DELETE CASCADE
);

CREATE TABLE "DomainContacts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DomainContacts" PRIMARY KEY AUTOINCREMENT,
    "RoleType" INTEGER NOT NULL,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Organization" TEXT NULL,
    "Email" TEXT NOT NULL,
    "Phone" TEXT NOT NULL,
    "Fax" TEXT NULL,
    "Address1" TEXT NOT NULL,
    "Address2" TEXT NULL,
    "City" TEXT NOT NULL,
    "State" TEXT NULL,
    "PostalCode" TEXT NOT NULL,
    "CountryCode" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "NormalizedFirstName" TEXT NOT NULL,
    "NormalizedLastName" TEXT NOT NULL,
    "NormalizedEmail" TEXT NOT NULL,
    "DomainId" INTEGER NOT NULL,
    "SourceContactPersonId" INTEGER NULL,
    "LastSyncedAt" TEXT NULL,
    "NeedsSync" INTEGER NOT NULL,
    "RegistrarContactId" TEXT NULL,
    "RegistrarType" TEXT NULL,
    "IsPrivacyProtected" INTEGER NOT NULL,
    "RegistrarSnapshot" TEXT NULL,
    "IsCurrentVersion" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_DomainContacts_ContactPersons_SourceContactPersonId" FOREIGN KEY ("SourceContactPersonId") REFERENCES "ContactPersons" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_DomainContacts_RegisteredDomains_DomainId" FOREIGN KEY ("DomainId") REFERENCES "RegisteredDomains" ("Id") ON DELETE CASCADE
);

CREATE TABLE "NameServerDomains" (
    "NameServerId" INTEGER NOT NULL,
    "DomainId" INTEGER NOT NULL,
    "Id" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "PK_NameServerDomains" PRIMARY KEY ("NameServerId", "DomainId"),
    CONSTRAINT "FK_NameServerDomains_NameServers_NameServerId" FOREIGN KEY ("NameServerId") REFERENCES "NameServers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_NameServerDomains_RegisteredDomains_DomainId" FOREIGN KEY ("DomainId") REFERENCES "RegisteredDomains" ("Id") ON DELETE CASCADE
);

CREATE TABLE "RegisteredDomainHistories" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RegisteredDomainHistories" PRIMARY KEY AUTOINCREMENT,
    "RegisteredDomainId" INTEGER NOT NULL,
    "ActionType" INTEGER NOT NULL,
    "Action" TEXT NOT NULL,
    "Details" TEXT NULL,
    "OccurredAt" TEXT NOT NULL,
    "SourceEntityType" TEXT NULL,
    "SourceEntityId" INTEGER NULL,
    "PerformedByUserId" INTEGER NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_RegisteredDomainHistories_RegisteredDomains_RegisteredDomainId" FOREIGN KEY ("RegisteredDomainId") REFERENCES "RegisteredDomains" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RegisteredDomainHistories_Users_PerformedByUserId" FOREIGN KEY ("PerformedByUserId") REFERENCES "Users" ("Id") ON DELETE SET NULL
);

CREATE TABLE "DnsZonePackageControlPanels" (
    "DnsZonePackageId" INTEGER NOT NULL,
    "ServerControlPanelId" INTEGER NOT NULL,
    CONSTRAINT "PK_DnsZonePackageControlPanels" PRIMARY KEY ("DnsZonePackageId", "ServerControlPanelId"),
    CONSTRAINT "FK_DnsZonePackageControlPanels_DnsZonePackages_DnsZonePackageId" FOREIGN KEY ("DnsZonePackageId") REFERENCES "DnsZonePackages" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_DnsZonePackageControlPanels_ServerControlPanels_ServerControlPanelId" FOREIGN KEY ("ServerControlPanelId") REFERENCES "ServerControlPanels" ("Id") ON DELETE CASCADE
);

CREATE TABLE "HostingAccounts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HostingAccounts" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "ServiceId" INTEGER NOT NULL,
    "ServerId" INTEGER NULL,
    "ServerControlPanelId" INTEGER NULL,
    "Provider" TEXT NOT NULL,
    "Username" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "ExpirationDate" TEXT NOT NULL,
    "ExternalAccountId" TEXT NULL,
    "LastSyncedAt" TEXT NULL,
    "SyncStatus" TEXT NULL,
    "ConfigurationJson" TEXT NULL,
    "DiskUsageMB" INTEGER NULL,
    "BandwidthUsageMB" INTEGER NULL,
    "DiskQuotaMB" INTEGER NULL,
    "BandwidthLimitMB" INTEGER NULL,
    "MaxEmailAccounts" INTEGER NULL,
    "MaxDatabases" INTEGER NULL,
    "MaxFtpAccounts" INTEGER NULL,
    "MaxSubdomains" INTEGER NULL,
    "PlanName" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_HostingAccounts_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_HostingAccounts_ServerControlPanels_ServerControlPanelId" FOREIGN KEY ("ServerControlPanelId") REFERENCES "ServerControlPanels" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_HostingAccounts_Servers_ServerId" FOREIGN KEY ("ServerId") REFERENCES "Servers" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_HostingAccounts_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES "Services" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "CouponUsages" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CouponUsages" PRIMARY KEY AUTOINCREMENT,
    "CouponId" INTEGER NOT NULL,
    "CustomerId" INTEGER NOT NULL,
    "QuoteId" INTEGER NULL,
    "OrderId" INTEGER NULL,
    "DiscountAmount" TEXT NOT NULL,
    "UsedAt" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CouponUsages_Coupons_CouponId" FOREIGN KEY ("CouponId") REFERENCES "Coupons" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CouponUsages_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CouponUsages_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id"),
    CONSTRAINT "FK_CouponUsages_Quotes_QuoteId" FOREIGN KEY ("QuoteId") REFERENCES "Quotes" ("Id")
);

CREATE TABLE "OrderLines" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OrderLines" PRIMARY KEY AUTOINCREMENT,
    "OrderId" INTEGER NOT NULL,
    "ServiceId" INTEGER NULL,
    "LineNumber" INTEGER NOT NULL,
    "Description" TEXT NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "UnitPrice" TEXT NOT NULL,
    "TotalPrice" TEXT NOT NULL,
    "IsRecurring" INTEGER NOT NULL,
    "Notes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_OrderLines_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_OrderLines_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES "Services" ("Id") ON DELETE SET NULL
);

CREATE TABLE "OrderTaxSnapshots" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OrderTaxSnapshots" PRIMARY KEY AUTOINCREMENT,
    "OrderId" INTEGER NOT NULL,
    "TaxJurisdictionId" INTEGER NULL,
    "BuyerCountryCode" TEXT NOT NULL,
    "BuyerStateCode" TEXT NULL,
    "BuyerType" INTEGER NOT NULL,
    "BuyerTaxId" TEXT NOT NULL,
    "BuyerTaxIdValidated" INTEGER NOT NULL,
    "TaxCurrencyCode" TEXT NOT NULL,
    "DisplayCurrencyCode" TEXT NOT NULL,
    "ExchangeRate" TEXT NULL,
    "ExchangeRateDate" TEXT NULL,
    "ExchangeRateSource" INTEGER NULL,
    "TaxDeterminationEvidenceId" INTEGER NULL,
    "NetAmount" TEXT NOT NULL,
    "TaxAmount" TEXT NOT NULL,
    "GrossAmount" TEXT NOT NULL,
    "AppliedTaxRate" TEXT NOT NULL,
    "AppliedTaxName" TEXT NOT NULL,
    "ReverseChargeApplied" INTEGER NOT NULL,
    "RuleVersion" TEXT NOT NULL,
    "IdempotencyKey" TEXT NULL,
    "CalculationInputsJson" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_OrderTaxSnapshots_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_OrderTaxSnapshots_TaxDeterminationEvidences_TaxDeterminationEvidenceId" FOREIGN KEY ("TaxDeterminationEvidenceId") REFERENCES "TaxDeterminationEvidences" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_OrderTaxSnapshots_TaxJurisdictions_TaxJurisdictionId" FOREIGN KEY ("TaxJurisdictionId") REFERENCES "TaxJurisdictions" ("Id") ON DELETE SET NULL
);

CREATE TABLE "CommunicationAttachments" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CommunicationAttachments" PRIMARY KEY AUTOINCREMENT,
    "CommunicationMessageId" INTEGER NOT NULL,
    "FileName" TEXT NOT NULL,
    "ContentType" TEXT NULL,
    "StoragePath" TEXT NULL,
    "SizeBytes" INTEGER NULL,
    "InlineContentId" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CommunicationAttachments_CommunicationMessages_CommunicationMessageId" FOREIGN KEY ("CommunicationMessageId") REFERENCES "CommunicationMessages" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CommunicationStatusEvents" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CommunicationStatusEvents" PRIMARY KEY AUTOINCREMENT,
    "CommunicationMessageId" INTEGER NOT NULL,
    "Status" TEXT NOT NULL,
    "Details" TEXT NULL,
    "Source" TEXT NULL,
    "OccurredAtUtc" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CommunicationStatusEvents_CommunicationMessages_CommunicationMessageId" FOREIGN KEY ("CommunicationMessageId") REFERENCES "CommunicationMessages" ("Id") ON DELETE CASCADE
);

CREATE TABLE "HostingDatabases" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HostingDatabases" PRIMARY KEY AUTOINCREMENT,
    "HostingAccountId" INTEGER NOT NULL,
    "DatabaseName" TEXT NOT NULL,
    "DatabaseType" TEXT NOT NULL,
    "SizeMB" INTEGER NULL,
    "ServerHost" TEXT NULL,
    "ServerPort" INTEGER NULL,
    "CharacterSet" TEXT NULL,
    "Collation" TEXT NULL,
    "ExternalDatabaseId" TEXT NULL,
    "LastSyncedAt" TEXT NULL,
    "SyncStatus" TEXT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_HostingDatabases_HostingAccounts_HostingAccountId" FOREIGN KEY ("HostingAccountId") REFERENCES "HostingAccounts" ("Id") ON DELETE CASCADE
);

CREATE TABLE "HostingDomains" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HostingDomains" PRIMARY KEY AUTOINCREMENT,
    "HostingAccountId" INTEGER NOT NULL,
    "DomainName" TEXT NOT NULL,
    "DomainType" TEXT NOT NULL,
    "DocumentRoot" TEXT NULL,
    "SslEnabled" INTEGER NOT NULL,
    "SslExpirationDate" TEXT NULL,
    "SslIssuer" TEXT NULL,
    "AutoRenewSsl" INTEGER NOT NULL,
    "ExternalDomainId" TEXT NULL,
    "LastSyncedAt" TEXT NULL,
    "SyncStatus" TEXT NULL,
    "PhpEnabled" INTEGER NOT NULL,
    "PhpVersion" TEXT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_HostingDomains_HostingAccounts_HostingAccountId" FOREIGN KEY ("HostingAccountId") REFERENCES "HostingAccounts" ("Id") ON DELETE CASCADE
);

CREATE TABLE "HostingEmailAccounts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HostingEmailAccounts" PRIMARY KEY AUTOINCREMENT,
    "HostingAccountId" INTEGER NOT NULL,
    "EmailAddress" TEXT NOT NULL,
    "Username" TEXT NOT NULL,
    "PasswordHash" TEXT NULL,
    "QuotaMB" INTEGER NULL,
    "UsageMB" INTEGER NULL,
    "IsForwarderOnly" INTEGER NOT NULL,
    "ForwardTo" TEXT NULL,
    "AutoResponderEnabled" INTEGER NOT NULL,
    "AutoResponderMessage" TEXT NULL,
    "SpamFilterEnabled" INTEGER NOT NULL,
    "SpamScoreThreshold" INTEGER NULL,
    "ExternalEmailId" TEXT NULL,
    "LastSyncedAt" TEXT NULL,
    "SyncStatus" TEXT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_HostingEmailAccounts_HostingAccounts_HostingAccountId" FOREIGN KEY ("HostingAccountId") REFERENCES "HostingAccounts" ("Id") ON DELETE CASCADE
);

CREATE TABLE "HostingFtpAccounts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HostingFtpAccounts" PRIMARY KEY AUTOINCREMENT,
    "HostingAccountId" INTEGER NOT NULL,
    "Username" TEXT NOT NULL,
    "PasswordHash" TEXT NULL,
    "HomeDirectory" TEXT NOT NULL,
    "QuotaMB" INTEGER NULL,
    "ReadOnly" INTEGER NOT NULL,
    "SftpEnabled" INTEGER NOT NULL,
    "FtpsEnabled" INTEGER NOT NULL,
    "ExternalFtpId" TEXT NULL,
    "LastSyncedAt" TEXT NULL,
    "SyncStatus" TEXT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_HostingFtpAccounts_HostingAccounts_HostingAccountId" FOREIGN KEY ("HostingAccountId") REFERENCES "HostingAccounts" ("Id") ON DELETE CASCADE
);

CREATE TABLE "SoldHostingPackages" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_SoldHostingPackages" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "HostingPackageId" INTEGER NOT NULL,
    "RegisteredDomainId" INTEGER NULL,
    "OrderId" INTEGER NOT NULL,
    "OrderLineId" INTEGER NULL,
    "Status" TEXT NOT NULL,
    "BillingCycle" TEXT NOT NULL,
    "SetupFee" TEXT NOT NULL,
    "RecurringPrice" TEXT NOT NULL,
    "CurrencyCode" TEXT NOT NULL,
    "ActivatedAt" TEXT NOT NULL,
    "NextBillingDate" TEXT NOT NULL,
    "ExpiresAt" TEXT NULL,
    "AutoRenew" INTEGER NOT NULL,
    "ConfigurationSnapshotJson" TEXT NOT NULL,
    "Notes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_SoldHostingPackages_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_SoldHostingPackages_HostingPackages_HostingPackageId" FOREIGN KEY ("HostingPackageId") REFERENCES "HostingPackages" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_SoldHostingPackages_OrderLines_OrderLineId" FOREIGN KEY ("OrderLineId") REFERENCES "OrderLines" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_SoldHostingPackages_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_SoldHostingPackages_RegisteredDomains_RegisteredDomainId" FOREIGN KEY ("RegisteredDomainId") REFERENCES "RegisteredDomains" ("Id") ON DELETE SET NULL
);

CREATE TABLE "SoldOptionalServices" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_SoldOptionalServices" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "ServiceId" INTEGER NOT NULL,
    "RegisteredDomainId" INTEGER NULL,
    "OrderId" INTEGER NOT NULL,
    "OrderLineId" INTEGER NULL,
    "Quantity" INTEGER NOT NULL,
    "UnitPrice" TEXT NOT NULL,
    "TotalPrice" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "BillingCycle" TEXT NOT NULL,
    "CurrencyCode" TEXT NOT NULL,
    "ActivatedAt" TEXT NOT NULL,
    "NextBillingDate" TEXT NOT NULL,
    "ExpiresAt" TEXT NULL,
    "AutoRenew" INTEGER NOT NULL,
    "ConfigurationSnapshotJson" TEXT NOT NULL,
    "Notes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_SoldOptionalServices_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_SoldOptionalServices_OrderLines_OrderLineId" FOREIGN KEY ("OrderLineId") REFERENCES "OrderLines" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_SoldOptionalServices_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_SoldOptionalServices_RegisteredDomains_RegisteredDomainId" FOREIGN KEY ("RegisteredDomainId") REFERENCES "RegisteredDomains" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_SoldOptionalServices_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES "Services" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "HostingDatabaseUsers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HostingDatabaseUsers" PRIMARY KEY AUTOINCREMENT,
    "HostingDatabaseId" INTEGER NOT NULL,
    "Username" TEXT NOT NULL,
    "PasswordHash" TEXT NULL,
    "Privileges" TEXT NULL,
    "AllowedHosts" TEXT NULL,
    "ExternalUserId" TEXT NULL,
    "LastSyncedAt" TEXT NULL,
    "SyncStatus" TEXT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_HostingDatabaseUsers_HostingDatabases_HostingDatabaseId" FOREIGN KEY ("HostingDatabaseId") REFERENCES "HostingDatabases" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CreditTransactions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CreditTransactions" PRIMARY KEY AUTOINCREMENT,
    "CustomerCreditId" INTEGER NOT NULL,
    "Type" INTEGER NOT NULL,
    "Amount" TEXT NOT NULL,
    "InvoiceId" INTEGER NULL,
    "PaymentTransactionId" INTEGER NULL,
    "Description" TEXT NOT NULL,
    "BalanceAfter" TEXT NOT NULL,
    "CreatedByUserId" INTEGER NULL,
    "InternalNotes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CreditTransactions_CustomerCredits_CustomerCreditId" FOREIGN KEY ("CustomerCreditId") REFERENCES "CustomerCredits" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CreditTransactions_Users_CreatedByUserId" FOREIGN KEY ("CreatedByUserId") REFERENCES "Users" ("Id"),
    CONSTRAINT "FK_CreditTransactions_Invoices_InvoiceId" FOREIGN KEY ("InvoiceId") REFERENCES "Invoices" ("Id"),
    CONSTRAINT "FK_CreditTransactions_PaymentTransactions_PaymentTransactionId" FOREIGN KEY ("PaymentTransactionId") REFERENCES "PaymentTransactions" ("Id")
);

CREATE TABLE "CustomerPaymentMethods" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CustomerPaymentMethods" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "PaymentGatewayId" INTEGER NOT NULL,
    "Type" INTEGER NOT NULL,
    "PaymentMethodToken" TEXT NOT NULL,
    "Last4Digits" TEXT NOT NULL,
    "ExpiryMonth" INTEGER NULL,
    "ExpiryYear" INTEGER NULL,
    "CardBrand" TEXT NOT NULL,
    "CardholderName" TEXT NOT NULL,
    "BillingAddressJson" TEXT NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsVerified" INTEGER NOT NULL,
    "VerifiedAt" TEXT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_CustomerPaymentMethods_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CustomerPaymentMethods_PaymentGateways_PaymentGatewayId" FOREIGN KEY ("PaymentGatewayId") REFERENCES "PaymentGateways" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PaymentMethodTokens" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PaymentMethodTokens" PRIMARY KEY AUTOINCREMENT,
    "CustomerPaymentMethodId" INTEGER NOT NULL,
    "EncryptedToken" TEXT NOT NULL,
    "GatewayCustomerId" TEXT NOT NULL,
    "GatewayPaymentMethodId" TEXT NOT NULL,
    "ExpiresAt" TEXT NULL,
    "Last4Digits" TEXT NOT NULL,
    "CardBrand" TEXT NOT NULL,
    "ExpiryMonth" INTEGER NULL,
    "ExpiryYear" INTEGER NULL,
    "IsDefault" INTEGER NOT NULL,
    "IsVerified" INTEGER NOT NULL,
    "VerifiedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_PaymentMethodTokens_CustomerPaymentMethods_CustomerPaymentMethodId" FOREIGN KEY ("CustomerPaymentMethodId") REFERENCES "CustomerPaymentMethods" ("Id") ON DELETE CASCADE
);

CREATE TABLE "InvoiceLines" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_InvoiceLines" PRIMARY KEY AUTOINCREMENT,
    "InvoiceId" INTEGER NOT NULL,
    "ServiceId" INTEGER NULL,
    "UnitId" INTEGER NULL,
    "LineNumber" INTEGER NOT NULL,
    "Description" TEXT NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "LineType" INTEGER NOT NULL,
    "IsGatewayFee" INTEGER NOT NULL,
    "UnitPrice" TEXT NOT NULL,
    "Discount" TEXT NOT NULL,
    "TotalPrice" TEXT NOT NULL,
    "TaxRate" TEXT NOT NULL,
    "TaxAmount" TEXT NOT NULL,
    "TotalWithTax" TEXT NOT NULL,
    "ServiceNameSnapshot" TEXT NOT NULL,
    "AccountingCode" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "Notes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_InvoiceLines_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES "Services" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_InvoiceLines_Units_UnitId" FOREIGN KEY ("UnitId") REFERENCES "Units" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_InvoiceLines_Invoices_InvoiceId" FOREIGN KEY ("InvoiceId") REFERENCES "Invoices" ("Id") ON DELETE CASCADE
);

CREATE TABLE "VendorCosts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_VendorCosts" PRIMARY KEY AUTOINCREMENT,
    "InvoiceLineId" INTEGER NOT NULL,
    "VendorPayoutId" INTEGER NULL,
    "VendorType" INTEGER NOT NULL,
    "VendorId" INTEGER NULL,
    "VendorName" TEXT NOT NULL,
    "VendorCurrency" TEXT NOT NULL,
    "VendorAmount" TEXT NOT NULL,
    "BaseCurrency" TEXT NOT NULL,
    "BaseAmount" TEXT NOT NULL,
    "ExchangeRate" TEXT NOT NULL,
    "ExchangeRateDate" TEXT NOT NULL,
    "IsRefundable" INTEGER NOT NULL,
    "RefundPolicy" INTEGER NOT NULL,
    "RefundDeadline" TEXT NULL,
    "Status" INTEGER NOT NULL,
    "Notes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_VendorCosts_InvoiceLines_InvoiceLineId" FOREIGN KEY ("InvoiceLineId") REFERENCES "InvoiceLines" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_VendorCosts_VendorPayouts_VendorPayoutId" FOREIGN KEY ("VendorPayoutId") REFERENCES "VendorPayouts" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "InvoicePayments" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_InvoicePayments" PRIMARY KEY AUTOINCREMENT,
    "InvoiceId" INTEGER NOT NULL,
    "PaymentTransactionId" INTEGER NOT NULL,
    "AmountApplied" TEXT NOT NULL,
    "Currency" TEXT NOT NULL,
    "InvoiceBalance" TEXT NOT NULL,
    "InvoiceTotalAmount" TEXT NOT NULL,
    "IsFullPayment" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_InvoicePayments_Invoices_InvoiceId" FOREIGN KEY ("InvoiceId") REFERENCES "Invoices" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_InvoicePayments_PaymentTransactions_PaymentTransactionId" FOREIGN KEY ("PaymentTransactionId") REFERENCES "PaymentTransactions" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Invoices" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Invoices" PRIMARY KEY AUTOINCREMENT,
    "InvoiceNumber" TEXT NOT NULL,
    "CustomerId" INTEGER NOT NULL,
    "OrderId" INTEGER NULL,
    "OrderTaxSnapshotId" INTEGER NULL,
    "Status" INTEGER NOT NULL,
    "IssueDate" TEXT NOT NULL,
    "DueDate" TEXT NOT NULL,
    "PaidAt" TEXT NULL,
    "SubTotal" TEXT NOT NULL,
    "TaxAmount" TEXT NOT NULL,
    "TotalAmount" TEXT NOT NULL,
    "AmountPaid" TEXT NOT NULL,
    "AmountDue" TEXT NOT NULL,
    "CurrencyCode" TEXT NOT NULL,
    "TaxRate" TEXT NOT NULL,
    "TaxName" TEXT NOT NULL,
    "BaseCurrencyCode" TEXT NOT NULL,
    "DisplayCurrencyCode" TEXT NOT NULL,
    "ExchangeRate" TEXT NULL,
    "BaseTotalAmount" TEXT NULL,
    "ExchangeRateDate" TEXT NULL,
    "CustomerName" TEXT NOT NULL,
    "CustomerAddress" TEXT NOT NULL,
    "CustomerTaxId" TEXT NOT NULL,
    "PaymentReference" TEXT NOT NULL,
    "PaymentMethod" TEXT NOT NULL,
    "Notes" TEXT NOT NULL,
    "InternalComment" TEXT NOT NULL,
    "SelectedPaymentGatewayId" INTEGER NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Invoices_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Invoices_OrderTaxSnapshots_OrderTaxSnapshotId" FOREIGN KEY ("OrderTaxSnapshotId") REFERENCES "OrderTaxSnapshots" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Invoices_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Invoices_PaymentGateways_SelectedPaymentGatewayId" FOREIGN KEY ("SelectedPaymentGatewayId") REFERENCES "PaymentGateways" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "PaymentAttempts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PaymentAttempts" PRIMARY KEY AUTOINCREMENT,
    "InvoiceId" INTEGER NOT NULL,
    "PaymentTransactionId" INTEGER NULL,
    "CustomerPaymentMethodId" INTEGER NOT NULL,
    "AttemptedAmount" TEXT NOT NULL,
    "Currency" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "GatewayResponse" TEXT NOT NULL,
    "GatewayTransactionId" TEXT NOT NULL,
    "ErrorCode" TEXT NOT NULL,
    "ErrorMessage" TEXT NOT NULL,
    "RetryCount" INTEGER NOT NULL,
    "NextRetryAt" TEXT NULL,
    "RequiresAuthentication" INTEGER NOT NULL,
    "AuthenticationUrl" TEXT NOT NULL,
    "AuthenticationStatus" INTEGER NOT NULL,
    "IpAddress" TEXT NOT NULL,
    "UserAgent" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_PaymentAttempts_CustomerPaymentMethods_CustomerPaymentMethodId" FOREIGN KEY ("CustomerPaymentMethodId") REFERENCES "CustomerPaymentMethods" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PaymentAttempts_Invoices_InvoiceId" FOREIGN KEY ("InvoiceId") REFERENCES "Invoices" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PaymentAttempts_PaymentTransactions_PaymentTransactionId" FOREIGN KEY ("PaymentTransactionId") REFERENCES "PaymentTransactions" ("Id")
);

CREATE TABLE "PaymentGateways" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PaymentGateways" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "NormalizedName" TEXT NOT NULL,
    "ProviderCode" TEXT NOT NULL,
    "PaymentInstrument" TEXT NOT NULL,
    "PaymentInstrumentId" INTEGER NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "ApiKey" TEXT NOT NULL,
    "ApiSecret" TEXT NOT NULL,
    "ConfigurationJson" TEXT NOT NULL,
    "UseSandbox" INTEGER NOT NULL,
    "WebhookUrl" TEXT NOT NULL,
    "WebhookSecret" TEXT NOT NULL,
    "DisplayOrder" INTEGER NOT NULL,
    "Description" TEXT NOT NULL,
    "LogoUrl" TEXT NOT NULL,
    "SupportedCurrencies" TEXT NOT NULL,
    "FeePercentage" TEXT NOT NULL,
    "FixedFee" TEXT NOT NULL,
    "Notes" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_PaymentGateways_PaymentInstruments_PaymentInstrumentId" FOREIGN KEY ("PaymentInstrumentId") REFERENCES "PaymentInstruments" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "PaymentInstruments" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PaymentInstruments" PRIMARY KEY AUTOINCREMENT,
    "Code" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "NormalizedCode" TEXT NOT NULL,
    "NormalizedName" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "DisplayOrder" INTEGER NOT NULL,
    "DefaultGatewayId" INTEGER NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_PaymentInstruments_PaymentGateways_DefaultGatewayId" FOREIGN KEY ("DefaultGatewayId") REFERENCES "PaymentGateways" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "PaymentIntents" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PaymentIntents" PRIMARY KEY AUTOINCREMENT,
    "InvoiceId" INTEGER NULL,
    "OrderId" INTEGER NULL,
    "CustomerId" INTEGER NOT NULL,
    "Amount" TEXT NOT NULL,
    "Currency" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "PaymentGatewayId" INTEGER NOT NULL,
    "GatewayIntentId" TEXT NOT NULL,
    "ClientSecret" TEXT NOT NULL,
    "ReturnUrl" TEXT NOT NULL,
    "CancelUrl" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "MetadataJson" TEXT NOT NULL,
    "AuthorizedAt" TEXT NULL,
    "CapturedAt" TEXT NULL,
    "FailedAt" TEXT NULL,
    "FailureReason" TEXT NOT NULL,
    "GatewayResponse" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_PaymentIntents_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PaymentIntents_Invoices_InvoiceId" FOREIGN KEY ("InvoiceId") REFERENCES "Invoices" ("Id"),
    CONSTRAINT "FK_PaymentIntents_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id"),
    CONSTRAINT "FK_PaymentIntents_PaymentGateways_PaymentGatewayId" FOREIGN KEY ("PaymentGatewayId") REFERENCES "PaymentGateways" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Subscriptions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Subscriptions" PRIMARY KEY AUTOINCREMENT,
    "CustomerId" INTEGER NOT NULL,
    "ServiceId" INTEGER NULL,
    "BillingCycleId" INTEGER NOT NULL,
    "CustomerPaymentMethodId" INTEGER NULL,
    "PaymentGatewayId" INTEGER NULL,
    "Status" INTEGER NOT NULL,
    "StartDate" TEXT NOT NULL,
    "EndDate" TEXT NULL,
    "NextBillingDate" TEXT NOT NULL,
    "CurrentPeriodStart" TEXT NOT NULL,
    "CurrentPeriodEnd" TEXT NOT NULL,
    "Amount" TEXT NOT NULL,
    "CurrencyCode" TEXT NOT NULL,
    "BillingPeriodCount" INTEGER NOT NULL,
    "BillingPeriodUnit" INTEGER NOT NULL,
    "TrialEndDate" TEXT NULL,
    "IsInTrial" INTEGER NOT NULL,
    "RetryCount" INTEGER NOT NULL,
    "MaxRetryAttempts" INTEGER NOT NULL,
    "LastBillingAttempt" TEXT NULL,
    "LastSuccessfulBilling" TEXT NULL,
    "CancelledAt" TEXT NULL,
    "CancellationReason" TEXT NOT NULL,
    "PausedAt" TEXT NULL,
    "PauseReason" TEXT NOT NULL,
    "Metadata" TEXT NOT NULL,
    "Notes" TEXT NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "SendEmailNotifications" INTEGER NOT NULL,
    "AutoRetryFailedPayments" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Subscriptions_BillingCycles_BillingCycleId" FOREIGN KEY ("BillingCycleId") REFERENCES "BillingCycles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Subscriptions_CustomerPaymentMethods_CustomerPaymentMethodId" FOREIGN KEY ("CustomerPaymentMethodId") REFERENCES "CustomerPaymentMethods" ("Id"),
    CONSTRAINT "FK_Subscriptions_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Subscriptions_PaymentGateways_PaymentGatewayId" FOREIGN KEY ("PaymentGatewayId") REFERENCES "PaymentGateways" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Subscriptions_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES "Services" ("Id")
);

CREATE TABLE "PaymentInstrumentGateways" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PaymentInstrumentGateways" PRIMARY KEY AUTOINCREMENT,
    "PaymentInstrumentId" INTEGER NOT NULL,
    "PaymentGatewayId" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDefault" INTEGER NOT NULL,
    "Priority" INTEGER NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_PaymentInstrumentGateways_PaymentGateways_PaymentGatewayId" FOREIGN KEY ("PaymentGatewayId") REFERENCES "PaymentGateways" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PaymentInstrumentGateways_PaymentInstruments_PaymentInstrumentId" FOREIGN KEY ("PaymentInstrumentId") REFERENCES "PaymentInstruments" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PaymentTransactions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PaymentTransactions" PRIMARY KEY AUTOINCREMENT,
    "InvoiceId" INTEGER NOT NULL,
    "PaymentIntentId" INTEGER NULL,
    "PaymentMethod" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "TransactionId" TEXT NOT NULL,
    "Amount" TEXT NOT NULL,
    "CurrencyCode" TEXT NOT NULL,
    "BaseCurrencyCode" TEXT NULL,
    "ExchangeRate" TEXT NULL,
    "BaseAmount" TEXT NULL,
    "GatewayFeeAmount" TEXT NULL,
    "GatewayFeeCurrency" TEXT NULL,
    "ActualExchangeRate" TEXT NULL,
    "PaymentGatewayId" INTEGER NULL,
    "GatewayResponse" TEXT NOT NULL,
    "FailureReason" TEXT NOT NULL,
    "ProcessedAt" TEXT NULL,
    "RefundedAmount" TEXT NOT NULL,
    "IsAutomatic" INTEGER NOT NULL,
    "InternalNotes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_PaymentTransactions_Invoices_InvoiceId" FOREIGN KEY ("InvoiceId") REFERENCES "Invoices" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_PaymentTransactions_PaymentGateways_PaymentGatewayId" FOREIGN KEY ("PaymentGatewayId") REFERENCES "PaymentGateways" ("Id"),
    CONSTRAINT "FK_PaymentTransactions_PaymentIntents_PaymentIntentId" FOREIGN KEY ("PaymentIntentId") REFERENCES "PaymentIntents" ("Id")
);

CREATE TABLE "Refunds" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Refunds" PRIMARY KEY AUTOINCREMENT,
    "PaymentTransactionId" INTEGER NOT NULL,
    "InvoiceId" INTEGER NOT NULL,
    "Amount" TEXT NOT NULL,
    "Reason" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "RefundTransactionId" TEXT NOT NULL,
    "ProcessedAt" TEXT NULL,
    "FailedAt" TEXT NULL,
    "FailureReason" TEXT NOT NULL,
    "GatewayResponse" TEXT NOT NULL,
    "InitiatedByUserId" INTEGER NULL,
    "InternalNotes" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Refunds_Invoices_InvoiceId" FOREIGN KEY ("InvoiceId") REFERENCES "Invoices" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Refunds_PaymentTransactions_PaymentTransactionId" FOREIGN KEY ("PaymentTransactionId") REFERENCES "PaymentTransactions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Refunds_Users_InitiatedByUserId" FOREIGN KEY ("InitiatedByUserId") REFERENCES "Users" ("Id")
);

CREATE TABLE "SubscriptionBillingHistories" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_SubscriptionBillingHistories" PRIMARY KEY AUTOINCREMENT,
    "SubscriptionId" INTEGER NOT NULL,
    "InvoiceId" INTEGER NULL,
    "PaymentTransactionId" INTEGER NULL,
    "BillingDate" TEXT NOT NULL,
    "AmountCharged" TEXT NOT NULL,
    "CurrencyCode" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "AttemptCount" INTEGER NOT NULL,
    "ErrorMessage" TEXT NOT NULL,
    "PeriodStart" TEXT NOT NULL,
    "PeriodEnd" TEXT NOT NULL,
    "IsAutomatic" INTEGER NOT NULL,
    "ProcessedByUserId" INTEGER NULL,
    "Notes" TEXT NOT NULL,
    "Metadata" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_SubscriptionBillingHistories_Invoices_InvoiceId" FOREIGN KEY ("InvoiceId") REFERENCES "Invoices" ("Id"),
    CONSTRAINT "FK_SubscriptionBillingHistories_PaymentTransactions_PaymentTransactionId" FOREIGN KEY ("PaymentTransactionId") REFERENCES "PaymentTransactions" ("Id"),
    CONSTRAINT "FK_SubscriptionBillingHistories_Subscriptions_SubscriptionId" FOREIGN KEY ("SubscriptionId") REFERENCES "Subscriptions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_SubscriptionBillingHistories_Users_ProcessedByUserId" FOREIGN KEY ("ProcessedByUserId") REFERENCES "Users" ("Id")
);

CREATE TABLE "RefundLossAudits" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RefundLossAudits" PRIMARY KEY AUTOINCREMENT,
    "RefundId" INTEGER NOT NULL,
    "InvoiceId" INTEGER NOT NULL,
    "OriginalInvoiceAmount" TEXT NOT NULL,
    "RefundedAmount" TEXT NOT NULL,
    "VendorCostUnrecoverable" TEXT NOT NULL,
    "NetLoss" TEXT NOT NULL,
    "Currency" TEXT NOT NULL,
    "Reason" TEXT NOT NULL,
    "ApprovalStatus" INTEGER NOT NULL,
    "ApprovedByUserId" INTEGER NULL,
    "ApprovedAt" TEXT NULL,
    "DenialReason" TEXT NULL,
    "InternalNotes" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_RefundLossAudits_Invoices_InvoiceId" FOREIGN KEY ("InvoiceId") REFERENCES "Invoices" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_RefundLossAudits_Refunds_RefundId" FOREIGN KEY ("RefundId") REFERENCES "Refunds" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_RefundLossAudits_Users_ApprovedByUserId" FOREIGN KEY ("ApprovedByUserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX "IX_AddressTypes_Code" ON "AddressTypes" ("Code");

CREATE INDEX "IX_AddressTypes_IsActive" ON "AddressTypes" ("IsActive");

CREATE INDEX "IX_AddressTypes_IsDefault" ON "AddressTypes" ("IsDefault");

CREATE UNIQUE INDEX "IX_AddressTypes_NormalizedCode" ON "AddressTypes" ("NormalizedCode");

CREATE INDEX "IX_AddressTypes_SortOrder" ON "AddressTypes" ("SortOrder");

CREATE INDEX "IX_AuditLogs_UserId" ON "AuditLogs" ("UserId");

CREATE INDEX "IX_CommunicationAttachments_CommunicationMessageId" ON "CommunicationAttachments" ("CommunicationMessageId");

CREATE INDEX "IX_CommunicationMessages_CommunicationThreadId" ON "CommunicationMessages" ("CommunicationThreadId");

CREATE INDEX "IX_CommunicationMessages_ExternalMessageId" ON "CommunicationMessages" ("ExternalMessageId");

CREATE INDEX "IX_CommunicationMessages_InternetMessageId" ON "CommunicationMessages" ("InternetMessageId");

CREATE INDEX "IX_CommunicationMessages_IsRead" ON "CommunicationMessages" ("IsRead");

CREATE INDEX "IX_CommunicationMessages_ReceivedAtUtc" ON "CommunicationMessages" ("ReceivedAtUtc");

CREATE INDEX "IX_CommunicationMessages_SentAtUtc" ON "CommunicationMessages" ("SentAtUtc");

CREATE INDEX "IX_CommunicationMessages_SentEmailId" ON "CommunicationMessages" ("SentEmailId");

CREATE INDEX "IX_CommunicationParticipants_CommunicationThreadId" ON "CommunicationParticipants" ("CommunicationThreadId");

CREATE INDEX "IX_CommunicationParticipants_CommunicationThreadId_EmailAddress_Role" ON "CommunicationParticipants" ("CommunicationThreadId", "EmailAddress", "Role");

CREATE INDEX "IX_CommunicationParticipants_EmailAddress" ON "CommunicationParticipants" ("EmailAddress");

CREATE INDEX "IX_CommunicationParticipants_Role" ON "CommunicationParticipants" ("Role");

CREATE INDEX "IX_CommunicationStatusEvents_CommunicationMessageId" ON "CommunicationStatusEvents" ("CommunicationMessageId");

CREATE INDEX "IX_CommunicationStatusEvents_OccurredAtUtc" ON "CommunicationStatusEvents" ("OccurredAtUtc");

CREATE INDEX "IX_CommunicationStatusEvents_Status" ON "CommunicationStatusEvents" ("Status");

CREATE INDEX "IX_CommunicationThreads_CustomerId" ON "CommunicationThreads" ("CustomerId");

CREATE INDEX "IX_CommunicationThreads_LastMessageAtUtc" ON "CommunicationThreads" ("LastMessageAtUtc");

CREATE INDEX "IX_CommunicationThreads_RelatedEntityType_RelatedEntityId" ON "CommunicationThreads" ("RelatedEntityType", "RelatedEntityId");

CREATE INDEX "IX_CommunicationThreads_Status" ON "CommunicationThreads" ("Status");

CREATE INDEX "IX_CommunicationThreads_UserId" ON "CommunicationThreads" ("UserId");

CREATE INDEX "IX_ContactPersons_CustomerId" ON "ContactPersons" ("CustomerId");

CREATE INDEX "IX_ContactPersons_Email" ON "ContactPersons" ("Email");

CREATE INDEX "IX_ContactPersons_IsActive" ON "ContactPersons" ("IsActive");

CREATE INDEX "IX_ContactPersons_IsPrimary" ON "ContactPersons" ("IsPrimary");

CREATE INDEX "IX_ContactPersons_NormalizedFirstName_NormalizedLastName" ON "ContactPersons" ("NormalizedFirstName", "NormalizedLastName");

CREATE INDEX "IX_ControlPanelTypes_IsActive" ON "ControlPanelTypes" ("IsActive");

CREATE UNIQUE INDEX "IX_ControlPanelTypes_Name" ON "ControlPanelTypes" ("Name");

CREATE UNIQUE INDEX "IX_Countries_Code" ON "Countries" ("Code");

CREATE INDEX "IX_Countries_EnglishName" ON "Countries" ("EnglishName");

CREATE INDEX "IX_Countries_NormalizedEnglishName" ON "Countries" ("NormalizedEnglishName");

CREATE INDEX "IX_Countries_Tld" ON "Countries" ("Tld");

CREATE UNIQUE INDEX "IX_Coupons_Code" ON "Coupons" ("Code");

CREATE INDEX "IX_Coupons_IsActive" ON "Coupons" ("IsActive");

CREATE INDEX "IX_Coupons_NormalizedName" ON "Coupons" ("NormalizedName");

CREATE INDEX "IX_Coupons_ValidFrom" ON "Coupons" ("ValidFrom");

CREATE INDEX "IX_Coupons_ValidUntil" ON "Coupons" ("ValidUntil");

CREATE INDEX "IX_CouponUsages_CouponId" ON "CouponUsages" ("CouponId");

CREATE INDEX "IX_CouponUsages_CustomerId" ON "CouponUsages" ("CustomerId");

CREATE INDEX "IX_CouponUsages_OrderId" ON "CouponUsages" ("OrderId");

CREATE INDEX "IX_CouponUsages_QuoteId" ON "CouponUsages" ("QuoteId");

CREATE INDEX "IX_CreditTransactions_CreatedByUserId" ON "CreditTransactions" ("CreatedByUserId");

CREATE INDEX "IX_CreditTransactions_CustomerCreditId" ON "CreditTransactions" ("CustomerCreditId");

CREATE INDEX "IX_CreditTransactions_InvoiceId" ON "CreditTransactions" ("InvoiceId");

CREATE INDEX "IX_CreditTransactions_PaymentTransactionId" ON "CreditTransactions" ("PaymentTransactionId");

CREATE UNIQUE INDEX "IX_Currencies_Code" ON "Currencies" ("Code");

CREATE INDEX "IX_Currencies_IsActive" ON "Currencies" ("IsActive");

CREATE INDEX "IX_Currencies_IsCustomerCurrency" ON "Currencies" ("IsCustomerCurrency");

CREATE INDEX "IX_Currencies_IsDefault" ON "Currencies" ("IsDefault");

CREATE INDEX "IX_Currencies_IsProviderCurrency" ON "Currencies" ("IsProviderCurrency");

CREATE UNIQUE INDEX "IX_Currencies_NormalizedCode" ON "Currencies" ("NormalizedCode");

CREATE INDEX "IX_Currencies_SortOrder" ON "Currencies" ("SortOrder");

CREATE INDEX "IX_CurrencyExchangeRates_BaseCurrency_TargetCurrency_EffectiveDate" ON "CurrencyExchangeRates" ("BaseCurrency", "TargetCurrency", "EffectiveDate");

CREATE INDEX "IX_CurrencyExchangeRates_IsActive" ON "CurrencyExchangeRates" ("IsActive");

CREATE INDEX "IX_CustomerAddresses_AddressTypeId" ON "CustomerAddresses" ("AddressTypeId");

CREATE INDEX "IX_CustomerAddresses_CustomerId" ON "CustomerAddresses" ("CustomerId");

CREATE INDEX "IX_CustomerAddresses_IsActive" ON "CustomerAddresses" ("IsActive");

CREATE INDEX "IX_CustomerAddresses_IsPrimary" ON "CustomerAddresses" ("IsPrimary");

CREATE INDEX "IX_CustomerAddresses_PostalCodeId" ON "CustomerAddresses" ("PostalCodeId");

CREATE INDEX "IX_CustomerChanges_ChangedAt" ON "CustomerChanges" ("ChangedAt");

CREATE INDEX "IX_CustomerChanges_ChangedByUserId" ON "CustomerChanges" ("ChangedByUserId");

CREATE INDEX "IX_CustomerChanges_ChangeType" ON "CustomerChanges" ("ChangeType");

CREATE INDEX "IX_CustomerChanges_CustomerId" ON "CustomerChanges" ("CustomerId");

CREATE INDEX "IX_CustomerCredits_CustomerId" ON "CustomerCredits" ("CustomerId");

CREATE INDEX "IX_CustomerInternalNotes_CreatedAt" ON "CustomerInternalNotes" ("CreatedAt");

CREATE INDEX "IX_CustomerInternalNotes_CreatedByUserId" ON "CustomerInternalNotes" ("CreatedByUserId");

CREATE INDEX "IX_CustomerInternalNotes_CustomerId" ON "CustomerInternalNotes" ("CustomerId");

CREATE INDEX "IX_CustomerPaymentMethods_CustomerId" ON "CustomerPaymentMethods" ("CustomerId");

CREATE INDEX "IX_CustomerPaymentMethods_PaymentGatewayId" ON "CustomerPaymentMethods" ("PaymentGatewayId");

CREATE INDEX "IX_Customers_CountryCode" ON "Customers" ("CountryCode");

CREATE INDEX "IX_Customers_CountryId" ON "Customers" ("CountryId");

CREATE UNIQUE INDEX "IX_Customers_CustomerNumber" ON "Customers" ("CustomerNumber");

CREATE INDEX "IX_Customers_CustomerStatusId" ON "Customers" ("CustomerStatusId");

CREATE INDEX "IX_Customers_Email" ON "Customers" ("Email");

CREATE INDEX "IX_Customers_IsActive" ON "Customers" ("IsActive");

CREATE INDEX "IX_Customers_IsSelfRegistered" ON "Customers" ("IsSelfRegistered");

CREATE INDEX "IX_Customers_NormalizedCustomerName" ON "Customers" ("NormalizedCustomerName");

CREATE INDEX "IX_Customers_NormalizedName" ON "Customers" ("NormalizedName");

CREATE UNIQUE INDEX "IX_Customers_ReferenceNumber" ON "Customers" ("ReferenceNumber");

CREATE INDEX "IX_Customers_Status" ON "Customers" ("Status");

CREATE INDEX "IX_Customers_TaxId" ON "Customers" ("TaxId");

CREATE INDEX "IX_Customers_VatNumber" ON "Customers" ("VatNumber");

CREATE UNIQUE INDEX "IX_CustomerStatuses_Code" ON "CustomerStatuses" ("Code");

CREATE INDEX "IX_CustomerStatuses_IsActive" ON "CustomerStatuses" ("IsActive");

CREATE INDEX "IX_CustomerStatuses_IsDefault" ON "CustomerStatuses" ("IsDefault");

CREATE INDEX "IX_CustomerStatuses_IsSystem" ON "CustomerStatuses" ("IsSystem");

CREATE UNIQUE INDEX "IX_CustomerStatuses_NormalizedCode" ON "CustomerStatuses" ("NormalizedCode");

CREATE INDEX "IX_CustomerStatuses_SortOrder" ON "CustomerStatuses" ("SortOrder");

CREATE UNIQUE INDEX "IX_CustomerTaxProfiles_CustomerId" ON "CustomerTaxProfiles" ("CustomerId");

CREATE INDEX "IX_CustomerTaxProfiles_TaxIdNumber" ON "CustomerTaxProfiles" ("TaxIdNumber");

CREATE INDEX "IX_CustomerTaxProfiles_TaxResidenceCountry" ON "CustomerTaxProfiles" ("TaxResidenceCountry");

CREATE INDEX "IX_DnsRecords_DnsRecordTypeId" ON "DnsRecords" ("DnsRecordTypeId");

CREATE INDEX "IX_DnsRecords_DomainId" ON "DnsRecords" ("DomainId");

CREATE INDEX "IX_DnsRecords_DomainId_DnsRecordTypeId" ON "DnsRecords" ("DomainId", "DnsRecordTypeId");

CREATE INDEX "IX_DnsRecordTypes_IsActive" ON "DnsRecordTypes" ("IsActive");

CREATE UNIQUE INDEX "IX_DnsRecordTypes_Type" ON "DnsRecordTypes" ("Type");

CREATE INDEX "IX_DnsZonePackageControlPanels_DnsZonePackageId" ON "DnsZonePackageControlPanels" ("DnsZonePackageId");

CREATE INDEX "IX_DnsZonePackageControlPanels_ServerControlPanelId" ON "DnsZonePackageControlPanels" ("ServerControlPanelId");

CREATE INDEX "IX_DnsZonePackageRecords_DnsRecordTypeId" ON "DnsZonePackageRecords" ("DnsRecordTypeId");

CREATE INDEX "IX_DnsZonePackageRecords_DnsZonePackageId" ON "DnsZonePackageRecords" ("DnsZonePackageId");

CREATE INDEX "IX_DnsZonePackages_IsActive" ON "DnsZonePackages" ("IsActive");

CREATE INDEX "IX_DnsZonePackages_IsDefault" ON "DnsZonePackages" ("IsDefault");

CREATE INDEX "IX_DnsZonePackages_Name" ON "DnsZonePackages" ("Name");

CREATE INDEX "IX_DnsZonePackages_ResellerCompanyId" ON "DnsZonePackages" ("ResellerCompanyId");

CREATE INDEX "IX_DnsZonePackages_SalesAgentId" ON "DnsZonePackages" ("SalesAgentId");

CREATE INDEX "IX_DnsZonePackages_SortOrder" ON "DnsZonePackages" ("SortOrder");

CREATE INDEX "IX_DnsZonePackageServers_DnsZonePackageId" ON "DnsZonePackageServers" ("DnsZonePackageId");

CREATE INDEX "IX_DnsZonePackageServers_ServerId" ON "DnsZonePackageServers" ("ServerId");

CREATE INDEX "IX_DomainContactAssignments_AssignedAt" ON "DomainContactAssignments" ("AssignedAt");

CREATE INDEX "IX_DomainContactAssignments_ContactPersonId" ON "DomainContactAssignments" ("ContactPersonId");

CREATE INDEX "IX_DomainContactAssignments_IsActive" ON "DomainContactAssignments" ("IsActive");

CREATE INDEX "IX_DomainContactAssignments_RegisteredDomainId" ON "DomainContactAssignments" ("RegisteredDomainId");

CREATE INDEX "IX_DomainContactAssignments_RegisteredDomainId_RoleType_IsActive" ON "DomainContactAssignments" ("RegisteredDomainId", "RoleType", "IsActive");

CREATE INDEX "IX_DomainContacts_DomainId" ON "DomainContacts" ("DomainId");

CREATE INDEX "IX_DomainContacts_DomainId_RoleType_IsCurrentVersion" ON "DomainContacts" ("DomainId", "RoleType", "IsCurrentVersion");

CREATE INDEX "IX_DomainContacts_Email" ON "DomainContacts" ("Email");

CREATE INDEX "IX_DomainContacts_IsCurrentVersion" ON "DomainContacts" ("IsCurrentVersion");

CREATE INDEX "IX_DomainContacts_NeedsSync" ON "DomainContacts" ("NeedsSync");

CREATE INDEX "IX_DomainContacts_NormalizedEmail" ON "DomainContacts" ("NormalizedEmail");

CREATE INDEX "IX_DomainContacts_RoleType" ON "DomainContacts" ("RoleType");

CREATE INDEX "IX_DomainContacts_SourceContactPersonId" ON "DomainContacts" ("SourceContactPersonId");

CREATE INDEX "IX_HostingAccounts_CustomerId" ON "HostingAccounts" ("CustomerId");

CREATE INDEX "IX_HostingAccounts_ServerControlPanelId" ON "HostingAccounts" ("ServerControlPanelId");

CREATE INDEX "IX_HostingAccounts_ServerId" ON "HostingAccounts" ("ServerId");

CREATE INDEX "IX_HostingAccounts_ServiceId" ON "HostingAccounts" ("ServiceId");

CREATE INDEX "IX_HostingDatabases_HostingAccountId" ON "HostingDatabases" ("HostingAccountId");

CREATE INDEX "IX_HostingDatabaseUsers_HostingDatabaseId" ON "HostingDatabaseUsers" ("HostingDatabaseId");

CREATE INDEX "IX_HostingDomains_HostingAccountId" ON "HostingDomains" ("HostingAccountId");

CREATE INDEX "IX_HostingEmailAccounts_HostingAccountId" ON "HostingEmailAccounts" ("HostingAccountId");

CREATE INDEX "IX_HostingFtpAccounts_HostingAccountId" ON "HostingFtpAccounts" ("HostingAccountId");

CREATE INDEX "IX_HostingPackages_IsActive" ON "HostingPackages" ("IsActive");

CREATE INDEX "IX_HostingPackages_Name" ON "HostingPackages" ("Name");

CREATE INDEX "IX_HostingPackages_NormalizedName" ON "HostingPackages" ("NormalizedName");

CREATE INDEX "IX_HostProviders_IsActive" ON "HostProviders" ("IsActive");

CREATE UNIQUE INDEX "IX_HostProviders_Name" ON "HostProviders" ("Name");

CREATE INDEX "IX_InvoiceLines_InvoiceId" ON "InvoiceLines" ("InvoiceId");

CREATE INDEX "IX_InvoiceLines_ServiceId" ON "InvoiceLines" ("ServiceId");

CREATE INDEX "IX_InvoiceLines_UnitId" ON "InvoiceLines" ("UnitId");

CREATE INDEX "IX_InvoicePayments_InvoiceId" ON "InvoicePayments" ("InvoiceId");

CREATE INDEX "IX_InvoicePayments_PaymentTransactionId" ON "InvoicePayments" ("PaymentTransactionId");

CREATE INDEX "IX_Invoices_CustomerId" ON "Invoices" ("CustomerId");

CREATE UNIQUE INDEX "IX_Invoices_InvoiceNumber" ON "Invoices" ("InvoiceNumber");

CREATE INDEX "IX_Invoices_OrderId" ON "Invoices" ("OrderId");

CREATE INDEX "IX_Invoices_OrderTaxSnapshotId" ON "Invoices" ("OrderTaxSnapshotId");

CREATE INDEX "IX_Invoices_SelectedPaymentGatewayId" ON "Invoices" ("SelectedPaymentGatewayId");

CREATE INDEX "IX_LoginHistories_AttemptedAt" ON "LoginHistories" ("AttemptedAt");

CREATE INDEX "IX_LoginHistories_IsSuccessful" ON "LoginHistories" ("IsSuccessful");

CREATE INDEX "IX_LoginHistories_UserId" ON "LoginHistories" ("UserId");

CREATE INDEX "IX_NameServerDomains_DomainId" ON "NameServerDomains" ("DomainId");

CREATE INDEX "IX_NameServers_Hostname" ON "NameServers" ("Hostname");

CREATE INDEX "IX_NameServers_ServerId" ON "NameServers" ("ServerId");

CREATE INDEX "IX_NameServers_SortOrder" ON "NameServers" ("SortOrder");

CREATE INDEX "IX_OperatingSystems_IsActive" ON "OperatingSystems" ("IsActive");

CREATE UNIQUE INDEX "IX_OperatingSystems_Name" ON "OperatingSystems" ("Name");

CREATE INDEX "IX_OrderLines_OrderId_LineNumber" ON "OrderLines" ("OrderId", "LineNumber");

CREATE INDEX "IX_OrderLines_ServiceId" ON "OrderLines" ("ServiceId");

CREATE INDEX "IX_Orders_CouponId" ON "Orders" ("CouponId");

CREATE INDEX "IX_Orders_CustomerId" ON "Orders" ("CustomerId");

CREATE INDEX "IX_Orders_QuoteId" ON "Orders" ("QuoteId");

CREATE INDEX "IX_Orders_ServiceId" ON "Orders" ("ServiceId");

CREATE INDEX "IX_OrderTaxSnapshots_BuyerCountryCode" ON "OrderTaxSnapshots" ("BuyerCountryCode");

CREATE INDEX "IX_OrderTaxSnapshots_CreatedAt" ON "OrderTaxSnapshots" ("CreatedAt");

CREATE INDEX "IX_OrderTaxSnapshots_OrderId" ON "OrderTaxSnapshots" ("OrderId");

CREATE UNIQUE INDEX "IX_OrderTaxSnapshots_OrderId_IdempotencyKey" ON "OrderTaxSnapshots" ("OrderId", "IdempotencyKey");

CREATE INDEX "IX_OrderTaxSnapshots_TaxDeterminationEvidenceId" ON "OrderTaxSnapshots" ("TaxDeterminationEvidenceId");

CREATE INDEX "IX_OrderTaxSnapshots_TaxJurisdictionId" ON "OrderTaxSnapshots" ("TaxJurisdictionId");

CREATE INDEX "IX_PaymentAttempts_CustomerPaymentMethodId" ON "PaymentAttempts" ("CustomerPaymentMethodId");

CREATE INDEX "IX_PaymentAttempts_InvoiceId" ON "PaymentAttempts" ("InvoiceId");

CREATE INDEX "IX_PaymentAttempts_PaymentTransactionId" ON "PaymentAttempts" ("PaymentTransactionId");

CREATE INDEX "IX_PaymentGateways_IsActive" ON "PaymentGateways" ("IsActive");

CREATE INDEX "IX_PaymentGateways_IsDefault" ON "PaymentGateways" ("IsDefault");

CREATE INDEX "IX_PaymentGateways_NormalizedName" ON "PaymentGateways" ("NormalizedName");

CREATE INDEX "IX_PaymentGateways_PaymentInstrument" ON "PaymentGateways" ("PaymentInstrument");

CREATE INDEX "IX_PaymentGateways_PaymentInstrumentId" ON "PaymentGateways" ("PaymentInstrumentId");

CREATE INDEX "IX_PaymentGateways_ProviderCode" ON "PaymentGateways" ("ProviderCode");

CREATE INDEX "IX_PaymentInstrumentGateways_IsActive" ON "PaymentInstrumentGateways" ("IsActive");

CREATE INDEX "IX_PaymentInstrumentGateways_IsDefault" ON "PaymentInstrumentGateways" ("IsDefault");

CREATE INDEX "IX_PaymentInstrumentGateways_PaymentGatewayId" ON "PaymentInstrumentGateways" ("PaymentGatewayId");

CREATE UNIQUE INDEX "IX_PaymentInstrumentGateways_PaymentInstrumentId_PaymentGatewayId" ON "PaymentInstrumentGateways" ("PaymentInstrumentId", "PaymentGatewayId");

CREATE INDEX "IX_PaymentInstrumentGateways_Priority" ON "PaymentInstrumentGateways" ("Priority");

CREATE UNIQUE INDEX "IX_PaymentInstruments_Code" ON "PaymentInstruments" ("Code");

CREATE INDEX "IX_PaymentInstruments_DefaultGatewayId" ON "PaymentInstruments" ("DefaultGatewayId");

CREATE INDEX "IX_PaymentInstruments_DisplayOrder" ON "PaymentInstruments" ("DisplayOrder");

CREATE INDEX "IX_PaymentInstruments_IsActive" ON "PaymentInstruments" ("IsActive");

CREATE UNIQUE INDEX "IX_PaymentInstruments_NormalizedCode" ON "PaymentInstruments" ("NormalizedCode");

CREATE INDEX "IX_PaymentIntents_CustomerId" ON "PaymentIntents" ("CustomerId");

CREATE INDEX "IX_PaymentIntents_InvoiceId" ON "PaymentIntents" ("InvoiceId");

CREATE INDEX "IX_PaymentIntents_OrderId" ON "PaymentIntents" ("OrderId");

CREATE INDEX "IX_PaymentIntents_PaymentGatewayId" ON "PaymentIntents" ("PaymentGatewayId");

CREATE INDEX "IX_PaymentMethodTokens_CustomerPaymentMethodId" ON "PaymentMethodTokens" ("CustomerPaymentMethodId");

CREATE INDEX "IX_PaymentTransactions_InvoiceId" ON "PaymentTransactions" ("InvoiceId");

CREATE INDEX "IX_PaymentTransactions_PaymentGatewayId" ON "PaymentTransactions" ("PaymentGatewayId");

CREATE INDEX "IX_PaymentTransactions_PaymentIntentId" ON "PaymentTransactions" ("PaymentIntentId");

CREATE INDEX "IX_PostalCodes_City" ON "PostalCodes" ("City");

CREATE INDEX "IX_PostalCodes_Code_CountryCode" ON "PostalCodes" ("Code", "CountryCode");

CREATE INDEX "IX_PostalCodes_CountryCode" ON "PostalCodes" ("CountryCode");

CREATE INDEX "IX_PostalCodes_NormalizedCity" ON "PostalCodes" ("NormalizedCity");

CREATE INDEX "IX_PostalCodes_NormalizedCode_NormalizedCountryCode" ON "PostalCodes" ("NormalizedCode", "NormalizedCountryCode");

CREATE UNIQUE INDEX "IX_ProfitMarginSettings_ProductClass" ON "ProfitMarginSettings" ("ProductClass");

CREATE INDEX "IX_QuoteLines_BillingCycleId" ON "QuoteLines" ("BillingCycleId");

CREATE INDEX "IX_QuoteLines_QuoteId" ON "QuoteLines" ("QuoteId");

CREATE INDEX "IX_QuoteLines_ServiceId" ON "QuoteLines" ("ServiceId");

CREATE INDEX "IX_QuoteRevisions_ActionType" ON "QuoteRevisions" ("ActionType");

CREATE INDEX "IX_QuoteRevisions_QuoteId" ON "QuoteRevisions" ("QuoteId");

CREATE UNIQUE INDEX "IX_QuoteRevisions_QuoteId_RevisionNumber" ON "QuoteRevisions" ("QuoteId", "RevisionNumber");

CREATE INDEX "IX_Quotes_CouponId" ON "Quotes" ("CouponId");

CREATE INDEX "IX_Quotes_CustomerId" ON "Quotes" ("CustomerId");

CREATE INDEX "IX_Quotes_PreparedByUserId" ON "Quotes" ("PreparedByUserId");

CREATE INDEX "IX_RefundLossAudits_ApprovalStatus" ON "RefundLossAudits" ("ApprovalStatus");

CREATE INDEX "IX_RefundLossAudits_ApprovedAt" ON "RefundLossAudits" ("ApprovedAt");

CREATE INDEX "IX_RefundLossAudits_ApprovedByUserId" ON "RefundLossAudits" ("ApprovedByUserId");

CREATE INDEX "IX_RefundLossAudits_InvoiceId" ON "RefundLossAudits" ("InvoiceId");

CREATE INDEX "IX_RefundLossAudits_RefundId" ON "RefundLossAudits" ("RefundId");

CREATE INDEX "IX_Refunds_InitiatedByUserId" ON "Refunds" ("InitiatedByUserId");

CREATE INDEX "IX_Refunds_InvoiceId" ON "Refunds" ("InvoiceId");

CREATE INDEX "IX_Refunds_PaymentTransactionId" ON "Refunds" ("PaymentTransactionId");

CREATE INDEX "IX_RegisteredDomainHistories_ActionType" ON "RegisteredDomainHistories" ("ActionType");

CREATE INDEX "IX_RegisteredDomainHistories_OccurredAt" ON "RegisteredDomainHistories" ("OccurredAt");

CREATE INDEX "IX_RegisteredDomainHistories_PerformedByUserId" ON "RegisteredDomainHistories" ("PerformedByUserId");

CREATE INDEX "IX_RegisteredDomainHistories_RegisteredDomainId" ON "RegisteredDomainHistories" ("RegisteredDomainId");

CREATE INDEX "IX_RegisteredDomainHistories_RegisteredDomainId_OccurredAt" ON "RegisteredDomainHistories" ("RegisteredDomainId", "OccurredAt");

CREATE INDEX "IX_RegisteredDomains_CustomerId" ON "RegisteredDomains" ("CustomerId");

CREATE INDEX "IX_RegisteredDomains_ExpirationDate" ON "RegisteredDomains" ("ExpirationDate");

CREATE UNIQUE INDEX "IX_RegisteredDomains_Name" ON "RegisteredDomains" ("Name");

CREATE INDEX "IX_RegisteredDomains_NextRegistrationAttemptUtc" ON "RegisteredDomains" ("NextRegistrationAttemptUtc");

CREATE UNIQUE INDEX "IX_RegisteredDomains_NormalizedName" ON "RegisteredDomains" ("NormalizedName");

CREATE INDEX "IX_RegisteredDomains_RegistrarId" ON "RegisteredDomains" ("RegistrarId");

CREATE INDEX "IX_RegisteredDomains_RegistrarTldId" ON "RegisteredDomains" ("RegistrarTldId");

CREATE INDEX "IX_RegisteredDomains_RegistrationStatus" ON "RegisteredDomains" ("RegistrationStatus");

CREATE INDEX "IX_RegisteredDomains_ServiceId" ON "RegisteredDomains" ("ServiceId");

CREATE INDEX "IX_RegisteredDomains_Status" ON "RegisteredDomains" ("Status");

CREATE INDEX "IX_RegistrarMailAddresses_CustomerId" ON "RegistrarMailAddresses" ("CustomerId");

CREATE INDEX "IX_RegistrarMailAddresses_IsActive" ON "RegistrarMailAddresses" ("IsActive");

CREATE INDEX "IX_RegistrarMailAddresses_IsDefault" ON "RegistrarMailAddresses" ("IsDefault");

CREATE UNIQUE INDEX "IX_RegistrarMailAddresses_MailAddress" ON "RegistrarMailAddresses" ("MailAddress");

CREATE UNIQUE INDEX "IX_Registrars_Code" ON "Registrars" ("Code");

CREATE INDEX "IX_Registrars_IsActive" ON "Registrars" ("IsActive");

CREATE INDEX "IX_Registrars_IsDefault" ON "Registrars" ("IsDefault");

CREATE INDEX "IX_Registrars_Name" ON "Registrars" ("Name");

CREATE INDEX "IX_Registrars_NormalizedName" ON "Registrars" ("NormalizedName");

CREATE INDEX "IX_RegistrarSelectionPreferences_RegistrarId" ON "RegistrarSelectionPreferences" ("RegistrarId");

CREATE INDEX "IX_RegistrarTldCostPricing_RegistrarTldId" ON "RegistrarTldCostPricing" ("RegistrarTldId");

CREATE INDEX "IX_RegistrarTldPriceChangeLogs_ChangedAtUtc" ON "RegistrarTldPriceChangeLogs" ("ChangedAtUtc");

CREATE INDEX "IX_RegistrarTldPriceChangeLogs_ChangeSource" ON "RegistrarTldPriceChangeLogs" ("ChangeSource");

CREATE INDEX "IX_RegistrarTldPriceChangeLogs_DownloadSessionId" ON "RegistrarTldPriceChangeLogs" ("DownloadSessionId");

CREATE INDEX "IX_RegistrarTldPriceChangeLogs_RegistrarTldId" ON "RegistrarTldPriceChangeLogs" ("RegistrarTldId");

CREATE INDEX "IX_RegistrarTldPriceDownloadSessions_RegistrarId_StartedAtUtc" ON "RegistrarTldPriceDownloadSessions" ("RegistrarId", "StartedAtUtc");

CREATE INDEX "IX_RegistrarTldPriceDownloadSessions_Success" ON "RegistrarTldPriceDownloadSessions" ("Success");

CREATE INDEX "IX_RegistrarTlds_IsActive" ON "RegistrarTlds" ("IsActive");

CREATE UNIQUE INDEX "IX_RegistrarTlds_RegistrarId_TldId" ON "RegistrarTlds" ("RegistrarId", "TldId");

CREATE INDEX "IX_RegistrarTlds_TldId" ON "RegistrarTlds" ("TldId");

CREATE INDEX "IX_ResellerCompanies_Email" ON "ResellerCompanies" ("Email");

CREATE INDEX "IX_ResellerCompanies_IsActive" ON "ResellerCompanies" ("IsActive");

CREATE INDEX "IX_ResellerCompanies_Name" ON "ResellerCompanies" ("Name");

CREATE INDEX "IX_ResellerTldDiscounts_ResellerCompanyId" ON "ResellerTldDiscounts" ("ResellerCompanyId");

CREATE INDEX "IX_ResellerTldDiscounts_TldId" ON "ResellerTldDiscounts" ("TldId");

CREATE INDEX "IX_SalesAgents_Email" ON "SalesAgents" ("Email");

CREATE INDEX "IX_SalesAgents_IsActive" ON "SalesAgents" ("IsActive");

CREATE INDEX "IX_SalesAgents_NormalizedFirstName_NormalizedLastName" ON "SalesAgents" ("NormalizedFirstName", "NormalizedLastName");

CREATE INDEX "IX_SalesAgents_ResellerCompanyId" ON "SalesAgents" ("ResellerCompanyId");

CREATE INDEX "IX_SentEmails_CustomerId" ON "SentEmails" ("CustomerId");

CREATE INDEX "IX_SentEmails_From" ON "SentEmails" ("From");

CREATE INDEX "IX_SentEmails_MessageId" ON "SentEmails" ("MessageId");

CREATE INDEX "IX_SentEmails_NextAttemptAt" ON "SentEmails" ("NextAttemptAt");

CREATE INDEX "IX_SentEmails_RelatedEntityType_RelatedEntityId" ON "SentEmails" ("RelatedEntityType", "RelatedEntityId");

CREATE INDEX "IX_SentEmails_SentDate" ON "SentEmails" ("SentDate");

CREATE INDEX "IX_SentEmails_Status" ON "SentEmails" ("Status");

CREATE INDEX "IX_SentEmails_Status_NextAttemptAt" ON "SentEmails" ("Status", "NextAttemptAt");

CREATE INDEX "IX_SentEmails_UserId" ON "SentEmails" ("UserId");

CREATE INDEX "IX_ServerControlPanels_ControlPanelTypeId" ON "ServerControlPanels" ("ControlPanelTypeId");

CREATE INDEX "IX_ServerControlPanels_IpAddressId" ON "ServerControlPanels" ("IpAddressId");

CREATE INDEX "IX_ServerControlPanels_ServerId" ON "ServerControlPanels" ("ServerId");

CREATE INDEX "IX_ServerControlPanels_Status" ON "ServerControlPanels" ("Status");

CREATE INDEX "IX_ServerIpAddresses_IpAddress" ON "ServerIpAddresses" ("IpAddress");

CREATE INDEX "IX_ServerIpAddresses_ServerId" ON "ServerIpAddresses" ("ServerId");

CREATE INDEX "IX_ServerIpAddresses_Status" ON "ServerIpAddresses" ("Status");

CREATE INDEX "IX_Servers_HostProviderId" ON "Servers" ("HostProviderId");

CREATE INDEX "IX_Servers_Name" ON "Servers" ("Name");

CREATE INDEX "IX_Servers_OperatingSystemId" ON "Servers" ("OperatingSystemId");

CREATE INDEX "IX_Servers_ServerTypeId" ON "Servers" ("ServerTypeId");

CREATE INDEX "IX_Servers_Status" ON "Servers" ("Status");

CREATE INDEX "IX_ServerTypes_IsActive" ON "ServerTypes" ("IsActive");

CREATE UNIQUE INDEX "IX_ServerTypes_Name" ON "ServerTypes" ("Name");

CREATE INDEX "IX_Services_BillingCycleId" ON "Services" ("BillingCycleId");

CREATE INDEX "IX_Services_HostingPackageId" ON "Services" ("HostingPackageId");

CREATE INDEX "IX_Services_ResellerCompanyId" ON "Services" ("ResellerCompanyId");

CREATE INDEX "IX_Services_SalesAgentId" ON "Services" ("SalesAgentId");

CREATE INDEX "IX_Services_ServiceTypeId" ON "Services" ("ServiceTypeId");

CREATE INDEX "IX_SoldHostingPackages_CustomerId" ON "SoldHostingPackages" ("CustomerId");

CREATE INDEX "IX_SoldHostingPackages_HostingPackageId" ON "SoldHostingPackages" ("HostingPackageId");

CREATE INDEX "IX_SoldHostingPackages_NextBillingDate" ON "SoldHostingPackages" ("NextBillingDate");

CREATE INDEX "IX_SoldHostingPackages_OrderId" ON "SoldHostingPackages" ("OrderId");

CREATE INDEX "IX_SoldHostingPackages_OrderLineId" ON "SoldHostingPackages" ("OrderLineId");

CREATE INDEX "IX_SoldHostingPackages_RegisteredDomainId" ON "SoldHostingPackages" ("RegisteredDomainId");

CREATE INDEX "IX_SoldHostingPackages_Status" ON "SoldHostingPackages" ("Status");

CREATE INDEX "IX_SoldOptionalServices_CustomerId" ON "SoldOptionalServices" ("CustomerId");

CREATE INDEX "IX_SoldOptionalServices_NextBillingDate" ON "SoldOptionalServices" ("NextBillingDate");

CREATE INDEX "IX_SoldOptionalServices_OrderId" ON "SoldOptionalServices" ("OrderId");

CREATE INDEX "IX_SoldOptionalServices_OrderLineId" ON "SoldOptionalServices" ("OrderLineId");

CREATE INDEX "IX_SoldOptionalServices_RegisteredDomainId" ON "SoldOptionalServices" ("RegisteredDomainId");

CREATE INDEX "IX_SoldOptionalServices_ServiceId" ON "SoldOptionalServices" ("ServiceId");

CREATE INDEX "IX_SoldOptionalServices_Status" ON "SoldOptionalServices" ("Status");

CREATE INDEX "IX_SubscriptionBillingHistories_InvoiceId" ON "SubscriptionBillingHistories" ("InvoiceId");

CREATE INDEX "IX_SubscriptionBillingHistories_PaymentTransactionId" ON "SubscriptionBillingHistories" ("PaymentTransactionId");

CREATE INDEX "IX_SubscriptionBillingHistories_ProcessedByUserId" ON "SubscriptionBillingHistories" ("ProcessedByUserId");

CREATE INDEX "IX_SubscriptionBillingHistories_SubscriptionId" ON "SubscriptionBillingHistories" ("SubscriptionId");

CREATE INDEX "IX_Subscriptions_BillingCycleId" ON "Subscriptions" ("BillingCycleId");

CREATE INDEX "IX_Subscriptions_CustomerId" ON "Subscriptions" ("CustomerId");

CREATE INDEX "IX_Subscriptions_CustomerPaymentMethodId" ON "Subscriptions" ("CustomerPaymentMethodId");

CREATE INDEX "IX_Subscriptions_PaymentGatewayId" ON "Subscriptions" ("PaymentGatewayId");

CREATE INDEX "IX_Subscriptions_ServiceId" ON "Subscriptions" ("ServiceId");

CREATE INDEX "IX_SupportTicketMessages_CreatedAt" ON "SupportTicketMessages" ("CreatedAt");

CREATE INDEX "IX_SupportTicketMessages_SenderUserId" ON "SupportTicketMessages" ("SenderUserId");

CREATE INDEX "IX_SupportTicketMessages_SupportTicketId" ON "SupportTicketMessages" ("SupportTicketId");

CREATE INDEX "IX_SupportTickets_AssignedDepartment" ON "SupportTickets" ("AssignedDepartment");

CREATE INDEX "IX_SupportTickets_AssignedToUserId" ON "SupportTickets" ("AssignedToUserId");

CREATE INDEX "IX_SupportTickets_CreatedByUserId" ON "SupportTickets" ("CreatedByUserId");

CREATE INDEX "IX_SupportTickets_CustomerId" ON "SupportTickets" ("CustomerId");

CREATE INDEX "IX_SupportTickets_LastMessageAt" ON "SupportTickets" ("LastMessageAt");

CREATE INDEX "IX_SupportTickets_Priority" ON "SupportTickets" ("Priority");

CREATE INDEX "IX_SupportTickets_Source" ON "SupportTickets" ("Source");

CREATE INDEX "IX_SupportTickets_Status" ON "SupportTickets" ("Status");

CREATE UNIQUE INDEX "IX_SystemSettings_Key" ON "SystemSettings" ("Key");

CREATE INDEX "IX_TaxCategories_CountryCode" ON "TaxCategories" ("CountryCode");

CREATE UNIQUE INDEX "IX_TaxCategories_CountryCode_StateCode_Code" ON "TaxCategories" ("CountryCode", "StateCode", "Code");

CREATE INDEX "IX_TaxCategories_IsActive" ON "TaxCategories" ("IsActive");

CREATE INDEX "IX_TaxDeterminationEvidences_BuyerCountryCode" ON "TaxDeterminationEvidences" ("BuyerCountryCode");

CREATE INDEX "IX_TaxDeterminationEvidences_CapturedAt" ON "TaxDeterminationEvidences" ("CapturedAt");

CREATE INDEX "IX_TaxDeterminationEvidences_CustomerId" ON "TaxDeterminationEvidences" ("CustomerId");

CREATE INDEX "IX_TaxDeterminationEvidences_OrderId" ON "TaxDeterminationEvidences" ("OrderId");

CREATE UNIQUE INDEX "IX_TaxJurisdictions_Code" ON "TaxJurisdictions" ("Code");

CREATE INDEX "IX_TaxJurisdictions_CountryCode" ON "TaxJurisdictions" ("CountryCode");

CREATE INDEX "IX_TaxJurisdictions_CountryCode_StateCode" ON "TaxJurisdictions" ("CountryCode", "StateCode");

CREATE INDEX "IX_TaxJurisdictions_IsActive" ON "TaxJurisdictions" ("IsActive");

CREATE INDEX "IX_TaxRegistrations_IsActive" ON "TaxRegistrations" ("IsActive");

CREATE INDEX "IX_TaxRegistrations_RegistrationNumber" ON "TaxRegistrations" ("RegistrationNumber");

CREATE INDEX "IX_TaxRegistrations_TaxJurisdictionId" ON "TaxRegistrations" ("TaxJurisdictionId");

CREATE UNIQUE INDEX "IX_TaxRegistrations_TaxJurisdictionId_RegistrationNumber" ON "TaxRegistrations" ("TaxJurisdictionId", "RegistrationNumber");

CREATE INDEX "IX_TaxRules_CountryCode" ON "TaxRules" ("CountryCode");

CREATE INDEX "IX_TaxRules_CountryCode_StateCode" ON "TaxRules" ("CountryCode", "StateCode");

CREATE INDEX "IX_TaxRules_EffectiveFrom" ON "TaxRules" ("EffectiveFrom");

CREATE INDEX "IX_TaxRules_EffectiveUntil" ON "TaxRules" ("EffectiveUntil");

CREATE INDEX "IX_TaxRules_IsActive" ON "TaxRules" ("IsActive");

CREATE INDEX "IX_TaxRules_TaxCategory" ON "TaxRules" ("TaxCategory");

CREATE INDEX "IX_TaxRules_TaxCategoryId" ON "TaxRules" ("TaxCategoryId");

CREATE INDEX "IX_TaxRules_TaxJurisdictionId" ON "TaxRules" ("TaxJurisdictionId");

CREATE INDEX "IX_TldRegistryRules_IsActive" ON "TldRegistryRules" ("IsActive");

CREATE INDEX "IX_TldRegistryRules_TldId" ON "TldRegistryRules" ("TldId");

CREATE UNIQUE INDEX "IX_Tlds_Extension" ON "Tlds" ("Extension");

CREATE INDEX "IX_Tlds_IsActive" ON "Tlds" ("IsActive");

CREATE INDEX "IX_TldSalesPricing_TldId" ON "TldSalesPricing" ("TldId");

CREATE INDEX "IX_Tokens_UserId" ON "Tokens" ("UserId");

CREATE UNIQUE INDEX "IX_Units_Code" ON "Units" ("Code");

CREATE INDEX "IX_Units_IsActive" ON "Units" ("IsActive");

CREATE INDEX "IX_UserRoles_RoleId" ON "UserRoles" ("RoleId");

CREATE INDEX "IX_Users_CustomerId" ON "Users" ("CustomerId");

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

CREATE UNIQUE INDEX "IX_Users_NormalizedUsername" ON "Users" ("NormalizedUsername");

CREATE UNIQUE INDEX "IX_Users_Username" ON "Users" ("Username");

CREATE INDEX "IX_VendorCosts_InvoiceLineId" ON "VendorCosts" ("InvoiceLineId");

CREATE INDEX "IX_VendorCosts_IsRefundable" ON "VendorCosts" ("IsRefundable");

CREATE INDEX "IX_VendorCosts_Status" ON "VendorCosts" ("Status");

CREATE INDEX "IX_VendorCosts_VendorPayoutId" ON "VendorCosts" ("VendorPayoutId");

CREATE INDEX "IX_VendorCosts_VendorType" ON "VendorCosts" ("VendorType");

CREATE INDEX "IX_VendorPayouts_InterventionResolvedByUserId" ON "VendorPayouts" ("InterventionResolvedByUserId");

CREATE INDEX "IX_VendorPayouts_RequiresManualIntervention" ON "VendorPayouts" ("RequiresManualIntervention");

CREATE INDEX "IX_VendorPayouts_ScheduledDate" ON "VendorPayouts" ("ScheduledDate");

CREATE INDEX "IX_VendorPayouts_Status" ON "VendorPayouts" ("Status");

CREATE INDEX "IX_VendorPayouts_VendorId" ON "VendorPayouts" ("VendorId");

CREATE INDEX "IX_VendorPayouts_VendorType" ON "VendorPayouts" ("VendorType");

CREATE INDEX "IX_VendorTaxProfiles_Require1099" ON "VendorTaxProfiles" ("Require1099");

CREATE UNIQUE INDEX "IX_VendorTaxProfiles_VendorId_VendorType" ON "VendorTaxProfiles" ("VendorId", "VendorType");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260319112641_Initial', '10.0.5');

COMMIT;

BEGIN TRANSACTION;
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260319120038_MySql_IndexKeyLengthFix', '10.0.5');

COMMIT;

