# Tlds API Documentation

## Overview

The Tlds API provides endpoints for managing Top-Level Domains (TLDs) and their associated registrars. It allows users to retrieve TLD information and administrators to manage TLD configurations.

## Base URL

```
https://api.example.com/api/v1/Tlds
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/Tlds/{tldId}/registrars

Retrieves all registrars supporting a specific TLD.

**Authorization:** None (public endpoint)

**Parameters:**
- `tldId` (path): The unique identifier of the TLD

**Response:**
- 200: Returns the list of registrars
- 401: If user is not authenticated
- 500: If an internal server error occurs

### GET /api/v1/Tlds

Retrieves all Top-Level Domains in the system.

**Authorization:** None (public endpoint)

**Response:**
- 200: Returns the list of TLDs
- 401: If user is not authenticated
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "extension": "com",
    "name": "Commercial",
    "description": "Commercial domains",
    "isActive": true,
    "registrationPrice": 12.99,
    "renewalPrice": 12.99,
    "transferPrice": 12.99,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/Tlds/active

Retrieves only active Top-Level Domains.

**Authorization:** None (public endpoint)

**Response:**
- 200: Returns the list of active TLDs
- 401: If user is not authenticated
- 500: If an internal server error occurs

### GET /api/v1/Tlds/{id}

Retrieves a specific TLD by its unique identifier.

**Authorization:** None (public endpoint)

**Parameters:**
- `id` (path): The unique identifier of the TLD

**Response:**
- 200: Returns the TLD data
- 401: If user is not authenticated
- 404: If TLD is not found
- 500: If an internal server error occurs

### GET /api/v1/Tlds/extension/{extension}

Retrieves a specific TLD by its extension.

**Authorization:** None (public endpoint)

**Parameters:**
- `extension` (path): The TLD extension (e.g., "com", "net", "org")

**Response:**
- 200: Returns the TLD data
- 401: If user is not authenticated
- 404: If TLD is not found
- 500: If an internal server error occurs

### POST /api/v1/Tlds

Create a new TLD.

**Authorization:** Admin

**Request Body:**
```json
{
  "extension": "xyz",
  "name": "XYZ Domain",
  "description": "New XYZ domains",
  "isActive": true,
  "registrationPrice": 15.99,
  "renewalPrice": 15.99,
  "transferPrice": 15.99
}
```

**Response:**
- 201: Returns the newly created TLD
- 400: If the TLD data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/Tlds/{id}

Update an existing TLD.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the TLD to update

**Request Body:**
```json
{
  "extension": "xyz",
  "name": "Updated XYZ Domain",
  "description": "Updated description",
  "isActive": true,
  "registrationPrice": 16.99,
  "renewalPrice": 16.99,
  "transferPrice": 16.99
}
```

**Response:**
- 200: Returns the updated TLD
- 400: If the TLD data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If TLD is not found
- 500: If an internal server error occurs

### DELETE /api/v1/Tlds/{id}

Delete a TLD.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the TLD to delete

**Response:**
- 204: If TLD was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If TLD is not found
- 500: If an internal server error occurs

## Data Models

### TldDto

```json
{
  "id": 0,
  "extension": "string",
  "name": "string",
  "description": "string",
  "isActive": true,
  "registrationPrice": 0.0,
  "renewalPrice": 0.0,
  "transferPrice": 0.0,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateTldDto

```json
{
  "extension": "string",
  "name": "string",
  "description": "string",
  "isActive": true,
  "registrationPrice": 0.0,
  "renewalPrice": 0.0,
  "transferPrice": 0.0
}
```

### UpdateTldDto

```json
{
  "extension": "string",
  "name": "string",
  "description": "string",
  "isActive": true,
  "registrationPrice": 0.0,
  "renewalPrice": 0.0,
  "transferPrice": 0.0
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

- **v1.0:** Initial release of Tlds API