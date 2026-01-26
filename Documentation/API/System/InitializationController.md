# Initialization API Documentation

## Overview

The Initialization API provides endpoints for system initialization. It allows setting up the system with the first admin user.

## Base URL

```
https://api.example.com/api/v1/Initialization
```

## Authentication

The initialization endpoint does not require authentication (AllowAnonymous).

## Endpoints

### POST /api/v1/Initialization/initialize

Initializes the system with the first admin user (only works if no users exist).

**Authorization:** None (public endpoint)

**Request Body:**
```json
{
  "username": "admin",
  "password": "securepassword123",
  "email": "admin@example.com",
  "firstName": "System",
  "lastName": "Administrator"
}
```

**Response:**
- 200: Returns the initialization result if successful
- 400: If required fields are missing, users already exist, or input is invalid
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "username": "admin",
  "email": "admin@example.com",
  "firstName": "System",
  "lastName": "Administrator",
  "roles": ["Admin"],
  "isActive": true,
  "createdAt": "2023-01-15T10:30:00Z"
}
```

**Note:** This endpoint can only be used once to create the first admin user. Subsequent calls will fail.

## Data Models

### InitializationRequestDto

```json
{
  "username": "string",
  "password": "string",
  "email": "string",
  "firstName": "string",
  "lastName": "string"
}
```

### InitializationResponseDto

```json
{
  "id": 0,
  "username": "string",
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "roles": ["string"],
  "isActive": true,
  "createdAt": "2023-01-15T10:30:00Z"
}
```

## Error Handling

All endpoints return appropriate HTTP status codes and error messages in case of failures. Common error responses include:

- **400 Bad Request:** Invalid request data, missing required fields, or users already exist
- **500 Internal Server Error:** Server-side error

## Rate Limiting

API calls are subject to rate limiting. Please refer to the general API documentation for rate limit details.

## Changelog

- **v1.0:** Initial release of Initialization API