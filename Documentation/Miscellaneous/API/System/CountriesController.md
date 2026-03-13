# Countries API Documentation

## Overview

The Countries API provides endpoints for managing countries and their information. It allows users to retrieve country data and administrators to manage country records.

## Base URL

```
https://api.example.com/api/v1/Countries
```

## Authentication

All endpoints require authentication using JWT tokens. Some endpoints require Admin role for modifications.

## Endpoints

### GET /api/v1/Countries

Retrieves all countries in the system.

**Authorization:** None (public endpoint)

**Response:**
- 200: Returns the list of countries
- 401: If user is not authenticated
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "United States",
    "code": "US",
    "isoCode": "USA",
    "phoneCode": "+1",
    "currencyCode": "USD",
    "isActive": true,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/Countries/active

Retrieves only active countries.

**Authorization:** None (public endpoint)

**Response:**
- 200: Returns the list of active countries
- 401: If user is not authenticated
- 500: If an internal server error occurs

### GET /api/v1/Countries/{id}

Retrieves a specific country by its unique identifier.

**Authorization:** None (public endpoint)

**Parameters:**
- `id` (path): The unique identifier of the country

**Response:**
- 200: Returns the country data
- 401: If user is not authenticated
- 404: If country is not found
- 500: If an internal server error occurs

### GET /api/v1/Countries/code/{code}

Retrieves a specific country by its country code.

**Authorization:** None (public endpoint)

**Parameters:**
- `code` (path): The country code (e.g., "US", "GB")

**Response:**
- 200: Returns the country data
- 401: If user is not authenticated
- 404: If country is not found
- 500: If an internal server error occurs

### POST /api/v1/Countries

Creates a new country in the system.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "Canada",
  "code": "CA",
  "isoCode": "CAN",
  "numeric": 124,
  "phoneCode": "+1",
  "currencyCode": "CAD",
  "isActive": true
}
```

**Response:**
- 201: Returns the newly created country
- 400: If the country data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/Countries/{id}

Update an existing country.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the country to update

**Request Body:**
```json
{
  "name": "Canada Updated",
  "code": "CA",
  "isoCode": "CAN",
  "phoneCode": "+1",
  "currencyCode": "CAD",
  "isActive": true
}
```

**Response:**
- 200: Returns the updated country
- 400: If the country data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If country is not found
- 500: If an internal server error occurs

### DELETE /api/v1/Countries/{id}

Delete a country.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the country to delete

**Response:**
- 204: If country was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If country is not found
- 500: If an internal server error occurs

## Data Models

### CountryDto

```json
{
  "id": 0,
  "name": "string",
  "code": "string",
  "isoCode": "string",
  "phoneCode": "string",
  "currencyCode": "string",
  "isActive": true,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateCountryDto

```json
{
  "name": "string",
  "code": "string",
  "isoCode": "string",
  "phoneCode": "string",
  "currencyCode": "string",
  "isActive": true
}
```

### UpdateCountryDto

```json
{
  "name": "string",
  "code": "string",
  "isoCode": "string",
  "phoneCode": "string",
  "currencyCode": "string",
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

- **v1.0:** Initial release of Countries API