# Customers API

This document describes how to use the `Customers` controller to manage customer records (CRUD) and related operations.

## Authentication

- All endpoints require a valid JWT in the `Authorization` header.
- Role restrictions are applied per-endpoint as implemented on the controller (see Roles in each endpoint section).

## High-level steps

1. List or retrieve customers
   - `GET /api/v1/Customers` (Admin, Support, Sales)
   - `GET /api/v1/Customers/{id}` (Admin, Support, Sales)
2. Create a customer (Admin or Sales)
   - `POST /api/v1/Customers` with `CreateCustomerDto` payload
3. Update a customer (Admin or Sales)
   - `PUT /api/v1/Customers/{id}` with `UpdateCustomerDto` payload
4. Delete a customer (Admin only)
   - `DELETE /api/v1/Customers/{id}`

## API usage

Base route: `api/v1/Customers`

### List customers
- `GET /api/v1/Customers`
- Roles: `Admin, Support, Sales`
- Response: `200 OK` with an array of `CustomerDto` objects

Curl example:

```bash
curl -H "Authorization: Bearer <JWT>" \
  https://your-host/api/v1/Customers
```

Example `200 OK` response JSON (array of `CustomerDto`):

```json
[
  {
    "id": 101,
    "name": "Acme Retail Ltd",
    "email": "accounts@acme-retail.com",
    "phone": "+44 20 7946 0958",
    "address": "10 High Street",
    "city": "London",
    "state": "",
    "postalCode": "SW1A 1AA",
    "countryCode": "GB",
    "companyName": "Acme Retail Ltd",
    "taxId": "GB123456789",
    "vatNumber": "GB999999973",
    "contactPerson": "Alice Johnson",
    "isCompany": true,
    "isActive": true,
    "status": "Active",
    "balance": 250.50,
    "creditLimit": 5000.00,
    "notes": "Priority customer, prefers email invoicing",
    "billingEmail": "billing@acme-retail.com",
    "preferredPaymentMethod": "bank_transfer",
    "preferredCurrency": "GBP",
    "allowCurrencyOverride": false,
    "createdAt": "2025-03-12T09:23:00Z",
    "updatedAt": "2025-04-01T14:02:00Z"
  },
  {
    "id": 102,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "phone": "+1 555-0100",
    "address": "742 Evergreen Terrace",
    "city": "Springfield",
    "state": "IL",
    "postalCode": "62704",
    "countryCode": "US",
    "companyName": null,
    "taxId": null,
    "vatNumber": null,
    "contactPerson": null,
    "isCompany": false,
    "isActive": true,
    "status": "Active",
    "balance": 0.00,
    "creditLimit": 0.00,
    "notes": null,
    "billingEmail": null,
    "preferredPaymentMethod": "card",
    "preferredCurrency": "USD",
    "allowCurrencyOverride": true,
    "createdAt": "2024-11-02T08:15:30Z",
    "updatedAt": "2025-01-20T10:45:00Z"
  }
]
```

### Get customer by id
- `GET /api/v1/Customers/{id}`
- Roles: `Admin, Support, Sales`

Responses:
- `200 OK` with `CustomerDto` if found
- `404 Not Found` if the customer does not exist

Curl example:

```bash
curl -H "Authorization: Bearer <JWT>" \
  https://your-host/api/v1/Customers/123
```

Example `200 OK` response JSON (`CustomerDto`):

```json
{
  "id": 123,
  "name": "John Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "address": "123 Main St",
  "city": "Anytown",
  "state": "State",
  "postalCode": "12345",
  "countryCode": "US",
  "companyName": "Acme Inc",
  "taxId": "TAX123",
  "vatNumber": "VAT123",
  "contactPerson": "Jane Admin",
  "isCompany": false,
  "isActive": true,
  "status": "Active",
  "balance": 0.0,
  "creditLimit": 1000.00,
  "notes": "Important customer",
  "billingEmail": "billing@example.com",
  "preferredPaymentMethod": "card",
  "preferredCurrency": "EUR",
  "allowCurrencyOverride": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

### Create customer
- `POST /api/v1/Customers`
- Roles: `Admin, Sales`
- Request body: `CreateCustomerDto` (JSON)

Example `CreateCustomerDto` JSON payload:

```json
{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "address": "123 Main St",
  "city": "Anytown",
  "state": "State",
  "postalCode": "12345",
  "countryCode": "US",
  "companyName": "Acme Inc",
  "taxId": "TAX123",
  "vatNumber": "VAT123",
  "contactPerson": "Jane Admin",
  "isCompany": false,
  "isActive": true,
  "status": "Active",
  "creditLimit": 1000.00,
  "notes": "Important customer",
  "billingEmail": "billing@example.com",
  "preferredPaymentMethod": "card",
  "preferredCurrency": "EUR",
  "allowCurrencyOverride": true
}
```

Curl example:

```bash
curl -X POST "https://your-host/api/v1/Customers" \
  -H "Authorization: Bearer <JWT>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john.doe@example.com",
    "phone": "+1234567890",
    "address": "123 Main St",
    "city": "Anytown",
    "state": "State",
    "postalCode": "12345",
    "countryCode": "US",
    "companyName": "Acme Inc",
    "taxId": "TAX123",
    "vatNumber": "VAT123",
    "contactPerson": "Jane Admin",
    "isCompany": false,
    "isActive": true,
    "status": "Active",
    "creditLimit": 1000.00,
    "notes": "Important customer",
    "billingEmail": "billing@example.com",
    "preferredPaymentMethod": "card",
    "preferredCurrency": "EUR",
    "allowCurrencyOverride": true
  }'
```

Successful response:
- `201 Created` with created `CustomerDto` and `Location` header referencing `GET /api/v1/Customers/{id}`

Example `CustomerDto` response JSON:

```json
{
  "id": 123,
  "name": "John Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "address": "123 Main St",
  "city": "Anytown",
  "state": "State",
  "postalCode": "12345",
  "countryCode": "US",
  "companyName": "Acme Inc",
  "taxId": "TAX123",
  "vatNumber": "VAT123",
  "contactPerson": "Jane Admin",
  "isCompany": false,
  "isActive": true,
  "status": "Active",
  "balance": 0.0,
  "creditLimit": 1000.00,
  "notes": "Important customer",
  "billingEmail": "billing@example.com",
  "preferredPaymentMethod": "card",
  "preferredCurrency": "EUR",
  "allowCurrencyOverride": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

### Update customer
- `PUT /api/v1/Customers/{id}`
- Roles: `Admin, Sales`
- Request body: `UpdateCustomerDto` (same fields as `CreateCustomerDto`)

Curl example:

```bash
curl -X PUT "https://your-host/api/v1/Customers/123" \
  -H "Authorization: Bearer <JWT>" \
  -H "Content-Type: application/json" \
  -d '{ "name": "John Doe Jr.", "email": "john.jr@example.com" }'
```

Responses:
- `200 OK` with updated `CustomerDto` if successful
- `400 Bad Request` if model validation fails
- `404 Not Found` if the customer does not exist

Example `200 OK` response JSON after update:

```json
{
  "id": 123,
  "name": "John Doe Jr.",
  "email": "john.jr@example.com",
  "phone": "+1234567890",
  "address": "123 Main St",
  "city": "Anytown",
  "state": "State",
  "postalCode": "12345",
  "countryCode": "US",
  "companyName": "Acme Inc",
  "taxId": "TAX123",
  "vatNumber": "VAT123",
  "contactPerson": "Jane Admin",
  "isCompany": false,
  "isActive": true,
  "status": "Active",
  "balance": 0.0,
  "creditLimit": 1000.00,
  "notes": "Important customer",
  "billingEmail": "billing@example.com",
  "preferredPaymentMethod": "card",
  "preferredCurrency": "EUR",
  "allowCurrencyOverride": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2025-04-05T12:30:00Z"
}
```

### Delete customer
- `DELETE /api/v1/Customers/{id}`
- Roles: `Admin`

Curl example:

```bash
curl -X DELETE "https://your-host/api/v1/Customers/123" \
  -H "Authorization: Bearer <JWT>"
```

Responses:
- `204 No Content` if deletion succeeded
- `404 Not Found` if the customer does not exist

Example `204 No Content` response (headers only):

```
HTTP/1.1 204 No Content
Date: Tue, 05 Apr 2025 12:35:00 GMT
```

## DTOs and main fields

- `CustomerDto` (response) includes these properties sent by the service:
  - `Id`, `Name`, `Email`, `Phone`, `Address`, `City`, `State`, `PostalCode`, `CountryCode`, `CompanyName`, `TaxId`, `VatNumber`, `ContactPerson`, `IsCompany`, `IsActive`, `Status`, `Balance`, `CreditLimit`, `Notes`, `BillingEmail`, `PreferredPaymentMethod`, `PreferredCurrency`, `AllowCurrencyOverride`, `CreatedAt`, `UpdatedAt`.

- `CreateCustomerDto` and `UpdateCustomerDto` contain the writable properties used for creating and updating customers. Note: `PreferredCurrency` defaults to `EUR` and `AllowCurrencyOverride` defaults to `true` in the DTOs.

## Service behavior (implementation notes)

- `CustomerService` executes data operations against `ApplicationDbContext` and maps `Customer` entities to `CustomerDto` using an internal `MapToDto`.
- Create initializes `Balance` to `0` and stamps `CreatedAt` and `UpdatedAt` with `DateTime.UtcNow`.
- Update copies writable fields to the entity and updates `UpdatedAt`.
- Delete removes the entity from the database.
- Read operations use `AsNoTracking` where appropriate to reduce EF Core tracking overhead.

## Error handling and status codes

- `401 Unauthorized` if no valid JWT is provided.
- `403 Forbidden` if the authenticated user lacks the required role for the endpoint.
- `400 Bad Request` for invalid input or model state errors.
- `404 Not Found` when the requested customer does not exist.
- `500 Internal Server Error` for unexpected failures (controller logs exceptions and returns `500`).

## Notes and best practices

- Always include the JWT in the `Authorization` header when calling these APIs.
- Validate emails and business identifiers before creating/updating customers.
- Prefer pagination for `GET /api/v1/Customers` in production to avoid large results.
- Consider soft-delete semantics if historical references must be preserved rather than permanent removal.
- Ensure idempotency for repeated identical update requests where appropriate.
- Keep DTO documentation updated if DTOs evolve � this document reflects the current DTOs in `DR_Admin/DTOs/CustomerDto.cs` and `CustomerService` behavior.

Refer to the `CustomersController` and `CustomerService` source files for exact request/response shapes, validation rules, and authorization details.
