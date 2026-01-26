# ServerControlPanels API Documentation

## Overview

The ServerControlPanels API provides endpoints for managing server control panels. It allows administrators and support staff to create, retrieve, update, delete, and test connections to server control panels.

## Base URL

```
https://api.example.com/api/v1/ServerControlPanels
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/ServerControlPanels

Retrieves all server control panels in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of server control panels
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "serverId": 123,
    "controlPanelTypeId": 456,
    "url": "https://cp.example.com:2083",
    "username": "admin",
    "apiKey": "encrypted_key",
    "isActive": true,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/ServerControlPanels/server/{serverId}

Retrieves control panels for a specific server.

**Authorization:** Admin, Support

**Parameters:**
- `serverId` (path): The unique identifier of the server

**Response:**
- 200: Returns the list of server control panels
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/ServerControlPanels/{id}

Retrieves a specific server control panel by its unique identifier.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The unique identifier of the control panel

**Response:**
- 200: Returns the server control panel data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If control panel is not found
- 500: If an internal server error occurs

### POST /api/v1/ServerControlPanels

Creates a new server control panel in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "serverId": 123,
  "controlPanelTypeId": 456,
  "url": "https://cp.example.com:2083",
  "username": "admin",
  "apiKey": "encrypted_key",
  "isActive": true
}
```

**Response:**
- 201: Returns the newly created server control panel
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/ServerControlPanels/{id}

Updates an existing server control panel's information.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the control panel to update

**Request Body:**
```json
{
  "serverId": 123,
  "controlPanelTypeId": 456,
  "url": "https://cp-updated.example.com:2083",
  "username": "admin",
  "apiKey": "encrypted_key",
  "isActive": true
}
```

**Response:**
- 200: Returns the updated server control panel
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If control panel is not found
- 500: If an internal server error occurs

### DELETE /api/v1/ServerControlPanels/{id}

Deletes a server control panel from the system.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the control panel to delete

**Response:**
- 204: If control panel was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If control panel is not found
- 500: If an internal server error occurs

### POST /api/v1/ServerControlPanels/{id}/test-connection

Tests the connection to a server control panel.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The unique identifier of the control panel

**Response:**
- 200: Returns the connection test result
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If control panel is not found
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "success": true
}
```

## Data Models

### ServerControlPanelDto

```json
{
  "id": 0,
  "serverId": 0,
  "controlPanelTypeId": 0,
  "url": "string",
  "username": "string",
  "apiKey": "string",
  "isActive": true,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateServerControlPanelDto

```json
{
  "serverId": 0,
  "controlPanelTypeId": 0,
  "url": "string",
  "username": "string",
  "apiKey": "string",
  "isActive": true
}
```

### UpdateServerControlPanelDto

```json
{
  "serverId": 0,
  "controlPanelTypeId": 0,
  "url": "string",
  "username": "string",
  "apiKey": "string",
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

- **v1.0:** Initial release of ServerControlPanels API