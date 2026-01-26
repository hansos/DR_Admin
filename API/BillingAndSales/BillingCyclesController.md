# BillingCycles API Documentation

## Overview

The BillingCycles API provides endpoints for managing billing cycles in the system. It allows administrators, support staff, and sales personnel to create, retrieve, update, and delete billing cycle records.

## Base URL

```
https://api.example.com/api/v1/BillingCycles
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Sales depending on the endpoint).

## Endpoints

### GET /api/v1/BillingCycles

Retrieves all billing cycles in the system.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of billing cycles
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "Monthly",
    "description": "Monthly billing cycle",
    "days": 30,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/BillingCycles/{id}

Retrieves a specific billing cycle by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the billing cycle

**Response:**
- 200: Returns the billing cycle data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If billing cycle is not found
- 500: If an internal server error occurs

### POST /api/v1/BillingCycles

Creates a new billing cycle in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "Quarterly",
  "description": "Quarterly billing cycle",
  "days": 90
}
```

**Response:**
- 201: Returns the newly created billing cycle
- 400: If the billing cycle data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/BillingCycles/{id}

Updates an existing billing cycle's information.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the billing cycle to update

**Request Body:**
```json
{
  "name": "Updated Name",
  "description": "Updated description",
  "days": 120
}
```

**Response:**
- 200: Returns the updated billing cycle
- 400: If the billing cycle data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If billing cycle is not found
- 500: If an internal server error occurs

### DELETE /api/v1/BillingCycles/{id}

Deletes a billing cycle from the system.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the billing cycle to delete

**Response:**
- 204: If billing cycle was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If billing cycle is not found
- 500: If an internal server error occurs

## Data Models

### BillingCycleDto

```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "days": 0,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateBillingCycleDto

```json
{
  "name": "string",
  "description": "string",
  "days": 0
}
```

### UpdateBillingCycleDto

```json
{
  "name": "string",
  "description": "string",
  "days": 0
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

- **v1.0:** Initial release of BillingCycles API