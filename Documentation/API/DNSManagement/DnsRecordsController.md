# DnsRecords API Documentation

## Overview

The DnsRecords API provides endpoints for managing DNS records for domains. It allows administrators, support staff, and customers to create, retrieve, update, and delete DNS records.

## Base URL

```
https://api.example.com/api/v1/DnsRecords
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Customer depending on the endpoint).

## Endpoints

### GET /api/v1/DnsRecords

Retrieves all DNS records in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of DNS records
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "domainId": 123,
    "name": "www",
    "type": "A",
    "value": "192.168.1.1",
    "ttl": 3600,
    "priority": null,
    "isEditableByUser": true,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/DnsRecords/{id}

Retrieves a specific DNS record by its unique identifier.

**Authorization:** Admin, Support, Customer

**Parameters:**
- `id` (path): The unique identifier of the DNS record

**Response:**
- 200: Returns the DNS record data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If DNS record is not found
- 500: If an internal server error occurs

### GET /api/v1/DnsRecords/domain/{domainId}

Retrieves all DNS records for a specific domain.

**Authorization:** Admin, Support, Customer

**Parameters:**
- `domainId` (path): The unique identifier of the domain

**Response:**
- 200: Returns the list of DNS records
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/DnsRecords/type/{type}

Retrieves all DNS records of a specific type (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.).

**Authorization:** Admin, Support

**Parameters:**
- `type` (path): The DNS record type (e.g., "A", "CNAME", "MX")

**Response:**
- 200: Returns the list of DNS records
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### POST /api/v1/DnsRecords

Creates a new DNS record for a domain.

**Authorization:** Admin, Support, Customer

**Request Body:**
```json
{
  "domainId": 123,
  "name": "www",
  "type": "A",
  "value": "192.168.1.1",
  "ttl": 3600,
  "priority": null
}
```

**Response:**
- 201: Returns the newly created DNS record
- 400: If the DNS record data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/DnsRecords/{id}

Update an existing DNS record.

**Authorization:** Admin, Support, Customer

**Parameters:**
- `id` (path): The unique identifier of the DNS record to update

**Request Body:**
```json
{
  "name": "www",
  "type": "A",
  "value": "192.168.1.2",
  "ttl": 3600,
  "priority": null
}
```

**Response:**
- 200: Returns the updated DNS record
- 400: If the DNS record data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If DNS record is not found
- 500: If an internal server error occurs

**Note:** System-managed DNS records (where `isEditableByUser` is false) can only be edited by Admin and Support users.

### DELETE /api/v1/DnsRecords/{id}

Delete a DNS record.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The unique identifier of the DNS record to delete

**Response:**
- 204: If DNS record was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If DNS record is not found
- 500: If an internal server error occurs

## Data Models

### DnsRecordDto

```json
{
  "id": 0,
  "domainId": 0,
  "name": "string",
  "type": "string",
  "value": "string",
  "ttl": 0,
  "priority": 0,
  "isEditableByUser": true,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateDnsRecordDto

```json
{
  "domainId": 0,
  "name": "string",
  "type": "string",
  "value": "string",
  "ttl": 0,
  "priority": 0
}
```

### UpdateDnsRecordDto

```json
{
  "name": "string",
  "type": "string",
  "value": "string",
  "ttl": 0,
  "priority": 0
}
```

## DNS Record Types

Common DNS record types supported:

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
- **403 Forbidden:** Insufficient permissions or attempting to edit system-managed records
- **404 Not Found:** Resource not found
- **500 Internal Server Error:** Server-side error

## Rate Limiting

API calls are subject to rate limiting. Please refer to the general API documentation for rate limit details.

## Changelog

- **v1.0:** Initial release of DnsRecords API