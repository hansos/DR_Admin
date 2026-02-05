# Domain Registration Implementation Summary

## Overview
Implemented customer self-service and sales-assisted domain registration functionality for the DR_Admin system. The implementation integrates with your existing DomainRegistrationWorkflow and provides both customer-facing and administrative endpoints.

---

## Files Created

### 1. DTOs (Data Transfer Objects)
**File**: `DR_Admin/DTOs/DomainRegistrationDto.cs`
- `RegisterDomainDto` - Customer self-service registration
- `RegisterDomainForCustomerDto` - Sales/Admin registration for customers
- `DomainRegistrationResponseDto` - Registration response with order/invoice details
- `CheckDomainAvailabilityDto` - Request for availability check
- `DomainAvailabilityResponseDto` - Availability check response
- `DomainPricingDto` - TLD pricing information
- `AvailableTldDto` - Available TLD listing

### 2. Settings Configuration
**File**: `DR_Admin/Infrastructure/Settings/DomainRegistrationSettings.cs`
- Configuration class for domain registration behavior
- Controls approval workflows, registrar selection, pricing, etc.

---

## Files Modified

### 1. Authorization Policies
**File**: `DR_Admin/Infrastructure/AuthorizationPoliciesConfiguration.cs`
Added three new policies:
- `Domain.Register` - Allows CUSTOMER, SALES, ADMIN to register domains
- `Domain.RegisterForCustomer` - SALES, ADMIN only (register for specific customer)
- `Domain.CheckAvailability` - CUSTOMER, SALES, ADMIN can check availability

### 2. Application Settings
**File**: `DR_Admin/appsettings.json`
Added `DomainRegistration` section with:
```json
{
  "RequireApprovalForCustomers": false,
  "RequireApprovalForSales": false,
  "DefaultRegistrarId": 1,
  "AllowCustomerRegistration": true,
  "MaxRegistrationYears": 10,
  "MinRegistrationYears": 1,
  "DefaultRegistrationYears": 1,
  "EnableAvailabilityCheck": false,
  "EnablePricingCheck": true
}
```

### 3. Service Interface
**File**: `DR_Admin/Services/IDomainService.cs`
Added methods:
- `RegisterDomainAsync(RegisterDomainDto, customerId)` - Customer registration
- `RegisterDomainForCustomerAsync(RegisterDomainForCustomerDto)` - Admin/Sales registration
- `CheckDomainAvailabilityAsync(domainName)` - Check availability
- `GetDomainPricingAsync(tld, registrarId?)` - Get TLD pricing
- `GetAvailableTldsAsync()` - List all available TLDs

### 4. Service Implementation
**File**: `DR_Admin/Services/DomainService.cs`
- Updated constructor to inject `IDomainRegistrationWorkflow` and settings
- Implemented all new methods
- Integrated with existing DomainRegistrationWorkflow
- Added validation logic for customers, registrars, years
- Queries RegistrarTld table for pricing information

### 5. Controller
**File**: `DR_Admin/Controllers/DomainsController.cs`
Added endpoints:
- `POST /api/v1/domains/register` - Customer self-service registration
- `POST /api/v1/domains/register-for-customer` - Sales/Admin registration
- `POST /api/v1/domains/check-availability` - Check domain availability
- `GET /api/v1/domains/pricing/{tld}` - Get TLD pricing
- `GET /api/v1/domains/available-tlds` - List available TLDs

### 6. Dependency Injection
**File**: `DR_Admin/Program.cs`
- Registered `DomainRegistrationSettings` from configuration

---

## How It Works

### Customer Self-Service Flow
1. **Customer** calls `POST /api/v1/domains/register` with domain details
2. System extracts `CustomerId` from JWT claims
3. Validates customer exists and settings allow customer registration
4. Uses configured `DefaultRegistrarId` from settings
5. Creates `DomainRegistrationWorkflowInput` and executes workflow
6. Workflow creates Order ? Invoice ? (optionally registers with external registrar)
7. Returns `DomainRegistrationResponseDto` with order/invoice info

### Sales/Admin Assisted Flow
1. **Sales/Admin** calls `POST /api/v1/domains/register-for-customer` with customer ID and domain
2. System validates customer and specified registrar
3. Executes same workflow as customer flow
4. Returns registration response

### Pricing & Availability
- `GET /api/v1/domains/pricing/com` - Get pricing for .com domains
- `GET /api/v1/domains/available-tlds` - List all TLDs with pricing
- `POST /api/v1/domains/check-availability` - Check if domain exists in your system

---

## Configuration Options

| Setting | Description | Default |
|---------|-------------|---------|
| `RequireApprovalForCustomers` | Customer registrations need approval | `false` |
| `RequireApprovalForSales` | Sales registrations need approval | `false` |
| `DefaultRegistrarId` | Registrar ID for customer registrations | `1` |
| `AllowCustomerRegistration` | Enable/disable customer self-service | `true` |
| `MaxRegistrationYears` | Maximum registration period | `10` |
| `MinRegistrationYears` | Minimum registration period | `1` |
| `EnableAvailabilityCheck` | Check with external registrar | `false` |
| `EnablePricingCheck` | Fetch real-time pricing | `true` |

---

## Authorization Matrix

| Endpoint | Customer | Sales | Admin |
|----------|----------|-------|-------|
| `POST /domains/register` | ? | ? | ? |
| `POST /domains/register-for-customer` | ? | ? | ? |
| `POST /domains/check-availability` | ? | ? | ? |
| `GET /domains/pricing/{tld}` | ? | ? | ? |
| `GET /domains/available-tlds` | ? | ? | ? |

---

## Payment Integration

The implementation uses your existing workflow system:
1. **Order** is created with domain details
2. **Invoice** is generated automatically
3. **Payment** can be processed via existing payment methods
4. When payment is received, `DomainRegistrationWorkflow.OnPaymentReceivedAsync()` is called
5. Workflow then registers domain with external registrar (when configured)
6. **RegisteredDomain** entity is created in database

---

## Next Steps

1. **Configure Default Registrar**
   - Set `DomainRegistration:DefaultRegistrarId` in appsettings.json
   - Ensure registrar exists in database and is active

2. **Populate RegistrarTld Table**
   - Add TLD pricing for your registrars
   - Set `IsAvailable = true` for TLDs you want to offer

3. **Test Customer Registration**
   - Create test customer with JWT containing `CustomerId` claim
   - Call `/domains/register` endpoint

4. **Enable External Registrar (Optional)**
   - Configure registrar credentials in `RegistrarSettings` section
   - Set `EnableAvailabilityCheck = true` to check real availability
   - Uncomment registrar integration code in `DomainRegistrationWorkflow.cs`

5. **Customize Approval Workflow**
   - Set `RequireApprovalForCustomers = true` for manual approval
   - Implement approval UI in admin panel
   - Update order status after approval

---

## API Examples

### Customer Registration
```http
POST /api/v1/domains/register
Authorization: Bearer <customer_jwt_token>
Content-Type: application/json

{
  "domainName": "example.com",
  "years": 2,
  "autoRenew": true,
  "privacyProtection": false,
  "notes": "My new website domain"
}
```

### Sales Registration for Customer
```http
POST /api/v1/domains/register-for-customer
Authorization: Bearer <sales_jwt_token>
Content-Type: application/json

{
  "customerId": 123,
  "domainName": "customer-domain.com",
  "registrarId": 1,
  "years": 1,
  "autoRenew": true,
  "privacyProtection": true
}
```

### Check Availability
```http
POST /api/v1/domains/check-availability
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "domainName": "mynewdomain.com"
}
```

### Get Pricing
```http
GET /api/v1/domains/pricing/com?registrarId=1
Authorization: Bearer <jwt_token>
```

### List Available TLDs
```http
GET /api/v1/domains/available-tlds
Authorization: Bearer <jwt_token>
```

---

## Notes

- The implementation respects your existing workflow architecture
- Domain registration creates both an Order and a RegisteredDomain entity
- Service records are created automatically if not provided
- External registrar integration is ready but disabled by default
- All endpoints require authentication (JWT token)
- Customer role users can only register for themselves
- Sales/Admin can register for any customer and choose registrar

---

## Troubleshooting

**Issue**: "Customer domain registration is currently disabled"
- **Solution**: Set `AllowCustomerRegistration = true` in appsettings.json

**Issue**: "Registrar with ID X not found or inactive"
- **Solution**: Check `DefaultRegistrarId` setting and ensure registrar exists and `IsActive = true`

**Issue**: "Pricing not found for TLD"
- **Solution**: Add RegistrarTld record with pricing for that TLD

**Issue**: "Customer ID not found in authentication token"
- **Solution**: Ensure JWT token contains `CustomerId` claim for customer users
