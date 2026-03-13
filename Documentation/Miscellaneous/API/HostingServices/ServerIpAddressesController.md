# ServerIpAddresses API Documentation

## Overview

The ServerIpAddresses API provides endpoints for managing server IP addresses. It allows administrators and support staff to create, retrieve, update, and delete server IP address records.

## Base URL

```
https://api.example.com/api/v1/ServerIpAddresses
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/ServerIpAddresses

Retrieves all server IP addresses in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of server IP addresses
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "serverId": 123,
    "ipAddress": "192.168.1.10",
    "subnetMask": "255.255.255.0",
    "gateway": "192.168.1.1",
    "isPrimary": true,
    "status": "Active",
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/ServerIpAddresses/server/{serverId}

Retrieves IP addresses for a specific server.

**Authorization:** Admin, Support

**Parameters:**
- `serverId` (path): The unique identifier of the server

**Response:**
- 200: Returns the list of server IP addresses
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/ServerIpAddresses/{id}

Retrieves a specific server IP address by its unique identifier.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The unique identifier of the IP address

**Response:**
- 200: Returns the server IP address data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If IP address is not found
- 500: If an internal server error occurs

### POST /api/v1/ServerIpAddresses

Creates a new server IP address in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "serverId": 123,
  "ipAddress": "192.168.1.11",
  "subnetMask": "255.255.255.0",
  "gateway": "192.168.1.1",
  "isPrimary": false,
  "status": "Active"
}
```

**Response:**
- 201: Returns the newly created server IP address
- 400: If the IP address data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/ServerIpAddresses/{id}

Updates an existing server IP address's information.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the IP address to update

**Request Body:**
```json
{
  "serverId": 123,
  "ipAddress": "192.168.1.12",
  "subnetMask": "255.255.255.0",
  "gateway": "192.168.1.1",
  "isPrimary": false,
  "status": "Active"
}
```

**Response:**
- 200: Returns the updated server IP address
- 400: If the IP address data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If IP address is not found
- 500: If an internal server error occurs

### DELETE /api/v1/ServerIpAddresses/{id}

Deletes a server IP address from the system.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the IP address to delete

**Response:**
- 204: If IP address was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If IP address is not found
- 500: If an internal server error occurs

## Data Models

### ServerIpAddressDto

```json
{
  "id": 0,
  "serverId": 0,
  "ipAddress": "string",
  "subnetMask": "string",
  "gateway": "string",
  "isPrimary": true,
  "status": "string",
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateServerIpAddressDto

```json
{
  "serverId": 0,
  "ipAddress": "string",
  "subnetMask": "string",
  "gateway": "string",
  "isPrimary": true,
  "status": "string"
}
```

### UpdateServerIpAddressDto

```json
{
  "serverId": 0,
  "ipAddress": "string",
  "subnetMask": "string",
  "gateway": "string",
  "isPrimary": true,
  "status": "string"
}
```

## IP Address Statuses

- **Active**: IP address is active and in use
- **Reserved**: IP address is reserved but not currently assigned
- **Inactive**: IP address is inactive

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

- **v1.0:** Initial release of ServerIpAddresses API