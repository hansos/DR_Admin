# ControlPanelTypes API Documentation

## Overview

The ControlPanelTypes API provides endpoints for managing control panel types. It allows administrators, support staff, and sales personnel to create, retrieve, update, and delete control panel type records.

## Base URL

```
https://api.example.com/api/v1/ControlPanelTypes
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/ControlPanelTypes

Retrieves all control panel types in the system.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of control panel types
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "cPanel",
    "description": "cPanel web hosting control panel",
    "version": "11.0",
    "isActive": true,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/ControlPanelTypes/active

Retrieves only active control panel types.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of active control panel types
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/ControlPanelTypes/{id}

Retrieves a specific control panel type by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the control panel type

**Response:**
- 200: Returns the control panel type data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If control panel type is not found
- 500: If an internal server error occurs

### POST /api/v1/ControlPanelTypes

Creates a new control panel type in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "Plesk",
  "description": "Plesk web hosting control panel",
  "version": "18.0",
  "isActive": true
}
```

**Response:**
- 201: Returns the newly created control panel type
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/ControlPanelTypes/{id}

Updates an existing control panel type's information.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the control panel type to update

**Request Body:**
```json
{
  "name": "Plesk Updated",
  "description": "Updated Plesk web hosting control panel",
  "version": "18.0.50",
  "isActive": true
}
```

**Response:**
- 200: Returns the updated control panel type
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If control panel type is not found
- 500: If an internal server error occurs

### DELETE /api/v1/ControlPanelTypes/{id}

Deletes a control panel type from the system.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the control panel type to delete

**Response:**
- 204: If control panel type was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If control panel type is not found
- 500: If an internal server error occurs

## Data Models

### ControlPanelTypeDto

```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "version": "string",
  "isActive": true,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateControlPanelTypeDto

```json
{
  "name": "string",
  "description": "string",
  "version": "string",
  "isActive": true
}
```

### UpdateControlPanelTypeDto

```json
{
  "name": "string",
  "description": "string",
  "version": "string",
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

- **v1.0:** Initial release of ControlPanelTypes API