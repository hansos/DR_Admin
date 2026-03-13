# Financial Tracking System - Tax and Vendor Management

## Overview

The **Financial Tracking System** is a comprehensive module added to the DR_Admin ISP Administration System to manage vendor costs, tax compliance, refund auditing, and vendor payouts. This system provides complete visibility into financial operations, cost tracking, and tax reporting requirements.

## Key Features

### 1. **Vendor Cost Management**
- Track vendor costs associated with invoice line items
- Support multi-currency operations with exchange rate tracking
- Monitor refundable vs. non-refundable costs
- Track vendor cost lifecycle (Estimated ? Committed ? Paid ? Refunded)
- Generate profit margin analysis and cost summaries per invoice

### 2. **Tax Profile Management**

**Customer Tax Profiles:**
- Store and validate VAT IDs, EINs, and other tax identification numbers
- Support B2B and B2C customer classifications
- Manage tax exemptions with certificate tracking
- Tax ID validation with external services integration

**Vendor Tax Profiles:**
- Track vendor tax information (W-9, 1099 requirements)
- Manage withholding tax rates and treaty exemptions
- Support for international tax compliance

### 3. **Vendor Payout Processing**
- Schedule and process vendor payments
- Support multiple payout methods (Bank Transfer, PayPal, Check, Wire Transfer, ACH)
- Track payout status from Pending ? Processing ? Paid
- Handle failed payouts with retry mechanisms
- Manual intervention workflow for complex payout issues
- Multi-currency payout support with exchange rate snapshots

### 4. **Refund Loss Auditing**
- Calculate and track financial losses from customer refunds
- Identify unrecoverable vendor costs
- Approval/denial workflow for loss recognition
- Track refund impact on profit margins
- Comprehensive audit trail for financial reporting

## Architecture

### Database Layer
**5 New Entities:**
- `CustomerTaxProfile` - Customer tax information and validation
- `VendorTaxProfile` - Vendor tax compliance data
- `VendorCost` - Cost tracking per invoice line
- `VendorPayout` - Vendor payment records
- `RefundLossAudit` - Financial loss tracking

**9 New Enums:**
- `TaxIdType`, `CustomerType`, `VendorType`
- `VendorCostStatus`, `RefundPolicy`
- `VendorPayoutStatus`, `PayoutMethod`
- `ApprovalStatus`, `InvoiceLineType`

### Service Layer
**5 Service Interfaces + Implementations:**
- `ICustomerTaxProfileService` / `CustomerTaxProfileService`
- `IVendorCostService` / `VendorCostService`
- `IVendorPayoutService` / `VendorPayoutService`
- `IVendorTaxProfileService` / `VendorTaxProfileService`
- `IRefundLossAuditService` / `RefundLossAuditService`

### API Layer
**5 RESTful Controllers with 45 Endpoints:**
- `CustomerTaxProfilesController` - 8 endpoints
- `VendorCostsController` - 9 endpoints
- `VendorPayoutsController` - 10 endpoints
- `VendorTaxProfilesController` - 6 endpoints
- `RefundLossAuditsController` - 8 endpoints

### Data Transfer Layer
**22 DTOs:**
- 5 Read DTOs
- 5 Create DTOs
- 4 Update DTOs
- 8 Specialized DTOs (validation, processing, summaries)

## API Endpoints

### Customer Tax Profiles
```
GET    /api/v1/customertaxprofiles
GET    /api/v1/customertaxprofiles/{id}
GET    /api/v1/customertaxprofiles/customer/{customerId}
POST   /api/v1/customertaxprofiles
PUT    /api/v1/customertaxprofiles/{id}
DELETE /api/v1/customertaxprofiles/{id}
POST   /api/v1/customertaxprofiles/validate
```

### Vendor Costs
```
GET    /api/v1/vendorcosts
GET    /api/v1/vendorcosts/paged?pageNumber=1&pageSize=20
GET    /api/v1/vendorcosts/{id}
GET    /api/v1/vendorcosts/invoice-line/{invoiceLineId}
GET    /api/v1/vendorcosts/payout/{payoutId}
GET    /api/v1/vendorcosts/summary/invoice/{invoiceId}
POST   /api/v1/vendorcosts
PUT    /api/v1/vendorcosts/{id}
DELETE /api/v1/vendorcosts/{id}
```

### Vendor Payouts
```
GET    /api/v1/vendorpayouts
GET    /api/v1/vendorpayouts/paged?pageNumber=1&pageSize=20
GET    /api/v1/vendorpayouts/{id}
GET    /api/v1/vendorpayouts/vendor/{vendorId}
GET    /api/v1/vendorpayouts/summary/vendor/{vendorId}
POST   /api/v1/vendorpayouts
PUT    /api/v1/vendorpayouts/{id}
DELETE /api/v1/vendorpayouts/{id}
POST   /api/v1/vendorpayouts/process
POST   /api/v1/vendorpayouts/resolve-intervention
```

### Vendor Tax Profiles
```
GET    /api/v1/vendortaxprofiles
GET    /api/v1/vendortaxprofiles/{id}
GET    /api/v1/vendortaxprofiles/vendor/{vendorId}
POST   /api/v1/vendortaxprofiles
PUT    /api/v1/vendortaxprofiles/{id}
DELETE /api/v1/vendortaxprofiles/{id}
```

### Refund Loss Audits
```
GET    /api/v1/refundlossaudits
GET    /api/v1/refundlossaudits/paged?pageNumber=1&pageSize=20
GET    /api/v1/refundlossaudits/{id}
GET    /api/v1/refundlossaudits/refund/{refundId}
GET    /api/v1/refundlossaudits/invoice/{invoiceId}
POST   /api/v1/refundlossaudits
POST   /api/v1/refundlossaudits/approve
DELETE /api/v1/refundlossaudits/{id}
```

## Security & Authorization

All endpoints are protected with role-based authorization:
- **Admin** - Full access including deletions
- **Finance** - Create, read, update operations
- **Support** - Read-only access to most resources

## Integration Points

- **Invoices** - Vendor costs linked to invoice lines
- **Refunds** - Automatic loss audit creation
- **Customers** - Tax profile association
- **Exchange Rates** - Multi-currency support with snapshot tracking
- **Audit Logs** - Complete tracking of all financial operations

## Usage Scenarios

### 1. Cost Tracking
When an invoice line is created for a domain registration, create a `VendorCost` record to track the registrar fee:

```http
POST /api/v1/vendorcosts
{
  "invoiceLineId": 123,
  "vendorType": "DomainRegistrar",
  "vendorId": 5,
  "vendorName": "GoDaddy",
  "vendorCurrency": "USD",
  "vendorAmount": 12.99,
  "baseCurrency": "EUR",
  "baseAmount": 11.50,
  "exchangeRate": 1.13,
  "exchangeRateDate": "2024-02-09T00:00:00Z",
  "isRefundable": true,
  "refundPolicy": "FullyRefundable",
  "refundDeadline": "2024-03-09T00:00:00Z"
}
```

### 2. Tax Compliance
Validate customer VAT IDs before invoice generation:

```http
POST /api/v1/customertaxprofiles/validate
{
  "customerTaxProfileId": 42,
  "forceRevalidation": false
}
```

### 3. Vendor Payments
Schedule batch payouts to vendors:

```http
POST /api/v1/vendorpayouts
{
  "vendorId": 5,
  "vendorType": "DomainRegistrar",
  "vendorName": "GoDaddy",
  "payoutMethod": "BankTransfer",
  "vendorCurrency": "USD",
  "vendorAmount": 1299.00,
  "baseCurrency": "EUR",
  "baseAmount": 1150.00,
  "exchangeRate": 1.13,
  "exchangeRateDate": "2024-02-09T00:00:00Z",
  "scheduledDate": "2024-02-15T00:00:00Z",
  "vendorCostIds": [101, 102, 103]
}
```

Process the payout:

```http
POST /api/v1/vendorpayouts/process
{
  "vendorPayoutId": 15,
  "transactionReference": "TXN-2024-02-15-001",
  "paymentGatewayResponse": "{\"status\":\"success\"}",
  "isSuccessful": true
}
```

### 4. Refund Analysis
When processing a refund, create a loss audit to track unrecoverable costs:

```http
POST /api/v1/refundlossaudits
{
  "refundId": 78,
  "invoiceId": 456,
  "originalInvoiceAmount": 150.00,
  "refundedAmount": 150.00,
  "vendorCostUnrecoverable": 45.00,
  "netLoss": 45.00,
  "currency": "EUR",
  "reason": "Customer requested cancellation after domain registration",
  "internalNotes": "Domain was already registered with registrar"
}
```

Get management approval:

```http
POST /api/v1/refundlossaudits/approve
{
  "refundLossAuditId": 12,
  "approvedByUserId": 3,
  "isApproved": true,
  "notes": "Approved by CFO - Valid business loss"
}
```

### 5. Financial Reporting
Generate cost summaries per invoice to analyze profit margins:

```http
GET /api/v1/vendorcosts/summary/invoice/456
```

Response:
```json
{
  "invoiceId": 456,
  "invoiceNumber": "INV-2024-001",
  "invoiceTotal": 150.00,
  "currencyCode": "EUR",
  "totalVendorCosts": 45.00,
  "totalPaidVendorCosts": 45.00,
  "totalUnpaidVendorCosts": 0.00,
  "totalRefundableVendorCosts": 0.00,
  "totalNonRefundableVendorCosts": 45.00,
  "grossProfit": 105.00,
  "grossProfitMargin": 70.00,
  "vendorCosts": [...]
}
```

## Data Models

### CustomerTaxProfile
```csharp
{
  "id": 1,
  "customerId": 42,
  "taxIdNumber": "DE123456789",
  "taxIdType": "VAT",
  "taxIdValidated": true,
  "taxIdValidationDate": "2024-02-09T10:00:00Z",
  "taxResidenceCountry": "DE",
  "customerType": "B2B",
  "taxExempt": false,
  "createdAt": "2024-01-15T00:00:00Z",
  "updatedAt": "2024-02-09T10:00:00Z"
}
```

### VendorCost
```csharp
{
  "id": 101,
  "invoiceLineId": 123,
  "vendorPayoutId": 15,
  "vendorType": "DomainRegistrar",
  "vendorId": 5,
  "vendorName": "GoDaddy",
  "vendorCurrency": "USD",
  "vendorAmount": 12.99,
  "baseCurrency": "EUR",
  "baseAmount": 11.50,
  "exchangeRate": 1.13,
  "exchangeRateDate": "2024-02-09T00:00:00Z",
  "isRefundable": false,
  "refundPolicy": "NonRefundable",
  "status": "Paid",
  "notes": "Annual .com registration",
  "createdAt": "2024-02-09T12:00:00Z",
  "updatedAt": "2024-02-15T09:00:00Z"
}
```

### VendorPayout
```csharp
{
  "id": 15,
  "vendorId": 5,
  "vendorType": "DomainRegistrar",
  "vendorName": "GoDaddy",
  "payoutMethod": "BankTransfer",
  "vendorCurrency": "USD",
  "vendorAmount": 1299.00,
  "baseCurrency": "EUR",
  "baseAmount": 1150.00,
  "exchangeRate": 1.13,
  "exchangeRateDate": "2024-02-09T00:00:00Z",
  "status": "Paid",
  "scheduledDate": "2024-02-15T00:00:00Z",
  "processedDate": "2024-02-15T09:00:00Z",
  "failureCount": 0,
  "transactionReference": "TXN-2024-02-15-001",
  "requiresManualIntervention": false,
  "vendorCosts": [...],
  "createdAt": "2024-02-09T12:00:00Z",
  "updatedAt": "2024-02-15T09:00:00Z"
}
```

### RefundLossAudit
```csharp
{
  "id": 12,
  "refundId": 78,
  "invoiceId": 456,
  "originalInvoiceAmount": 150.00,
  "refundedAmount": 150.00,
  "vendorCostUnrecoverable": 45.00,
  "netLoss": 45.00,
  "currency": "EUR",
  "reason": "Customer cancellation",
  "approvalStatus": "Approved",
  "approvedByUserId": 3,
  "approvedAt": "2024-02-09T14:00:00Z",
  "internalNotes": "Domain already registered",
  "createdAt": "2024-02-09T13:00:00Z",
  "updatedAt": "2024-02-09T14:00:00Z"
}
```

## Enum Values

### TaxIdType
- `None` (0) - No tax ID
- `VAT` (1) - VAT identification number
- `EIN` (2) - Employer Identification Number (US)
- `SSN` (3) - Social Security Number (US)
- `TaxRegistrationNumber` (4) - General tax registration number
- `NationalID` (5) - National identification number

### CustomerType
- `B2C` (0) - Business to Consumer
- `B2B` (1) - Business to Business

### VendorType
- `DomainRegistrar` (0) - Domain name registrar
- `HostingProvider` (1) - Web hosting provider
- `SslCertificateProvider` (2) - SSL certificate provider
- `PaymentGateway` (3) - Payment processing service
- `Other` (99) - Other vendor types

### VendorCostStatus
- `Estimated` (0) - Cost is projected but not committed
- `Committed` (1) - Cost is locked in
- `Paid` (2) - Vendor has been paid
- `Refunded` (3) - Cost was refunded by vendor
- `Unrecoverable` (4) - Cost cannot be recovered

### RefundPolicy
- `FullyRefundable` (0) - 100% refundable
- `PartiallyRefundable` (1) - Partially refundable
- `NonRefundable` (2) - Cannot be refunded

### VendorPayoutStatus
- `Pending` (0) - Scheduled but not processed
- `Processing` (1) - Currently being processed
- `Paid` (2) - Successfully completed
- `Failed` (3) - Failed and needs retry
- `Cancelled` (4) - Cancelled

### PayoutMethod
- `BankTransfer` (0) - Direct bank transfer
- `PayPal` (1) - PayPal payment
- `Check` (2) - Physical check
- `WireTransfer` (3) - International wire transfer
- `ACH` (4) - Automated Clearing House

### ApprovalStatus
- `PendingApproval` (0) - Awaiting approval
- `Approved` (1) - Approved by authorized user
- `Denied` (2) - Denied with reason

## Technical Details

- **Framework**: .NET 10 / C# 14
- **Database**: Entity Framework Core with SQL Server
- **Logging**: Serilog integration throughout
- **Error Handling**: Comprehensive try-catch with appropriate HTTP status codes
- **Validation**: ModelState validation on all Create/Update operations
- **Documentation**: XML documentation and Swagger/OpenAPI support
- **Multi-Currency**: Full exchange rate snapshot support
- **Audit Trail**: CreatedAt/UpdatedAt timestamps on all entities

## Files Created

**Total: 51 files**

### Database Layer (14 files)
- **Entities**: `CustomerTaxProfile.cs`, `VendorTaxProfile.cs`, `VendorCost.cs`, `VendorPayout.cs`, `RefundLossAudit.cs`
- **Enums**: `TaxIdType.cs`, `CustomerType.cs`, `VendorType.cs`, `VendorCostStatus.cs`, `RefundPolicy.cs`, `VendorPayoutStatus.cs`, `PayoutMethod.cs`, `ApprovalStatus.cs`, `InvoiceLineType.cs`

### Service Layer (10 files)
- **Interfaces**: `ICustomerTaxProfileService.cs`, `IVendorCostService.cs`, `IVendorPayoutService.cs`, `IVendorTaxProfileService.cs`, `IRefundLossAuditService.cs`
- **Implementations**: `CustomerTaxProfileService.cs`, `VendorCostService.cs`, `VendorPayoutService.cs`, `VendorTaxProfileService.cs`, `RefundLossAuditService.cs`

### API Layer (5 files)
- **Controllers**: `CustomerTaxProfilesController.cs`, `VendorCostsController.cs`, `VendorPayoutsController.cs`, `VendorTaxProfilesController.cs`, `RefundLossAuditsController.cs`

### DTO Layer (22 files)
- **Read DTOs**: `CustomerTaxProfileDto.cs`, `VendorCostDto.cs`, `VendorPayoutDto.cs`, `VendorTaxProfileDto.cs`, `RefundLossAuditDto.cs`
- **Create DTOs**: `CreateCustomerTaxProfileDto.cs`, `CreateVendorCostDto.cs`, `CreateVendorPayoutDto.cs`, `CreateVendorTaxProfileDto.cs`, `CreateRefundLossAuditDto.cs`
- **Update DTOs**: `UpdateCustomerTaxProfileDto.cs`, `UpdateVendorCostDto.cs`, `UpdateVendorPayoutDto.cs`, `UpdateVendorTaxProfileDto.cs`
- **Specialized DTOs**: `ApproveRefundLossDto.cs`, `ProcessVendorPayoutDto.cs`, `ResolvePayoutInterventionDto.cs`, `ValidateTaxIdDto.cs`, `TaxIdValidationResultDto.cs`, `VendorCostSummaryDto.cs`, `VendorPayoutSummaryDto.cs`, (1 more)

## Future Enhancements

1. **Tax ID Validation Integration** - Connect to external tax validation services (VIES, IRS)
2. **Automated Payout Scheduling** - Background service for scheduled vendor payouts
3. **Payment Gateway Integration** - Direct integration with payment processors
4. **Reporting Dashboard** - Visual analytics for costs, margins, and losses
5. **Batch Operations** - Bulk vendor cost creation and payout processing
6. **Notifications** - Email alerts for failed payouts and pending approvals
7. **1099 Form Generation** - Automated tax form creation for US vendors
8. **Multi-Vendor Reconciliation** - Automated matching of vendor invoices to costs

---

**Last Updated**: February 9, 2024  
**Version**: 1.0  
**Status**: Production Ready
