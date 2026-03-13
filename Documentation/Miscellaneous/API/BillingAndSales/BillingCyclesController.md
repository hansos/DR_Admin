# BillingCycles API Documentation

## Overview

Manages billing cycles including creation, retrieval, updates, and deletion

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
    "durationInDays": 30,
    "description": "Monthly billing cycle",
    "createdAt": "2023-01-01T00:00:00Z",
    "updatedAt": "2023-01-01T00:00:00Z"
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

**Response Body:**
```json
{
  "id": 1,
  "name": "Monthly",
  "durationInDays": 30,
  "description": "Monthly billing cycle",
  "createdAt": "2023-01-01T00:00:00Z",
  "updatedAt": "2023-01-01T00:00:00Z"
}
```

### POST /api/v1/BillingCycles

Creates a new billing cycle in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "Monthly",
  "durationInDays": 30,
  "description": "Monthly billing cycle"
}
```

**Response:**
- 201: Returns the newly created billing cycle
- 400: If the billing cycle data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "name": "Monthly",
  "durationInDays": 30,
  "description": "Monthly billing cycle",
  "createdAt": "2023-01-01T00:00:00Z",
  "updatedAt": "2023-01-01T00:00:00Z"
}
```

### PUT /api/v1/BillingCycles/{id}

Updates an existing billing cycle's information.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the billing cycle to update

**Request Body:**
```json
{
  "name": "Monthly",
  "durationInDays": 30,
  "description": "Monthly billing cycle"
}
```

**Response:**
- 200: Returns the updated billing cycle
- 400: If the billing cycle data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If billing cycle is not found
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "name": "Monthly",
  "durationInDays": 30,
  "description": "Monthly billing cycle",
  "createdAt": "2023-01-01T00:00:00Z",
  "updatedAt": "2023-01-01T00:00:00Z"
}
```

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

[Back to Controllers Index](index.md)