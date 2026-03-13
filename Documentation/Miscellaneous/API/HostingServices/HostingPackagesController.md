# HostingPackages API Documentation

## Overview

The HostingPackages API provides endpoints for managing hosting packages. It allows administrators to create, retrieve, update, and delete hosting package records.

## Base URL

```
https://api.example.com/api/v1/HostingPackages
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/HostingPackages

Retrieves all hosting packages in the system.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of hosting packages
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "Basic Hosting",
    "description": "Basic web hosting package",
    "diskSpace": 5000,
    "bandwidth": 50000,
    "emailAccounts": 5,
    "databases": 2,
    "price": 9.99,
    "setupFee": 0.00,
    "isActive": true,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/HostingPackages/active

Retrieves only active hosting packages.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of active hosting packages
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/HostingPackages/{id}

Retrieves a specific hosting package by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the hosting package

**Response:**
- 200: Returns the hosting package data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If hosting package is not found
- 500: If an internal server error occurs

### POST /api/v1/HostingPackages

Creates a new hosting package in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "Premium Hosting",
  "description": "Premium web hosting package",
  "diskSpace": 10000,
  "bandwidth": 100000,
  "emailAccounts": 10,
  "databases": 5,
  "price": 19.99,
  "setupFee": 5.00,
  "isActive": true
}
```

**Response:**
- 201: Returns the newly created hosting package
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/HostingPackages/{id}

Updates an existing hosting package's information.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the hosting package to update

**Request Body:**
```json
{
  "name": "Updated Premium Hosting",
  "description": "Updated premium hosting package",
  "diskSpace": 15000,
  "bandwidth": 150000,
  "emailAccounts": 15,
  "databases": 10,
  "price": 24.99,
  "setupFee": 10.00,
  "isActive": true
}
```

**Response:**
- 200: Returns the updated hosting package
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If hosting package is not found
- 500: If an internal server error occurs

### DELETE /api/v1/HostingPackages/{id}

Deletes a hosting package from the system.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the hosting package to delete

**Response:**
- 204: If hosting package was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If hosting package is not found
- 500: If an internal server error occurs

## Data Models

### HostingPackageDto

```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "diskSpace": 0,
  "bandwidth": 0,
  "emailAccounts": 0,
  "databases": 0,
  "price": 0.0,
  "setupFee": 0.0,
  "isActive": true,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateHostingPackageDto

```json
{
  "name": "string",
  "description": "string",
  "diskSpace": 0,
  "bandwidth": 0,
  "emailAccounts": 0,
  "databases": 0,
  "price": 0.0,
  "setupFee": 0.0,
  "isActive": true
}
```

### UpdateHostingPackageDto

```json
{
  "name": "string",
  "description": "string",
  "diskSpace": 0,
  "bandwidth": 0,
  "emailAccounts": 0,
  "databases": 0,
  "price": 0.0,
  "setupFee": 0.0,
  "isActive": true
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

- **v1.0:** Initial release of HostingPackages API