# ResellerCompanies API Documentation

## Overview

The ResellerCompanies API provides endpoints for managing reseller companies. It allows administrators to create, retrieve, update, and delete reseller company records.

## Base URL

```
https://api.example.com/api/v1/ResellerCompanies
```

## Authentication

All endpoints require authentication using JWT tokens. Most endpoints require Admin role.

## Endpoints

### GET /api/v1/ResellerCompanies

Retrieves all reseller companies in the system.

**Authorization:** Admin

**Response:**
- 200: Returns the list of reseller companies
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "ABC Resellers Inc.",
    "contactEmail": "contact@abcresellers.com",
    "contactPhone": "+1-555-0123",
    "address": "123 Business St, City, State 12345",
    "isActive": true,
    "commissionRate": 0.15,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/ResellerCompanies/active

Retrieves only active reseller companies.

**Authorization:** Admin, Sales

**Response:**
- 200: Returns the list of active reseller companies
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/ResellerCompanies/{id}

Retrieves a specific reseller company by ID.

**Authorization:** Admin

**Parameters:**
- `id` (path): The reseller company ID

**Response:**
- 200: Returns the reseller company
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If reseller company is not found
- 500: If an internal server error occurs

### POST /api/v1/ResellerCompanies

Creates a new reseller company.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "XYZ Resellers LLC",
  "contactEmail": "info@xyzresellers.com",
  "contactPhone": "+1-555-0456",
  "address": "456 Commerce Ave, City, State 67890",
  "isActive": true,
  "commissionRate": 0.12
}
```

**Response:**
- 201: Returns the newly created reseller company
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/ResellerCompanies/{id}

Updates an existing reseller company.

**Authorization:** Admin

**Parameters:**
- `id` (path): The reseller company ID to update

**Request Body:**
```json
{
  "name": "XYZ Resellers LLC Updated",
  "contactEmail": "support@xyzresellers.com",
  "contactPhone": "+1-555-0456",
  "address": "456 Commerce Ave, City, State 67890",
  "isActive": true,
  "commissionRate": 0.13
}
```

**Response:**
- 200: Returns the updated reseller company
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If reseller company is not found
- 500: If an internal server error occurs

### DELETE /api/v1/ResellerCompanies/{id}

Deletes a reseller company.

**Authorization:** Admin

**Parameters:**
- `id` (path): The reseller company ID to delete

**Response:**
- 204: If the reseller company was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If reseller company is not found
- 500: If an internal server error occurs

## Data Models

### ResellerCompanyDto

```json
{
  "id": 0,
  "name": "string",
  "contactEmail": "string",
  "contactPhone": "string",
  "address": "string",
  "isActive": true,
  "commissionRate": 0.0,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateResellerCompanyDto

```json
{
  "name": "string",
  "contactEmail": "string",
  "contactPhone": "string",
  "address": "string",
  "isActive": true,
  "commissionRate": 0.0
}
```

### UpdateResellerCompanyDto

```json
{
  "name": "string",
  "contactEmail": "string",
  "contactPhone": "string",
  "address": "string",
  "isActive": true,
  "commissionRate": 0.0
}
```

## Error Handling

All endpoints return appropriate HTTP status codes and error messages in case of failures. Common error responses include:

- **400 Bad Request:** Invalid request data or parameters
- **401 Unauthorized:** Missing or invalid authentication
- **403 Forbidden:** Insufficient permissions
- **404 Not Found:** Resource not found
- **500 Internal Server Error:** Server-side error

## Rate Limiting

API calls are subject to rate limiting. Please refer to the general API documentation for rate limit details.

## Changelog

- **v1.0:** Initial release of ResellerCompanies API