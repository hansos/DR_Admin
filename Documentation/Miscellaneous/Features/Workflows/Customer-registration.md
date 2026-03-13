# Customer Registration Workflow

## Overview

This document describes the process of adding a new customer to the DR_Admin system database. The customer registration process is handled through a RESTful API endpoint that creates customer records with comprehensive contact, billing, and account management information.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Customer Entity Structure](#customer-entity-structure)
3. [Registration Process](#registration-process)
4. [API Endpoint](#api-endpoint)
5. [Request Format](#request-format)
6. [Response Format](#response-format)
7. [Validation Rules](#validation-rules)
8. [Authorization and Security](#authorization-and-security)
9. [Code Examples](#code-examples)
10. [Error Handling](#error-handling)
11. [Related Workflows](#related-workflows)

---

## Prerequisites

Before creating a new customer, ensure the following:

- **Authentication**: Valid JWT bearer token with appropriate permissions
- **Authorization**: User must have either `Admin` or `Sales` role
- **Database**: Application database is accessible and migrations are up to date
- **Required Data**: Minimum required customer information (name, email, phone, address details)

---

## Customer Entity Structure

The `Customer` entity in the database contains the following key properties:

### Core Information
- **Name** (required): Customer's full name
- **Email** (required): Primary email address
- **Phone** (required): Contact phone number
- **CustomerName** (optional): Company or individual name
- **ContactPerson** (optional): Primary contact person's name

### Address Information
- **Address** (required): Street address
- **City** (required): City name
- **State** (required): State or province
- **PostalCode** (required): ZIP or postal code
- **CountryCode** (optional): ISO country code (e.g., "US", "UK")

### Business Information
- **IsCompany**: Boolean flag indicating if customer is a business
- **TaxId**: Tax identification number
- **VatNumber**: VAT registration number

### Account Management
- **IsActive**: Account active status (default: true)
- **Status**: Customer status (default: "Active")
- **Balance**: Account balance (set to 0 on creation)
- **CreditLimit**: Maximum credit allowed

### Billing & Payment
- **BillingEmail**: Separate billing email if different from primary
- **PreferredPaymentMethod**: Customer's preferred payment method
- **PreferredCurrency**: ISO 4217 currency code (default: "EUR")
- **AllowCurrencyOverride**: Allow transaction-level currency override (default: true)

### Additional Information
- **Notes**: Internal notes about the customer
- **CreatedAt**: Timestamp of record creation
- **UpdatedAt**: Timestamp of last update

---

## Registration Process

### Workflow Steps

1. **User Authentication**
   - User authenticates and receives a JWT token
   - Token must contain `Admin` or `Sales` role

2. **Data Collection**
   - Gather all required customer information
   - Validate data format and completeness on client-side

3. **API Request**
   - Send POST request to `/api/v1/Customers` endpoint
   - Include customer data in request body as JSON
   - Include authentication token in Authorization header

4. **Server-Side Processing**
   - **Authorization Check**: Verify user has `Customer.Write` policy
   - **Model Validation**: Validate incoming data against model requirements
   - **Entity Creation**: Create new Customer entity with provided data
   - **Default Values**: Set automatic fields (Balance = 0, timestamps)
   - **Database Save**: Persist customer to database
   - **Response**: Return created customer with generated ID

5. **Post-Creation**
   - Customer record is available in the system
   - Customer ID can be used for related entities (invoices, orders, domains, etc.)
   - Audit logs capture the creation event

---

## API Endpoint

### Create Customer

**Endpoint:** `POST /api/v1/Customers`

**Authorization Required:** Yes (Bearer Token)

**Required Policy:** `Customer.Write`

**Allowed Roles:** `Admin`, `Sales`

---

## Request Format

### HTTP Headers

```http
POST /api/v1/Customers HTTP/1.1
Host: your-api-host.com
Authorization: Bearer {jwt-token}
Content-Type: application/json
```

### Request Body (CreateCustomerDto)

```json
{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "phone": "+1-555-0123",
  "address": "123 Main Street",
  "city": "Springfield",
  "state": "IL",
  "postalCode": "62701",
  "countryCode": "US",
  "customerName": "Doe Enterprises LLC",
  "taxId": "12-3456789",
  "vatNumber": "GB123456789",
  "contactPerson": "John Doe",
  "isCompany": true,
  "isActive": true,
  "status": "Active",
  "creditLimit": 5000.00,
  "notes": "VIP customer, handle with priority",
  "billingEmail": "billing@doeenterprises.com",
  "preferredPaymentMethod": "CreditCard",
  "preferredCurrency": "USD",
  "allowCurrencyOverride": true
}
```

### Minimal Request Example

```json
{
  "name": "Jane Smith",
  "email": "jane.smith@example.com",
  "phone": "+1-555-0456",
  "address": "456 Oak Avenue",
  "city": "Chicago",
  "state": "IL",
  "postalCode": "60601"
}
```

---

## Response Format

### Success Response (201 Created)

```json
{
  "id": 123,
  "name": "John Doe",
  "email": "john.doe@example.com",
  "phone": "+1-555-0123",
  "address": "123 Main Street",
  "city": "Springfield",
  "state": "IL",
  "postalCode": "62701",
  "countryCode": "US",
  "customerName": "Doe Enterprises LLC",
  "taxId": "12-3456789",
  "vatNumber": "GB123456789",
  "contactPerson": "John Doe",
  "isCompany": true,
  "isActive": true,
  "status": "Active",
  "balance": 0.00,
  "creditLimit": 5000.00,
  "notes": "VIP customer, handle with priority",
  "billingEmail": "billing@doeenterprises.com",
  "preferredPaymentMethod": "CreditCard",
  "preferredCurrency": "USD",
  "allowCurrencyOverride": true,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

### Response Headers

```http
HTTP/1.1 201 Created
Location: /api/v1/Customers/123
Content-Type: application/json
```

---

## Validation Rules

### Required Fields

The following fields are mandatory in the request:
- `name` - Customer's name (non-empty string)
- `email` - Valid email address format
- `phone` - Phone number (non-empty string)
- `address` - Street address (non-empty string)
- `city` - City name (non-empty string)
- `state` - State/province (non-empty string)
- `postalCode` - ZIP/postal code (non-empty string)

### Optional Fields with Defaults

- `isActive` - Defaults to `true`
- `status` - Defaults to `"Active"`
- `preferredCurrency` - Defaults to `"EUR"`
- `allowCurrencyOverride` - Defaults to `true`
- `balance` - Automatically set to `0` on creation

### Business Rules

1. **Email Uniqueness**: While not enforced at the API level shown, consider implementing email uniqueness validation
2. **Credit Limit**: Can be set to any decimal value; defaults to `0` if not specified
3. **Company Flag**: When `isCompany` is `true`, consider requiring `customerName` and `taxId`
4. **Currency Codes**: Should use ISO 4217 standard codes (e.g., "USD", "EUR", "GBP")
5. **Country Codes**: Should use ISO 3166-1 alpha-2 codes (e.g., "US", "GB", "DE")

---

## Authorization and Security

### Authentication

- All requests must include a valid JWT bearer token in the Authorization header
- Token format: `Authorization: Bearer {token}`

### Authorization Policies

**Customer.Write Policy**
- Required for creating customers
- Granted to users with `Admin` or `Sales` roles

**Role-Based Access:**
- ? **Admin**: Full access - can create customers
- ? **Sales**: Can create customers
- ? **Support**: Read-only access - cannot create customers
- ? **Customer**: Cannot access customer management endpoints

### Security Considerations

1. **Input Validation**: All input is validated server-side through ModelState
2. **SQL Injection Protection**: Entity Framework provides parameterized queries
3. **Audit Logging**: All customer creation events are logged with user information
4. **HTTPS**: Always use HTTPS in production to protect sensitive customer data
5. **Data Privacy**: Ensure compliance with GDPR, CCPA, and other data protection regulations

---

## Code Examples

### C# Client Example

```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ISPAdmin.DTOs;

public async Task<CustomerDto> CreateCustomerAsync(string bearerToken)
{
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://your-api-host.com");
    httpClient.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", bearerToken);

    var createDto = new CreateCustomerDto
    {
        Name = "John Doe",
        Email = "john.doe@example.com",
        Phone = "+1-555-0123",
        Address = "123 Main Street",
        City = "Springfield",
        State = "IL",
        PostalCode = "62701",
        CountryCode = "US",
        IsCompany = false,
        IsActive = true,
        CreditLimit = 1000.00m
    };

    var response = await httpClient.PostAsJsonAsync(
        "/api/v1/Customers", 
        createDto);

    response.EnsureSuccessStatusCode();

    var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
    return customer;
}
```

### JavaScript/TypeScript Example

```javascript
async function createCustomer(bearerToken) {
    const customerData = {
        name: "John Doe",
        email: "john.doe@example.com",
        phone: "+1-555-0123",
        address: "123 Main Street",
        city: "Springfield",
        state: "IL",
        postalCode: "62701",
        countryCode: "US",
        isCompany: false,
        isActive: true,
        creditLimit: 1000.00
    };

    const response = await fetch('https://your-api-host.com/api/v1/Customers', {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${bearerToken}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(customerData)
    });

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }

    const customer = await response.json();
    console.log('Created customer with ID:', customer.id);
    return customer;
}
```

### cURL Example

```bash
curl -X POST https://your-api-host.com/api/v1/Customers \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john.doe@example.com",
    "phone": "+1-555-0123",
    "address": "123 Main Street",
    "city": "Springfield",
    "state": "IL",
    "postalCode": "62701",
    "countryCode": "US",
    "isCompany": false,
    "isActive": true,
    "creditLimit": 1000.00
  }'
```

---

## Error Handling

### Common HTTP Status Codes

| Status Code | Meaning | Common Causes |
|-------------|---------|---------------|
| 201 Created | Success | Customer created successfully |
| 400 Bad Request | Invalid Data | Missing required fields, invalid format, validation errors |
| 401 Unauthorized | Authentication Failed | Missing or invalid JWT token |
| 403 Forbidden | Authorization Failed | User lacks required role (Admin or Sales) |
| 500 Internal Server Error | Server Error | Database connection issues, unexpected errors |

### Error Response Format

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": ["The Email field is required."],
    "Name": ["The Name field is required."]
  }
}
```

### Handling Validation Errors

```csharp
try
{
    var response = await httpClient.PostAsJsonAsync("/api/v1/Customers", createDto);
    
    if (!response.IsSuccessStatusCode)
    {
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Validation errors: {errorContent}");
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("Authentication required. Please log in.");
        }
        else if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            Console.WriteLine("You don't have permission to create customers.");
        }
    }
    
    response.EnsureSuccessStatusCode();
    var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
    return customer;
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Request error: {ex.Message}");
    throw;
}
```

---

## Related Workflows

After successfully creating a customer, you may want to:

1. **Create User Account** - Associate a user login with the customer
2. **Add Credit** - Add initial account credit or set up payment method
3. **Create Order** - Place initial orders for domains, hosting, etc.
4. **Generate Invoice** - Create invoices for services
5. **Manage Domains** - Register or transfer domains for the customer
6. **Setup Hosting** - Configure hosting accounts

### Integration Points

The Customer entity serves as a central reference for:
- **Users**: Multiple users can be associated with one customer
- **Invoices**: All billing documents are linked to customers
- **Orders**: Service orders reference the customer
- **Domains**: Domain registrations track customer ownership
- **Hosting Accounts**: Web hosting services are customer-specific
- **Support Tickets**: Customer support requests (if implemented)
- **Payment Transactions**: All financial transactions reference the customer

---

## Service Layer Implementation

The customer creation logic is implemented in the `CustomerService` class:

### Key Components

**Controller**: `CustomersController.CreateCustomer()`
- Handles HTTP request
- Validates ModelState
- Calls service layer
- Returns appropriate HTTP response

**Service**: `CustomerService.CreateCustomerAsync()`
- Creates Customer entity from DTO
- Sets default values (Balance = 0, timestamps)
- Saves to database via EF Core
- Returns CustomerDto

**Data Access**: Entity Framework Core
- `ApplicationDbContext` manages database connection
- Change tracking and SaveChangesAsync() persist data
- Automatic transaction management

---

## Best Practices

1. **Data Validation**: Always validate customer data on both client and server side
2. **Error Handling**: Implement proper try-catch blocks and log errors
3. **Logging**: Use structured logging (Serilog) to track customer creation events
4. **Idempotency**: Consider implementing duplicate detection for email addresses
5. **Async Operations**: Use async/await for database operations to improve scalability
6. **DTO Pattern**: Always use DTOs to decouple API contracts from database entities
7. **Security**: Never expose internal entity structures in API responses
8. **Audit Trail**: Maintain CreatedAt/UpdatedAt timestamps for compliance
9. **Testing**: Write integration tests to verify the complete workflow
10. **Documentation**: Keep API documentation in sync with code changes

---

## Troubleshooting

### Common Issues and Solutions

**Problem**: 401 Unauthorized Error
- **Solution**: Ensure JWT token is valid and not expired
- **Check**: Token is included in Authorization header with "Bearer " prefix

**Problem**: 403 Forbidden Error
- **Solution**: Verify user has Admin or Sales role
- **Check**: User permissions and role assignments

**Problem**: 400 Bad Request - Validation Errors
- **Solution**: Review required fields and data formats
- **Check**: All required fields are provided and properly formatted

**Problem**: 500 Internal Server Error
- **Solution**: Check application logs for detailed error information
- **Check**: Database connectivity, migration status, and server health

**Problem**: Customer Created But Not Visible
- **Solution**: Check database transaction commit
- **Check**: Verify no authorization filters blocking retrieval

---

## Additional Resources

- **API Documentation**: Swagger UI at `/swagger` endpoint
- **Customer Management**: See `Documentation/Features/CustomerManagement/Customers.md`
- **Authentication Guide**: Refer to authentication documentation
- **Database Migrations**: Check migration files for schema details
- **Integration Tests**: See `DR_Admin.IntegrationTests/Controllers/CustomersControllerTests.cs`

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-02-04 | Initial documentation created |

---

## Contact and Support

For questions or issues regarding customer registration:
- Review application logs in `/logs` directory
- Check Swagger documentation at `/swagger`
- Consult integration tests for working examples
- Contact development team for additional support
