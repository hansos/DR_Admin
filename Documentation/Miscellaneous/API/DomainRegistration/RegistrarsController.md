# Registrars API Documentation

## Overview

The Registrars API provides endpoints for managing domain registrars and their TLD offerings. It allows administrators to configure registrars and assign TLDs to them.

## Base URL

```
https://api.example.com/api/v1/Registrars
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/Registrars/{registrarId}/tlds

Retrieves all TLDs supported by a specific registrar.

**Authorization:** None (public endpoint)

**Parameters:**
- `registrarId` (path): The unique identifier of the registrar

**Response:**
- 200: Returns the list of TLDs
- 401: If user is not authenticated
- 500: If an internal server error occurs

### POST /api/v1/Registrars/{registrarId}/tld/{tldId}

Assigns a TLD to a registrar using their unique identifiers.

**Authorization:** Admin

**Parameters:**
- `registrarId` (path): The unique identifier of the registrar
- `tldId` (path): The unique identifier of the TLD

**Response:**
- 201: Returns the created registrar-TLD relationship
- 400: If the assignment is invalid or already exists
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### POST /api/v1/Registrars/{registrarId}/tld

Assigns a TLD to a registrar using TLD details.

**Authorization:** Admin

**Parameters:**
- `registrarId` (path): The unique identifier of the registrar

**Request Body:**
```json
{
  "id": 456,
  "extension": "com",
  "name": "Commercial",
  "description": "Commercial domains",
  "isActive": true,
  "registrationPrice": 12.99,
  "renewalPrice": 12.99,
  "transferPrice": 12.99
}
```

**Response:**
- 201: Returns the created registrar-TLD relationship
- 400: If the TLD data is invalid or assignment already exists
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### GET /api/v1/Registrars

Retrieves all domain registrars in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of registrars
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "Namecheap",
    "code": "NAMECHEAP",
    "description": "Namecheap domain registrar",
    "isActive": true,
    "apiUrl": "https://api.namecheap.com/xml.response",
    "username": "apiuser",
    "apiKey": "encrypted_key",
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/Registrars/active

Get active registrars only.

**Authorization:** None (public endpoint)

**Response:**
- 200: Returns the list of active registrars
- 401: If user is not authenticated
- 500: If an internal server error occurs

### GET /api/v1/Registrars/{id}

Get registrar by ID.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The unique identifier of the registrar

**Response:**
- 200: Returns the registrar data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If registrar is not found
- 500: If an internal server error occurs

### GET /api/v1/Registrars/code/{code}

Get registrar by code.

**Authorization:** Admin, Support

**Parameters:**
- `code` (path): The registrar code

**Response:**
- 200: Returns the registrar data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If registrar is not found
- 500: If an internal server error occurs

### POST /api/v1/Registrars

Create a new registrar.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "GoDaddy",
  "code": "GODADDY",
  "description": "GoDaddy domain registrar",
  "isActive": true,
  "apiUrl": "https://api.godaddy.com/v1",
  "username": "apiuser",
  "apiKey": "encrypted_key"
}
```

**Response:**
- 201: Returns the newly created registrar
- 400: If the registrar data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/Registrars/{id}

Update an existing registrar.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the registrar to update

**Request Body:**
```json
{
  "name": "GoDaddy Updated",
  "code": "GODADDY",
  "description": "Updated GoDaddy registrar",
  "isActive": true,
  "apiUrl": "https://api.godaddy.com/v1",
  "username": "apiuser",
  "apiKey": "encrypted_key"
}
```

**Response:**
- 200: Returns the updated registrar
- 400: If the registrar data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If registrar is not found
- 500: If an internal server error occurs

### DELETE /api/v1/Registrars/{id}

Delete a registrar.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the registrar to delete

**Response:**
- 204: If registrar was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If registrar is not found
- 500: If an internal server error occurs

## Data Models

### RegistrarDto

```json
{
  "id": 0,
  "name": "string",
  "code": "string",
  "description": "string",
  "isActive": true,
  "apiUrl": "string",
  "username": "string",
  "apiKey": "string",
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateRegistrarDto

```json
{
  "name": "string",
  "code": "string",
  "description": "string",
  "isActive": true,
  "apiUrl": "string",
  "username": "string",
  "apiKey": "string"
}
```

### UpdateRegistrarDto

```json
{
  "name": "string",
  "code": "string",
  "description": "string",
  "isActive": true,
  "apiUrl": "string",
  "username": "string",
  "apiKey": "string"
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

- **v1.0:** Initial release of Registrars API