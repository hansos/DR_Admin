# DnsZonePackageRecords API Documentation

## Overview

The DnsZonePackageRecords API provides endpoints for managing individual DNS records within DNS zone packages. It allows administrators and support staff to create, retrieve, update, and delete DNS zone package records.

## Base URL

```
https://api.example.com/api/v1/DnsZonePackageRecords
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin or Support depending on the endpoint).

## Endpoints

### GET /api/v1/DnsZonePackageRecords

Retrieves all DNS zone package records in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of DNS zone package records
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "dnsZonePackageId": 123,
    "name": "@",
    "type": "A",
    "value": "192.168.1.1",
    "ttl": 3600,
    "priority": null,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/DnsZonePackageRecords/package/{packageId}

Retrieves DNS zone package records for a specific package.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `packageId` (path): The unique identifier of the DNS zone package

**Response:**
- 200: Returns the list of DNS zone package records
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/DnsZonePackageRecords/{id}

Retrieves a specific DNS zone package record by its unique identifier.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The unique identifier of the DNS zone package record

**Response:**
- 200: Returns the DNS zone package record data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If DNS zone package record is not found
- 500: If an internal server error occurs

### POST /api/v1/DnsZonePackageRecords

Creates a new DNS zone package record in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "dnsZonePackageId": 123,
  "name": "www",
  "type": "A",
  "value": "192.168.1.1",
  "ttl": 3600,
  "priority": null
}
```

**Response:**
- 201: Returns the newly created DNS zone package record
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/DnsZonePackageRecords/{id}

Updates an existing DNS zone package record's information.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the DNS zone package record to update

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
- 200: Returns the updated DNS zone package record
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If DNS zone package record is not found
- 500: If an internal server error occurs

### DELETE /api/v1/DnsZonePackageRecords/{id}

Deletes a DNS zone package record from the system.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the DNS zone package record to delete

**Response:**
- 204: If DNS zone package record was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If DNS zone package record is not found
- 500: If an internal server error occurs

## Data Models

### DnsZonePackageRecordDto

```json
{
  "id": 0,
  "dnsZonePackageId": 0,
  "name": "string",
  "type": "string",
  "value": "string",
  "ttl": 0,
  "priority": 0,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateDnsZonePackageRecordDto

```json
{
  "dnsZonePackageId": 0,
  "name": "string",
  "type": "string",
  "value": "string",
  "ttl": 0,
  "priority": 0
}
```

### UpdateDnsZonePackageRecordDto

```json
{
  "name": "string",
  "type": "string",
  "value": "string",
  "ttl": 0,
  "priority": 0
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

- **v1.0:** Initial release of DnsZonePackageRecords API