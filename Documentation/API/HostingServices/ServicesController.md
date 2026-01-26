# Services API Documentation

## Overview

The Services API provides endpoints for managing services offered to customers. It allows administrators to create, retrieve, update, and delete service records.

## Base URL

```
https://api.example.com/api/v1/Services
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/Services

Retrieves all services in the system.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of services
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "Basic Hosting",
    "description": "Basic web hosting service",
    "serviceTypeId": 123,
    "hostingPackageId": 456,
    "price": 9.99,
    "setupFee": 0.00,
    "isActive": true,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/Services/{id}

Retrieves a specific service by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the service

**Response:**
- 200: Returns the service data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If service is not found
- 500: If an internal server error occurs

### POST /api/v1/Services

Creates a new service in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "Premium Hosting",
  "description": "Premium web hosting service",
  "serviceTypeId": 123,
  "hostingPackageId": 456,
  "price": 19.99,
  "setupFee": 5.00,
  "isActive": true
}
```

**Response:**
- 201: Returns the newly created service
- 400: If the service data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/Services/{id}

Updates an existing service's information.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the service to update

**Request Body:**
```json
{
  "name": "Updated Premium Hosting",
  "description": "Updated premium hosting service",
  "serviceTypeId": 123,
  "hostingPackageId": 456,
  "price": 24.99,
  "setupFee": 10.00,
  "isActive": true
}
```

**Response:**
- 200: Returns the updated service
- 400: If the service data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If service is not found
- 500: If an internal server error occurs

### DELETE /api/v1/Services/{id}

Deletes a service from the system.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the service to delete

**Response:**
- 204: If service was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If service is not found
- 500: If an internal server error occurs

## Data Models

### ServiceDto

```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "serviceTypeId": 0,
  "hostingPackageId": 0,
  "price": 0.0,
  "setupFee": 0.0,
  "isActive": true,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateServiceDto

```json
{
  "name": "string",
  "description": "string",
  "serviceTypeId": 0,
  "hostingPackageId": 0,
  "price": 0.0,
  "setupFee": 0.0,
  "isActive": true
}
```

### UpdateServiceDto

```json
{
  "name": "string",
  "description": "string",
  "serviceTypeId": 0,
  "hostingPackageId": 0,
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

- **v1.0:** Initial release of Services API