# PostalCodes API Documentation

## Overview

The PostalCodes API provides endpoints for managing postal codes and their geographic information. It allows users to retrieve postal code data and administrators to manage postal code records.

## Base URL

```
https://api.example.com/api/v1/PostalCodes
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/PostalCodes

Retrieves all postal codes in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of postal codes
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "code": "12345",
    "city": "New York",
    "state": "NY",
    "countryCode": "US",
    "latitude": 40.7128,
    "longitude": -74.0060,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/PostalCodes/country/{countryCode}

Retrieves all postal codes for a specific country.

**Authorization:** None (public endpoint)

**Parameters:**
- `countryCode` (path): The country code (e.g., "US", "GB")

**Response:**
- 200: Returns the list of postal codes
- 401: If user is not authenticated
- 500: If an internal server error occurs

### GET /api/v1/PostalCodes/city/{city}

Retrieves all postal codes for a specific city.

**Authorization:** None (public endpoint)

**Parameters:**
- `city` (path): The city name

**Response:**
- 200: Returns the list of postal codes
- 401: If user is not authenticated
- 500: If an internal server error occurs

### GET /api/v1/PostalCodes/{id}

Retrieves a specific postal code by its unique identifier.

**Authorization:** None (public endpoint)

**Parameters:**
- `id` (path): The unique identifier of the postal code

**Response:**
- 200: Returns the postal code data
- 401: If user is not authenticated
- 404: If postal code is not found
- 500: If an internal server error occurs

### GET /api/v1/PostalCodes/{code}/country/{countryCode}

Retrieves a specific postal code by code and country.

**Authorization:** None (public endpoint)

**Parameters:**
- `code` (path): The postal code
- `countryCode` (path): The country code (e.g., "US", "GB")

**Response:**
- 200: Returns the postal code data
- 401: If user is not authenticated
- 404: If postal code is not found
- 500: If an internal server error occurs

### POST /api/v1/PostalCodes

Create a new postal code.

**Authorization:** Admin

**Request Body:**
```json
{
  "code": "67890",
  "city": "Los Angeles",
  "state": "CA",
  "countryCode": "US",
  "latitude": 34.0522,
  "longitude": -118.2437
}
```

**Response:**
- 201: Returns the newly created postal code
- 400: If the postal code data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/PostalCodes/{id}

Update an existing postal code.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the postal code to update

**Request Body:**
```json
{
  "code": "67890",
  "city": "Los Angeles Updated",
  "state": "CA",
  "countryCode": "US",
  "latitude": 34.0522,
  "longitude": -118.2437
}
```

**Response:**
- 200: Returns the updated postal code
- 400: If the postal code data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If postal code is not found
- 500: If an internal server error occurs

### DELETE /api/v1/PostalCodes/{id}

Delete a postal code.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the postal code to delete

**Response:**
- 204: If postal code was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If postal code is not found
- 500: If an internal server error occurs

## Data Models

### PostalCodeDto

```json
{
  "id": 0,
  "code": "string",
  "city": "string",
  "state": "string",
  "countryCode": "string",
  "latitude": 0.0,
  "longitude": 0.0,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreatePostalCodeDto

```json
{
  "code": "string",
  "city": "string",
  "state": "string",
  "countryCode": "string",
  "latitude": 0.0,
  "longitude": 0.0
}
```

### UpdatePostalCodeDto

```json
{
  "code": "string",
  "city": "string",
  "state": "string",
  "countryCode": "string",
  "latitude": 0.0,
  "longitude": 0.0
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

- **v1.0:** Initial release of PostalCodes API