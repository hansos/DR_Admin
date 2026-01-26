# ServiceTypes API Documentation

## Overview

The ServiceTypes API provides endpoints for managing service types. It allows administrators to create, retrieve, update, and delete service type records.

## Base URL

```
https://api.example.com/api/v1/ServiceTypes
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/ServiceTypes

Retrieves all service types in the system.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of service types
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "Web Hosting",
    "description": "Web hosting services",
    "isActive": true,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/ServiceTypes/{id}

Retrieves a specific service type by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the service type

**Response:**
- 200: Returns the service type data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If service type is not found
- 500: If an internal server error occurs

### POST /api/v1/ServiceTypes

Creates a new service type in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "Email Hosting",
  "description": "Email hosting services",
  "isActive": true
}
```

**Response:**
- 201: Returns the newly created service type
- 400: If the service type data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/ServiceTypes/{id}

Updates an existing service type's information.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the service type to update

**Request Body:**
```json
{
  "name": "Updated Email Hosting",
  "description": "Updated email hosting services",
  "isActive": true
}
```

**Response:**
- 200: Returns the updated service type
- 400: If the service type data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If service type is not found
- 500: If an internal server error occurs

### DELETE /api/v1/ServiceTypes/{id}

Deletes a service type from the system.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the service type to delete

**Response:**
- 204: If service type was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If service type is not found
- 500: If an internal server error occurs

## Data Models

### ServiceTypeDto

```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "isActive": true,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateServiceTypeDto

```json
{
  "name": "string",
  "description": "string",
  "isActive": true
}
```

### UpdateServiceTypeDto

```json
{
  "name": "string",
  "description": "string",
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

- **v1.0:** Initial release of ServiceTypes API