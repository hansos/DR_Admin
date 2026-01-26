# TaxRules API Documentation

## Overview

Manages tax rules for automatic tax calculation

## Base URL

```
https://api.example.com/api/v1/TaxRules
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin depending on the endpoint).

## Endpoints

### GET /api/v1/TaxRules

Retrieves all tax rules in the system.

**Authorization:** Admin

**Response:**
- 200: Returns the list of tax rules
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "countryCode": "DE",
    "stateCode": null,
    "taxName": "VAT",
    "taxRate": 19.0,
    "isActive": true,
    "effectiveFrom": "2023-01-01T00:00:00Z",
    "effectiveUntil": null,
    "appliesToSetupFees": true,
    "appliesToRecurring": true,
    "reverseCharge": false,
    "taxAuthority": "German Tax Authority",
    "taxRegistrationNumber": "DE123456789",
    "priority": 1,
    "createdAt": "2023-01-01T00:00:00Z",
    "updatedAt": "2023-01-01T00:00:00Z"
  }
]
```

### GET /api/v1/TaxRules/active

Retrieves all active tax rules.

**Authorization:** Admin

**Response:**
- 200: Returns the list of active tax rules
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/TaxRules/{id}

Retrieves a specific tax rule by ID.

**Authorization:** Admin

**Parameters:**
- `id` (path): The tax rule ID

**Response:**
- 200: Returns the tax rule
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If the tax rule is not found
- 500: If an internal server error occurs

### GET /api/v1/TaxRules/location/{countryCode}

Retrieves tax rules by location.

**Authorization:** None (authenticated users)

**Parameters:**
- `countryCode` (path): The country code
- `stateCode` (query, optional): The state code

**Response:**
- 200: Returns the list of tax rules
- 401: If user is not authenticated
- 500: If an internal server error occurs

### POST /api/v1/TaxRules

Creates a new tax rule.

**Authorization:** Admin

**Request Body:**
```json
{
  "countryCode": "DE",
  "stateCode": null,
  "taxName": "VAT",
  "taxRate": 19.0,
  "isActive": true,
  "effectiveFrom": "2023-01-01T00:00:00Z",
  "effectiveUntil": null,
  "appliesToSetupFees": true,
  "appliesToRecurring": true,
  "reverseCharge": false,
  "taxAuthority": "German Tax Authority",
  "taxRegistrationNumber": "DE123456789",
  "priority": 1
}
```

**Response:**
- 201: Returns the newly created tax rule
- 400: If the tax rule data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/TaxRules/{id}

Updates an existing tax rule.

**Authorization:** Admin

**Parameters:**
- `id` (path): The tax rule ID

**Request Body:**
```json
{
  "taxRate": 20.0,
  "isActive": true
}
```

**Response:**
- 200: Returns the updated tax rule
- 400: If the tax rule data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If the tax rule is not found
- 500: If an internal server error occurs

### DELETE /api/v1/TaxRules/{id}

Deletes a tax rule (soft delete).

**Authorization:** Admin

**Parameters:**
- `id` (path): The tax rule ID

**Response:**
- 204: If the tax rule was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If the tax rule is not found
- 500: If an internal server error occurs

### GET /api/v1/TaxRules/calculate

Calculates tax for a customer and amount.

**Authorization:** None (authenticated users)

**Parameters:**
- `customerId` (query): The customer ID
- `amount` (query): The taxable amount
- `isSetupFee` (query, optional): Whether this is a setup fee

**Response:**
- 200: Returns the tax calculation
- 401: If user is not authenticated
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "taxAmount": 3.8,
  "taxRate": 19.0,
  "taxName": "VAT"
}
```

### POST /api/v1/TaxRules/validate-vat

Validates a VAT number (EU VIES check).

**Authorization:** None (authenticated users)

**Parameters:**
- `vatNumber` (query): The VAT number to validate
- `countryCode` (query): The country code

**Response:**
- 200: Returns the validation result
- 401: If user is not authenticated
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "isValid": true,
  "vatNumber": "DE123456789",
  "countryCode": "DE"
}
```

[Back to Controllers Index](index.md)