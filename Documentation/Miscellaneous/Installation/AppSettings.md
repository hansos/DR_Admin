# AppSettings Reference

This document provides a detailed description of every configuration section in the `appsettings.json` file used by the **DR_Admin** API project. Each section includes a description of its purpose, the available keys, their types, default values, and usage notes.

> **Tip:** Use `appsettings.Development.json` to override values for local development without modifying the base `appsettings.json`.

[Back to index](./index.md)

---

## Table of Contents

| Section                                         | Description                                          |
| ----------------------------------------------- | ---------------------------------------------------- |
| [AppSettings](#appsettings)                     | Frontend site URLs and routing paths                 |
| [SandboxSettings](#sandboxsettings)             | Sandbox/simulation mode for external integrations    |
| [Logging](#logging)                             | ASP.NET Core built-in log levels                     |
| [DataCollections](#datacollections)             | External data source URLs for TLD/public-suffix data |
| [AllowedHosts](#allowedhosts)                   | Host header filtering                                |
| [ConnectionStrings](#connectionstrings)         | Database connection strings                          |
| [DbSettings](#dbsettings)                       | Database provider selection                          |
| [JwtSettings](#jwtsettings)                     | JWT authentication configuration                     |
| [CorsSettings](#corssettings)                   | Cross-Origin Resource Sharing policy                 |
| [Serilog](#serilog)                             | Structured logging with Serilog                      |
| [RegistrarSettings](#registrarsettings)         | Domain registrar provider selection and credentials  |
| [DatabaseBackup](#databasebackup)               | Automatic database backup scheduling                 |
| [DomainRegistration](#domainregistration)       | Domain registration workflow options                 |
| [EmailSettings](#emailsettings)                 | Email sender plugin selection and provider config    |
| [EmailReceiverSettings](#emailreceiversettings) | Email receiver plugin selection and provider config  |
| [TldPricing](#tldpricing)                       | TLD pricing retention, margins, and alerts           |
| [ReportSettings](#reportsettings)               | Report generation provider and output settings       |
| [ExchangeRate](#exchangerate)                   | Currency exchange rate provider and scheduling       |
| [Stripe](#stripe)                               | Stripe payment gateway credentials                   |

---

## AppSettings

Controls how the API resolves frontend panel URLs for actions such as email confirmation links and password reset links.

```json
"AppSettings": {
  "DefaultFrontendSiteCode": "reseller",
  "FrontendSites": [
    {
      "Code": "reseller",
      "BaseUrl": "https://localhost:7246",
      "EmailConfirmationPath": "/my-account/confirm-email",
      "PasswordResetPath": "/my-account/change-password"
    },
    {
      "Code": "shop",
      "BaseUrl": "https://localhost:7555",
      "EmailConfirmationPath": "/my-account/confirm-email",
      "PasswordResetPath": "/my-account/change-password"
    }
  ]
}
```

| Key                                     | Type   | Default             | Description                                                                                                   |
| --------------------------------------- | ------ | ------------------- | ------------------------------------------------------------------------------------------------------------- |
| `DefaultFrontendSiteCode`               | string | `"reseller"`        | The `Code` of the frontend site used as the default when the originating panel is unknown                     |
| `FrontendSites`                         | array  | `[]`                | List of frontend panel definitions. Each entry maps a site code to its base URL and authentication page paths |
| `FrontendSites[].Code`                  | string | —                   | Unique identifier for the frontend panel (e.g., `"reseller"`, `"shop"`)                                       |
| `FrontendSites[].BaseUrl`               | string | —                   | Absolute base URL of the frontend panel                                                                       |
| `FrontendSites[].EmailConfirmationPath` | string | `"/confirm-email"`  | Relative path appended to `BaseUrl` for email confirmation links                                              |
| `FrontendSites[].PasswordResetPath`     | string | `"/reset-password"` | Relative path appended to `BaseUrl` for password reset links                                                  |

If `FrontendSites` is empty and the legacy `FrontendBaseUrl` property is set, the application will automatically create a single site entry using `DefaultFrontendSiteCode`, `FrontendBaseUrl`, `EmailConfirmationPath`, and `PasswordResetPath`.

---

## SandboxSettings

Enables sandbox (simulation) mode for external integrations. When sandbox mode is active, real provider APIs are replaced by built-in sandbox implementations that return dummy data.

```json
"SandboxSettings": {
  "Enabled": true,
  "Filters": {
    "DomainRegistrationLib": true
  }
}
```

| Key                             | Type | Default | Description                                                                                                                        |
| ------------------------------- | ---- | ------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| `Enabled`                       | bool | `false` | Master switch — must be `true` for any per-module sandbox filter to take effect                                                    |
| `Filters.DomainRegistrationLib` | bool | `false` | When `true` (and `Enabled` is `true`), domain registration operations use the sandbox registrar instead of the configured provider |

---

## Logging

Standard ASP.NET Core logging configuration. This section is used by the built-in logging infrastructure; for file and console output the [Serilog](#serilog) section is preferred.

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

| Key                             | Type   | Default         | Description                                            |
| ------------------------------- | ------ | --------------- | ------------------------------------------------------ |
| `LogLevel.Default`              | string | `"Information"` | Minimum log level for all categories unless overridden |
| `LogLevel.Microsoft.AspNetCore` | string | `"Warning"`     | Override for ASP.NET Core framework messages           |

---

## DataCollections

URLs used to download public TLD and domain suffix lists. These are fetched at runtime by the application to keep the domain registration feature up to date.

```json
"DataCollections": {
  "PublicSuffix": "https://publicsuffix.org/list/public_suffix_list.dat",
  "DataIana": "https://data.iana.org/TLD/tlds-alpha-by-domain.txt"
}
```

| Key            | Type   | Description                                |
| -------------- | ------ | ------------------------------------------ |
| `PublicSuffix` | string | URL to the Mozilla Public Suffix List      |
| `DataIana`     | string | URL to the IANA TLD list (alpha-by-domain) |

---

## AllowedHosts

Standard ASP.NET Core host filtering. A semicolon-separated list of allowed host names, or `"*"` to allow all hosts.

```json
"AllowedHosts": "*"
```

---

## ConnectionStrings

Database connection string used by Entity Framework Core.

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=C:\\Tmp\\DR_Admin.db"
}
```

| Key                 | Type   | Description                                                                                    |
| ------------------- | ------ | ---------------------------------------------------------------------------------------------- |
| `DefaultConnection` | string | Connection string for the application database. Format depends on the selected `DatabaseType`. |

### Connection string examples

| Database   | Example                                                                                   |
| ---------- | ----------------------------------------------------------------------------------------- |
| SQLite     | `Data Source=C:\\Tmp\\DR_Admin.db`                                                        |
| SQL Server | `Server=localhost;Database=DR_Admin;Trusted_Connection=True;TrustServerCertificate=True;` |
| PostgreSQL | `Host=localhost;Port=5432;Database=DR_Admin;Username=postgres;Password=secret;`           |
| MySQL      | `Server=localhost;Port=3306;Database=DR_Admin;User=root;Password=secret;`                 |
| MariaDB    | `Server=localhost;Port=3306;Database=DR_Admin;User=root;Password=secret;`                 |

> **Note:** The SQLite database file is created automatically on first startup.
> 
> **MySQL/MariaDB:** MySQL and MariaDB are supported using the `Pomelo.EntityFrameworkCore.MySql` fork available at [hansos/Pomelo.EntityFrameworkCore.MySql](https://github.com/hansos/Pomelo.EntityFrameworkCore.MySql).

---

## DbSettings

Selects the database provider used by Entity Framework Core.

```json
"DbSettings": {
  "DatabaseType": "SQLITE"
}
```

| Key            | Type   | Default    | Description                                                            |
| -------------- | ------ | ---------- | ---------------------------------------------------------------------- |
| `DatabaseType` | string | `"SQLITE"` | The database engine to use. See the table below for recognised values. |

### Recognised `DatabaseType` values

| Value        | Alias     | EF Core Provider                          | Status      |
| ------------ | --------- | ----------------------------------------- | ----------- |
| `SQLITE`     | `LITESQL` | `Microsoft.EntityFrameworkCore.Sqlite`    | ✅ Supported |
| `SQLSERVER`  | `MSSQL`   | `Microsoft.EntityFrameworkCore.SqlServer` | ✅ Supported |
| `POSTGRESQL` | `POSTGRE` | `Npgsql.EntityFrameworkCore.PostgreSQL`   | ✅ Supported |
| `MYSQL`      | `MYSQL`   | `Pomelo.EntityFrameworkCore.MySql` (fork) | ✅ Supported |
| `MARIADB`    | `MARIA`   | `Pomelo.EntityFrameworkCore.MySql` (fork) | ✅ Supported |

> **MySQL/MariaDB note:** The `MYSQL` database provider now supports both MySQL and MariaDB through the forked package [hansos/Pomelo.EntityFrameworkCore.MySql](https://github.com/hansos/Pomelo.EntityFrameworkCore.MySql).

Either the primary value or its alias can be used — they are treated identically (case-insensitive).

---

## JwtSettings

Configuration for JWT (JSON Web Token) authentication used by the API.

```json
"JwtSettings": {
  "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLongForSecurity",
  "Issuer": "DR_Admin",
  "Audience": "DR_Admin_Users",
  "ExpirationInMinutes": 60
}
```

| Key                   | Type   | Default            | Description                                                                 |
| --------------------- | ------ | ------------------ | --------------------------------------------------------------------------- |
| `SecretKey`           | string | —                  | **Required.** HMAC-SHA256 signing key. Must be at least 32 characters long. |
| `Issuer`              | string | `"DR_Admin"`       | The `iss` claim value embedded in generated tokens                          |
| `Audience`            | string | `"DR_Admin_Users"` | The `aud` claim value embedded in generated tokens                          |
| `ExpirationInMinutes` | int    | `60`               | Token lifetime in minutes                                                   |

> **Security:** In production, store `SecretKey` in a secrets manager or environment variable — never commit real secrets to source control.

---

## CorsSettings

Configures the Cross-Origin Resource Sharing policy applied to the API. This controls which frontend origins are allowed to make requests.

```json
"CorsSettings": {
  "PolicyName": "AllowSpecificOrigins",
  "AllowedOrigins": [
    "https://localhost:7246",
    "http://localhost:5062"
  ],
  "AllowedMethods": [ "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" ],
  "AllowedHeaders": [ "*" ],
  "AllowCredentials": true,
  "MaxAge": 600
}
```

| Key                | Type     | Default                  | Description                                                                         |
| ------------------ | -------- | ------------------------ | ----------------------------------------------------------------------------------- |
| `PolicyName`       | string   | `"AllowSpecificOrigins"` | Name of the CORS policy registered in the middleware                                |
| `AllowedOrigins`   | string[] | `[]`                     | Origins allowed to call the API. Use `["*"]` to allow any origin (not recommended). |
| `AllowedMethods`   | string[] | `[]`                     | HTTP methods permitted. Use `["*"]` for all methods.                                |
| `AllowedHeaders`   | string[] | `[]`                     | Request headers allowed. Use `["*"]` for all headers.                               |
| `AllowCredentials` | bool     | `false`                  | Whether cookies and authorization headers are sent with cross-origin requests       |
| `MaxAge`           | int      | `0`                      | Preflight cache duration in seconds. `0` disables caching.                          |

> **Important:** When `AllowCredentials` is `true`, `AllowedOrigins` must list explicit origins — wildcard `"*"` is not permitted by browsers.

---

## Serilog

Structured logging configuration using [Serilog](https://serilog.net/). DR_Admin ships with console and file sinks.

```json
"Serilog": {
  "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "System": "Warning"
    }
  },
  "WriteTo": [
    {
      "Name": "Console",
      "Args": {
        "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
      }
    },
    {
      "Name": "File",
      "Args": {
        "path": "D:\\LogFiles\\DR_Admin\\DR_Admin-.log",
        "rollingInterval": "Day",
        "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"
      }
    }
  ],
  "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
}
```

| Key                              | Type     | Description                                                                                     |
| -------------------------------- | -------- | ----------------------------------------------------------------------------------------------- |
| `Using`                          | string[] | Serilog sink assemblies to load                                                                 |
| `MinimumLevel.Default`           | string   | Global minimum log level (`Verbose`, `Debug`, `Information`, `Warning`, `Error`, `Fatal`)       |
| `MinimumLevel.Override`          | object   | Per-namespace minimum level overrides                                                           |
| `WriteTo`                        | array    | Array of sink configurations                                                                    |
| `WriteTo[].Name`                 | string   | Sink name (e.g., `"Console"`, `"File"`)                                                         |
| `WriteTo[].Args.path`            | string   | *(File sink)* Output file path. A `-` suffix before `.log` enables the rolling file date stamp. |
| `WriteTo[].Args.rollingInterval` | string   | *(File sink)* Rolling interval: `Day`, `Hour`, `Month`, etc.                                    |
| `WriteTo[].Args.outputTemplate`  | string   | Message output format template                                                                  |
| `Enrich`                         | string[] | Enrichers added to every log event                                                              |

> **Action required:** Change the `path` value in the File sink to a valid directory on your system where the application has write permissions.

---

## RegistrarSettings

Selects the active domain registrar provider and holds per-provider credential blocks. Only the provider matching the `Provider` key needs to be configured; all others can remain `null`.

```json
"RegistrarSettings": {
  "Provider": "none",
  "Namecheap": null,
  "GoDaddy": null,
  "Cloudflare": null,
  "OpenSrs": null,
  "CentralNic": null,
  "DnSimple": null,
  "Domainbox": null,
  "Oxxa": null,
  "Regtons": null,
  "DomainNameApi": null,
  "Aws": null
}
```

| Key        | Type   | Default  | Description                                                                                                                                                                                                                                              |
| ---------- | ------ | -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Provider` | string | `"none"` | Active registrar key. Set to `"none"` to disable external domain registration. Available values: `"namecheap"`, `"godaddy"`, `"cloudflare"`, `"opensrs"`, `"centralnic"`, `"dnsimple"`, `"domainbox"`, `"oxxa"`, `"regtons"`, `"domainnameapi"`, `"aws"` |

Each provider block contains credentials specific to that registrar (API keys, secrets, endpoints). Populate only the block for the provider set in `Provider`. Example for AWS:

```json
"Aws": {
  "AccessKeyId": "YOUR_ACCESS_KEY",
  "SecretAccessKey": "YOUR_SECRET_KEY",
  "Region": "us-east-1",
  "Route53HostedZoneId": "YOUR_HOSTED_ZONE_ID"
}
```

> **Note:** When [SandboxSettings](#sandboxsettings) is enabled for `DomainRegistrationLib`, the sandbox registrar is used regardless of this setting.

---

## DatabaseBackup

Configures the automatic database backup background service.

```json
"DatabaseBackup": {
  "Enabled": true,
  "BackupLocation": "C:\\Tmp\\Backup\\dr",
  "FrequencyInHours": 24,
  "MaxBackupsToKeep": 20,
  "CompressBackup": true,
  "RunOnStartup": true
}
```

| Key                | Type   | Default | Description                                                       |
| ------------------ | ------ | ------- | ----------------------------------------------------------------- |
| `Enabled`          | bool   | `true`  | Whether the backup background service is active                   |
| `BackupLocation`   | string | `""`    | **Required.** Directory path where backup files are stored        |
| `FrequencyInHours` | int    | `24`    | Interval in hours between automatic backups                       |
| `MaxBackupsToKeep` | int    | `7`     | Maximum number of backup files to retain; older files are deleted |
| `CompressBackup`   | bool   | `true`  | Whether to compress backup files (SQLite only)                    |
| `RunOnStartup`     | bool   | `false` | Whether to run a backup immediately when the application starts   |

---

## DomainRegistration

Controls the domain registration workflow behavior, including approval gates and pricing features.

```json
"DomainRegistration": {
  "RequireApprovalForCustomers": false,
  "RequireApprovalForSales": false,
  "AllowCustomerRegistration": true,
  "DefaultRegistrationYears": 1,
  "EnableAvailabilityCheck": false,
  "EnablePricingCheck": true
}
```

| Key                           | Type | Default | Description                                                                                      |
| ----------------------------- | ---- | ------- | ------------------------------------------------------------------------------------------------ |
| `RequireApprovalForCustomers` | bool | `false` | Whether customer self-service domain registrations require admin/sales approval before execution |
| `RequireApprovalForSales`     | bool | `false` | Whether domain registrations initiated by sales/admin staff require additional approval          |
| `AllowCustomerRegistration`   | bool | `true`  | Whether customers are allowed to register domains through self-service                           |
| `DefaultRegistrationYears`    | int  | `1`     | Default registration period in years (used as the initial value in the UI)                       |
| `EnableAvailabilityCheck`     | bool | `false` | Whether to check domain availability with the registrar before allowing registration             |
| `EnablePricingCheck`          | bool | `true`  | Whether to fetch real-time pricing from the registrar                                            |

---

## EmailSettings

Configures the email sender plugin system. The plugin selection block determines which plugin is used at runtime, and each provider block holds that provider's credentials.

For a full guide on email plugin installation and configuration, see [Email Plugins](Email_Plugins.md).

```json
"EmailSettings": {
  "Provider": "smtp",
  "TemplatesPath": "Templates",
  "Selection": {
    "DefaultPluginKey": "smtp",
    "EnabledPluginKeys": [ "smtp" ],
    "FallbackPluginKeys": [],
    "DisabledPluginKeys": []
  },
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "user@example.com",
    "Password": "secret",
    "EnableSsl": true,
    "FromEmail": "noreply@example.com",
    "FromName": "DR Admin"
  }
}
```

| Key                            | Type     | Default       | Description                                                                                 |
| ------------------------------ | -------- | ------------- | ------------------------------------------------------------------------------------------- |
| `Provider`                     | string   | `""`          | Legacy provider key (kept for backward compatibility). Prefer `Selection.DefaultPluginKey`. |
| `TemplatesPath`                | string   | `"Templates"` | File system path to email template files                                                    |
| `Selection.DefaultPluginKey`   | string   | `""`          | Preferred plugin key used when no explicit key is requested at runtime                      |
| `Selection.EnabledPluginKeys`  | string[] | `[]`          | Plugin keys that are loaded and available for use                                           |
| `Selection.FallbackPluginKeys` | string[] | `[]`          | Plugin keys tried sequentially if the selected plugin fails                                 |
| `Selection.DisabledPluginKeys` | string[] | `[]`          | Plugin keys explicitly excluded regardless of other settings                                |

Available plugin keys: `smtp`, `mailkit`, `graphapi`, `exchange`, `sendgrid`, `mailgun`, `amazonses`, `postmark`.

---

## EmailReceiverSettings

Configures the email receiver plugin system used for inbound email processing.

```json
"EmailReceiverSettings": {
  "Provider": "office365",
  "Selection": {
    "EnabledPluginKeys": [ "office365" ],
    "DefaultPluginKey": "office365",
    "FallbackPluginKeys": [],
    "DisabledPluginKeys": []
  },
  "Office365": {
    "TenantId": "",
    "ClientId": "",
    "MailboxAddress": "",
    "AliasRecipient": "",
    "DefaultFolder": "Inbox",
    "DefaultMaxItems": 25
  }
}
```

| Key                            | Type     | Default   | Description                                        |
| ------------------------------ | -------- | --------- | -------------------------------------------------- |
| `Provider`                     | string   | `""`      | Active email receiver provider key                 |
| `Selection.DefaultPluginKey`   | string   | `""`      | Preferred receiver plugin key                      |
| `Selection.EnabledPluginKeys`  | string[] | `[]`      | Receiver plugin keys loaded and available          |
| `Selection.FallbackPluginKeys` | string[] | `[]`      | Fallback receiver plugin keys                      |
| `Selection.DisabledPluginKeys` | string[] | `[]`      | Explicitly disabled receiver plugin keys           |
| `Office365.TenantId`           | string   | `""`      | Azure AD tenant ID                                 |
| `Office365.ClientId`           | string   | `""`      | Azure AD application (client) ID                   |
| `Office365.MailboxAddress`     | string   | `""`      | Mailbox email address to read from                 |
| `Office365.AliasRecipient`     | string   | `""`      | Alias address used for filtering incoming messages |
| `Office365.DefaultFolder`      | string   | `"Inbox"` | Mail folder to read from                           |
| `Office365.DefaultMaxItems`    | int      | `25`      | Maximum number of items to fetch per polling cycle |

---

## TldPricing

Controls TLD pricing data retention, margin monitoring, and automated archiving.

```json
"TldPricing": {
  "CostPricingRetentionYears": 7,
  "SalesPricingRetentionYears": 5,
  "DiscountHistoryRetentionYears": 5,
  "MinimumMarginPercentage": 5.0,
  "EnableNegativeMarginAlerts": true,
  "EnableLowMarginAlerts": true,
  "MarginAlertEmails": "pricing@yourcompany.com,admin@yourcompany.com",
  "AllowEditingFuturePrices": true,
  "MaxScheduleDays": 365,
  "DefaultSalesCurrency": "USD",
  "EnableAutoCurrencyConversion": true,
  "CurrencyConversionMarkup": 0.0,
  "CurrentPriceCacheMinutes": 60,
  "AllowDiscountStacking": false,
  "EnableAutoArchiving": true,
  "AutoArchivingHour": 2
}
```

| Key                             | Type    | Default | Description                                                      |
| ------------------------------- | ------- | ------- | ---------------------------------------------------------------- |
| `CostPricingRetentionYears`     | int     | `7`     | Years to retain cost pricing history for compliance              |
| `SalesPricingRetentionYears`    | int     | `5`     | Years to retain sales pricing history                            |
| `DiscountHistoryRetentionYears` | int     | `5`     | Years to retain discount history                                 |
| `MinimumMarginPercentage`       | decimal | `5.0`   | Minimum acceptable margin percentage before alerts trigger       |
| `EnableNegativeMarginAlerts`    | bool    | `true`  | Send alerts when margins are negative (cost exceeds price)       |
| `EnableLowMarginAlerts`         | bool    | `true`  | Send alerts when margins fall below `MinimumMarginPercentage`    |
| `MarginAlertEmails`             | string  | `""`    | Comma-separated email addresses to receive margin alerts         |
| `AllowEditingFuturePrices`      | bool    | `true`  | Whether future prices can be edited before their effective date  |
| `MaxScheduleDays`               | int     | `365`   | Maximum number of days in advance that prices can be scheduled   |
| `DefaultSalesCurrency`          | string  | `"USD"` | Default ISO 4217 currency code for sales pricing                 |
| `EnableAutoCurrencyConversion`  | bool    | `true`  | Whether to automatically convert currencies using exchange rates |
| `CurrencyConversionMarkup`      | decimal | `0.0`   | Additional markup percentage applied on currency conversions     |
| `CurrentPriceCacheMinutes`      | int     | `60`    | Minutes to cache current pricing queries                         |
| `AllowDiscountStacking`         | bool    | `false` | Whether discounts can be combined with promotional prices        |
| `EnableAutoArchiving`           | bool    | `true`  | Whether old pricing data is automatically archived on a schedule |
| `AutoArchivingHour`             | int     | `2`     | Hour of day (0–23) when auto-archiving runs                      |

---

## ReportSettings

Configures the report generation provider used for invoices and other documents.

```json
"ReportSettings": {
  "Provider": "questpdf",
  "FastReport": null,
  "QuestPdf": {
    "OutputPath": "C:\\Tmp\\Reports",
    "DefaultFormat": "PDF",
    "LicenseType": "Community"
  }
}
```

| Key                                 | Type   | Default       | Description                                                |
| ----------------------------------- | ------ | ------------- | ---------------------------------------------------------- |
| `Provider`                          | string | `""`          | Active report provider key: `"questpdf"` or `"fastreport"` |
| `QuestPdf.OutputPath`               | string | `""`          | Directory path where generated reports are written         |
| `QuestPdf.DefaultFormat`            | string | `"PDF"`       | Default output format                                      |
| `QuestPdf.LicenseType`              | string | `"Community"` | QuestPDF license type (`"Community"` for the free tier)    |
| `FastReport.TemplatesPath`          | string | `""`          | Path to FastReport template files                          |
| `FastReport.OutputPath`             | string | `""`          | Directory path for FastReport output                       |
| `FastReport.DefaultFormat`          | string | `"PDF"`       | Default output format                                      |
| `FastReport.LicenseKey`             | string | `null`        | FastReport license key (required for commercial use)       |
| `FastReport.EnableCache`            | bool   | `true`        | Whether to cache compiled report templates                 |
| `FastReport.CacheExpirationMinutes` | int    | `30`          | Cache expiration time in minutes                           |

> Populate only the provider block that matches the `Provider` key.

---

## ExchangeRate

Configures the currency exchange rate update service, including provider selection and scheduling.

```json
"ExchangeRate": {
  "Provider": "exchangeratehost",
  "Frankfurter": {
    "ApiUrl": "https://api.frankfurter.app",
    "UseHttps": true
  },
  "ExchangeRateHost": {
    "ApiUrl": "https://api.exchangerate.host/",
    "ApiKey": "YOUR_API_KEY",
    "UseHttps": true
  },
  "MaxUpdatesPerDay": 1,
  "HoursBetweenUpdates": 24,
  "UpdateOnStartup": true
}
```

| Key                         | Type   | Default                            | Description                                                                     |
| --------------------------- | ------ | ---------------------------------- | ------------------------------------------------------------------------------- |
| `Provider`                  | string | `""`                               | Active exchange rate provider key (e.g., `"exchangeratehost"`, `"frankfurter"`) |
| `MaxUpdatesPerDay`          | int    | `24`                               | Maximum number of rate updates per day (`0` = unlimited)                        |
| `HoursBetweenUpdates`       | int    | `1`                                | Minimum hours between consecutive updates                                       |
| `UpdateOnStartup`           | bool   | `true`                             | Whether to fetch rates immediately on application startup                       |
| `Frankfurter.ApiUrl`        | string | `"https://api.frankfurter.app"`    | Frankfurter API base URL                                                        |
| `Frankfurter.UseHttps`      | bool   | `true`                             | Whether to use HTTPS for Frankfurter requests                                   |
| `ExchangeRateHost.ApiUrl`   | string | `"https://api.exchangerate.host/"` | ExchangeRate.host API base URL                                                  |
| `ExchangeRateHost.ApiKey`   | string | `null`                             | API key for ExchangeRate.host                                                   |
| `ExchangeRateHost.UseHttps` | bool   | `true`                             | Whether to use HTTPS for ExchangeRate.host requests                             |

Additional providers (OpenExchangeRates, CurrencyLayer, Fixer, Xe, Oanda) follow the same pattern with their own credential blocks.

---

## Stripe

Stripe payment gateway credentials. Used when Stripe is configured as the payment gateway.

```json
"Stripe": {
  "PublishableKey": "pk_test_...",
  "SecretKey": "sk_test_...",
  "WebhookSecret": "",
  "Currency": "usd"
}
```

| Key              | Type   | Default | Description                                                        |
| ---------------- | ------ | ------- | ------------------------------------------------------------------ |
| `PublishableKey` | string | `""`    | Stripe publishable (public) API key                                |
| `SecretKey`      | string | `""`    | Stripe secret API key                                              |
| `WebhookSecret`  | string | `""`    | Secret used to verify Stripe webhook signatures                    |
| `Currency`       | string | `"usd"` | Default currency code for Stripe transactions (lowercase ISO 4217) |

> **Security:** Use test keys (`pk_test_`, `sk_test_`) during development. Never commit live keys to source control.

---

## Environment-Specific Overrides

ASP.NET Core merges configuration files in order. Values in `appsettings.Development.json` override those in `appsettings.json` when the `ASPNETCORE_ENVIRONMENT` is set to `Development`.

Typical overrides for development include:

- Pointing `ConnectionStrings.DefaultConnection` to a local database path
- Setting `SandboxSettings.Enabled` to `false` for direct registrar testing
- Lowering `Serilog.MinimumLevel.Default` to `Debug`
- Providing real email and registrar credentials for integration testing

---

## See Also

- [Getting the Source Code](Installation.md)
- [Configuration Quick Start](Configuration.md)
- [Build](Build.md)
- [Email Plugins](Email_Plugins.md)
