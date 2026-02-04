# API Surface Overview

This document lists the HTTP API endpoints grouped by functionality for the project. All controllers use the base route `api/v1/{controller}` (controller name without the `Controller` suffix) unless otherwise noted.

## Authentication & security

- `AuthController` (base: `api/v1/auth`)
  - `POST /api/v1/auth/login` - login (returns JWT)
  - `POST /api/v1/auth/refresh` - refresh token
  - `POST /api/v1/auth/logout` - revoke refresh token
  - `GET /api/v1/auth/verify` - token verification
- `TokensController` - token management (refresh/revoke/list endpoints)
- `UsersController`, `RolesController`, `MyAccountController` - user, role and account endpoints (typical CRUD + profile operations)

## System & administration

- `SystemController` (base: `api/v1/system`)
  - `POST /api/v1/system/normalize-all-records` - normalize database fields for exact searches
  - `GET /api/v1/system/health` - health check
  - `POST /api/v1/system/backup` - create DB backup
  - `POST /api/v1/system/restore` - restore DB from backup
- `InitializationController` - setup / bootstrap endpoints
- `ExchangeRateDownloadLogsController` - exchange-rate support and logs
- `EmailQueueController`, `SentEmailsController` - email queue and sent email management
- `DocumentTemplatesController`, `ReportTemplatesController` - template management

## Customers, resellers & contacts

- `CustomersController` - customers (list, get, create, update, delete)
- `ResellerCompaniesController` - reseller company CRUD
- `ContactPersonsController` - contact person endpoints
- `CustomerPaymentMethodsController`, `CustomerCreditsController`, `CustomerStatusesController` - payment methods, credits and statuses

## Billing, orders & subscriptions

- `BillingCyclesController` - billing cycles (CRUD)
- `InvoicesController`, `InvoiceLinesController` - invoices and invoice lines
- `OrdersController`, `QuotesController`, `RefundsController` - orders/quotes/refunds
- `PaymentGatewaysController`, `PaymentIntentsController` - payment gateway config and payment intents
- `CouponsController` - coupon management
- `SubscriptionsController`, `SubscriptionBillingHistoriesController` - subscriptions and billing history

## Products / Hosting / Servers / Control panels

- `HostingPackagesController` - hosting package CRUD
- `ServiceTypesController`, `ServicesController` - service types and services
- `ServersController`, `ServerControlPanelsController`, `ServerIpAddressesController` - servers, control panels and IPs
- `ControlPanelTypesController` - control panel types
- `NameServersController` - nameserver records for hosting

## Domains & DNS

- `DomainsController` - domain CRUD and domain operations
- `DomainContactsController` - domain contact management
- `RegistrarsController`, `RegistrarTldsController`, `TldsController` - registrars and TLD management
- `DnsZonePackagesController`, `DnsZonePackageRecordsController`, `DnsRecordsController`, `DnsRecordTypesController` - DNS zone packages, records and record types

## Geography & localization

- `CountriesController` - country data (CRUD, lookup)
- `PostalCodesController` - postal code lookup and maintenance
- `CurrenciesController` - currency data
- `UnitsController` - measurement units
- `TaxRulesController` - tax rules and lookup

## Reporting & utilities

- `ReportTemplatesController` - report template CRUD
- `ExchangeRateDownloadLogsController` - exchange-rate download log listing
- Misc utilities: `EmailQueueController`, `SentEmailsController`, logs, diagnostics endpoints


