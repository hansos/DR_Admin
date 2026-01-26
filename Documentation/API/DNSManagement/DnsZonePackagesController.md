# DnsZonePackages API Documentation

## Overview

The DnsZonePackages API provides endpoints for managing DNS zone packages that contain predefined sets of DNS records. It allows administrators, support staff, and sales personnel to create, retrieve, update, and apply DNS zone packages to domains.

## Base URL

```
https://api.example.com/api/v1/DnsZonePackages
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Sales depending on the endpoint).

## Endpoints

### GET /api/v1/DnsZonePackages

Retrieves all DNS zone packages in the system.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of DNS zone packages
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "Basic DNS Package",
    "description": "Basic DNS records for a domain",
    "isActive": true,
    "isDefault": false,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/DnsZonePackages/with-records

Retrieves all DNS zone packages with their records.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of DNS zone packages with records
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/DnsZonePackages/active

Retrieves only active DNS zone packages.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of active DNS zone packages
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/DnsZonePackages/default

Retrieves the default DNS zone package.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the default DNS zone package with its records
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If no default package is configured
- 500: If an internal server error occurs

### GET /api/v1/DnsZonePackages/{id}

Retrieves a specific DNS zone package by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the DNS zone package

**Response:**
- 200: Returns the DNS zone package data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If DNS zone package is not found
- 500: If an internal server error occurs

### GET /api/v1/DnsZonePackages/{id}/with-records

Retrieves a specific DNS zone package with its records by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the DNS zone package

**Response:**
- 200: Returns the DNS zone package data with records
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If DNS zone package is not found
- 500: If an internal server error occurs

### POST /api/v1/DnsZonePackages

Creates a new DNS zone package in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "Advanced DNS Package",
  "description": "Advanced DNS records including mail and security",
  "isActive": true,
  "isDefault": false,
  "records": [
    {
      "name": "@",
      "type": "A",
      "value": "192.168.1.1",
      "ttl": 3600
    }
  ]
}
```

**Response:**
- 201: Returns the newly created DNS zone package
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/DnsZonePackages/{id}

Updates an existing DNS zone package's information.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the DNS zone package to update

**Request Body:**
```json
{
  "name": "Updated DNS Package",
  "description": "Updated description",
  "isActive": true,
  "isDefault": false
}
```

**Response:**
- 200: Returns the updated DNS zone package
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If DNS zone package is not found
- 500: If an internal server error occurs

### DELETE /api/v1/DnsZonePackages/{id}

Deletes a DNS zone package from the system.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the DNS zone package to delete

**Response:**
- 204: If DNS zone package was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If DNS zone package is not found
- 500: If an internal server error occurs

### POST /api/v1/DnsZonePackages/{packageId}/apply-to-domain/{domainId}

Applies a DNS zone package to a domain by creating DNS records.

**Authorization:** Admin, Support

**Parameters:**
- `packageId` (path): The unique identifier of the DNS zone package
- `domainId` (path): The unique identifier of the domain

**Response:**
- 200: If the package was successfully applied
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If DNS zone package or domain is not found
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "success": true,
  "message": "DNS zone package successfully applied to domain"
}
```

## Data Models

### DnsZonePackageDto

```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "isActive": true,
  "isDefault": false,
  "records": [
    {
      "id": 0,
      "name": "string",
      "type": "string",
      "value": "string",
      "ttl": 0,
      "priority": 0
    }
  ],
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateDnsZonePackageDto

```json
{
  "name": "string",
  "description": "string",
  "isActive": true,
  "isDefault": false,
  "records": [
    {
      "name": "string",
      "type": "string",
      "value": "string",
      "ttl": 0,
      "priority": 0
    }
  ]
}
```

### UpdateDnsZonePackageDto

```json
{
  "name": "string",
  "description": "string",
  "isActive": true,
  "isDefault": false
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

- **v1.0:** Initial release of DnsZonePackages API