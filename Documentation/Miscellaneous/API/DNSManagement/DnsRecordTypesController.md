# DnsRecordTypes API Documentation

## Overview

The DnsRecordTypes API provides endpoints for managing DNS record types (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.). It allows administrators, support staff, and customers to retrieve DNS record type information.

## Base URL

```
https://api.example.com/api/v1/DnsRecordTypes
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Customer depending on the endpoint).

## Endpoints

### GET /api/v1/DnsRecordTypes

Retrieves all DNS record types in the system.

**Authorization:** Admin, Support, Customer

**Response:**
- 200: Returns the list of DNS record types
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "type": "A",
    "name": "A Record",
    "description": "IPv4 address record",
    "isActive": true,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/DnsRecordTypes/active

Retrieves only active DNS record types available for customer use.

**Authorization:** Admin, Support, Customer

**Response:**
- 200: Returns the list of active DNS record types
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/DnsRecordTypes/{id}

Retrieves a specific DNS record type by its unique identifier.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The unique identifier of the DNS record type

**Response:**
- 200: Returns the DNS record type data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If DNS record type is not found
- 500: If an internal server error occurs

### GET /api/v1/DnsRecordTypes/type/{type}

Retrieves a specific DNS record type by its type name (e.g., A, AAAA, CNAME, MX, TXT).

**Authorization:** Admin, Support, Customer

**Parameters:**
- `type` (path): The DNS record type name (e.g., "A", "CNAME", "MX")

**Response:**
- 200: Returns the DNS record type data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If DNS record type is not found
- 500: If an internal server error occurs

### POST /api/v1/DnsRecordTypes

Create a new DNS record type.

**Authorization:** Admin

**Request Body:**
```json
{
  "type": "SRV",
  "name": "SRV Record",
  "description": "Service locator record",
  "isActive": true
}
```

**Response:**
- 201: Returns the newly created DNS record type
- 400: If the DNS record type data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/DnsRecordTypes/{id}

Update an existing DNS record type.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the DNS record type to update

**Request Body:**
```json
{
  "type": "SRV",
  "name": "SRV Record",
  "description": "Updated service locator record",
  "isActive": true
}
```

**Response:**
- 200: Returns the updated DNS record type
- 400: If the DNS record type data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If DNS record type is not found
- 500: If an internal server error occurs

### DELETE /api/v1/DnsRecordTypes/{id}

Delete a DNS record type.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the DNS record type to delete

**Response:**
- 204: If DNS record type was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If DNS record type is not found
- 500: If an internal server error occurs

## Data Models

### DnsRecordTypeDto

```json
{
  "id": 0,
  "type": "string",
  "name": "string",
  "description": "string",
  "isActive": true,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateDnsRecordTypeDto

```json
{
  "type": "string",
  "name": "string",
  "description": "string",
  "isActive": true
}
```

### UpdateDnsRecordTypeDto

```json
{
  "type": "string",
  "name": "string",
  "description": "string",
  "isActive": true
}
```

## Common DNS Record Types

- **A**: IPv4 address record
- **AAAA**: IPv6 address record
- **CNAME**: Canonical name record
- **MX**: Mail exchange record
- **TXT**: Text record
- **NS**: Name server record
- **SRV**: Service locator record
- **PTR**: Pointer record

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

- **v1.0:** Initial release of DnsRecordTypes API