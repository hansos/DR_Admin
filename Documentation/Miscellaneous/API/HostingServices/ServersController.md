# Servers API Documentation

## Overview

The Servers API provides endpoints for managing servers in the system. It allows administrators and support staff to create, retrieve, update, and delete server records.

## Base URL

```
https://api.example.com/api/v1/Servers
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/Servers

Retrieves all servers in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of servers
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "Web Server 1",
    "hostname": "web1.example.com",
    "ipAddress": "192.168.1.10",
    "controlPanelTypeId": 1,
    "location": "Data Center A",
    "status": "Active",
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/Servers/{id}

Retrieves a specific server by its unique identifier.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The unique identifier of the server

**Response:**
- 200: Returns the server data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If server is not found
- 500: If an internal server error occurs

### POST /api/v1/Servers

Creates a new server in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "Web Server 2",
  "hostname": "web2.example.com",
  "ipAddress": "192.168.1.11",
  "controlPanelTypeId": 1,
  "location": "Data Center A",
  "status": "Active"
}
```

**Response:**
- 201: Returns the newly created server
- 400: If the server data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/Servers/{id}

Updates an existing server's information.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the server to update

**Request Body:**
```json
{
  "name": "Updated Web Server 2",
  "hostname": "web2-updated.example.com",
  "ipAddress": "192.168.1.11",
  "controlPanelTypeId": 1,
  "location": "Data Center B",
  "status": "Maintenance"
}
```

**Response:**
- 200: Returns the updated server
- 400: If the server data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If server is not found
- 500: If an internal server error occurs

### DELETE /api/v1/Servers/{id}

Deletes a server from the system.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the server to delete

**Response:**
- 204: If server was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If server is not found
- 500: If an internal server error occurs

## Data Models

### ServerDto

```json
{
  "id": 0,
  "name": "string",
  "hostname": "string",
  "ipAddress": "string",
  "controlPanelTypeId": 0,
  "location": "string",
  "status": "string",
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateServerDto

```json
{
  "name": "string",
  "hostname": "string",
  "ipAddress": "string",
  "controlPanelTypeId": 0,
  "location": "string",
  "status": "string"
}
```

### UpdateServerDto

```json
{
  "name": "string",
  "hostname": "string",
  "ipAddress": "string",
  "controlPanelTypeId": 0,
  "location": "string",
  "status": "string"
}
```

## Server Statuses

- **Active**: Server is operational and available
- **Maintenance**: Server is under maintenance
- **Offline**: Server is offline or unreachable
- **Decommissioned**: Server has been decommissioned

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

- **v1.0:** Initial release of Servers API