# RegistrarTlds API Documentation

## Overview

The RegistrarTlds API provides endpoints for managing registrar-TLD relationships and pricing configurations. It allows administrators to configure which registrars support which TLDs and their associated pricing.

## Base URL

```
https://api.example.com/api/v1/RegistrarTlds
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/RegistrarTlds

Retrieves all registrar-TLD offerings in the system.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of registrar-TLD offerings
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "registrarId": 123,
    "tldId": 456,
    "registrationPrice": 12.99,
    "renewalPrice": 12.99,
    "transferPrice": 12.99,
    "isAvailable": true,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/RegistrarTlds/available

Retrieves only available registrar-TLD offerings for purchase.

**Authorization:** None (public endpoint)

**Response:**
- 200: Returns the list of available offerings
- 401: If user is not authenticated
- 500: If an internal server error occurs

### GET /api/v1/RegistrarTlds/registrar/{registrarId}

Retrieves all TLD offerings for a specific registrar.

**Authorization:** None (public endpoint)

**Parameters:**
- `registrarId` (path): The unique identifier of the registrar

**Response:**
- 200: Returns the list of TLD offerings
- 401: If user is not authenticated
- 500: If an internal server error occurs

### GET /api/v1/RegistrarTlds/tld/{tldId}

Retrieves all registrars offering a specific TLD.

**Authorization:** None (public endpoint)

**Parameters:**
- `tldId` (path): The unique identifier of the TLD

**Response:**
- 200: Returns the list of registrar offerings
- 401: If user is not authenticated
- 500: If an internal server error occurs

### GET /api/v1/RegistrarTlds/{id}

Retrieves a specific registrar-TLD offering by its unique identifier.

**Authorization:** None (public endpoint)

**Parameters:**
- `id` (path): The unique identifier of the registrar-TLD relationship

**Response:**
- 200: Returns the registrar-TLD offering data
- 401: If user is not authenticated
- 404: If registrar-TLD offering is not found
- 500: If an internal server error occurs

### GET /api/v1/RegistrarTlds/registrar/{registrarId}/tld/{tldId}

Get registrar TLD offering by registrar and TLD combination.

**Authorization:** None (public endpoint)

**Parameters:**
- `registrarId` (path): The unique identifier of the registrar
- `tldId` (path): The unique identifier of the TLD

**Response:**
- 200: Returns the registrar-TLD offering data
- 401: If user is not authenticated
- 404: If registrar-TLD offering is not found
- 500: If an internal server error occurs

### POST /api/v1/RegistrarTlds

Create a new registrar TLD offering.

**Authorization:** Admin

**Request Body:**
```json
{
  "registrarId": 123,
  "tldId": 456,
  "registrationPrice": 12.99,
  "renewalPrice": 12.99,
  "transferPrice": 12.99,
  "isAvailable": true
}
```

**Response:**
- 201: Returns the newly created registrar TLD offering
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/RegistrarTlds/{id}

Update an existing registrar TLD offering.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the registrar-TLD offering to update

**Request Body:**
```json
{
  "registrarId": 123,
  "tldId": 456,
  "registrationPrice": 13.99,
  "renewalPrice": 13.99,
  "transferPrice": 13.99,
  "isAvailable": true
}
```

**Response:**
- 200: Returns the updated registrar TLD offering
- 400: If the data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If registrar-TLD offering is not found
- 500: If an internal server error occurs

### DELETE /api/v1/RegistrarTlds/{id}

Delete a registrar TLD offering.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the registrar-TLD offering to delete

**Response:**
- 204: If registrar-TLD offering was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If registrar-TLD offering is not found
- 500: If an internal server error occurs

## Data Models

### RegistrarTldDto

```json
{
  "id": 0,
  "registrarId": 0,
  "tldId": 0,
  "registrationPrice": 0.0,
  "renewalPrice": 0.0,
  "transferPrice": 0.0,
  "isAvailable": true,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateRegistrarTldDto

```json
{
  "registrarId": 0,
  "tldId": 0,
  "registrationPrice": 0.0,
  "renewalPrice": 0.0,
  "transferPrice": 0.0,
  "isAvailable": true
}
```

### UpdateRegistrarTldDto

```json
{
  "registrarId": 0,
  "tldId": 0,
  "registrationPrice": 0.0,
  "renewalPrice": 0.0,
  "transferPrice": 0.0,
  "isAvailable": true
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

- **v1.0:** Initial release of RegistrarTlds API