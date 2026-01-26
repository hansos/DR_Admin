# SalesAgents API Documentation

## Overview

The SalesAgents API provides endpoints for managing sales agents. It allows administrators and sales personnel to create, retrieve, update, and delete sales agent records.

## Base URL

```
https://api.example.com/api/v1/SalesAgents
```

## Authentication

All endpoints require authentication using JWT tokens. Most endpoints require Admin role.

## Endpoints

### GET /api/v1/SalesAgents

Retrieves all sales agents in the system.

**Authorization:** Admin

**Response:**
- 200: Returns the list of sales agents
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@reseller.com",
    "phone": "+1-555-0123",
    "resellerCompanyId": 123,
    "commissionRate": 0.10,
    "isActive": true,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/SalesAgents/active

Retrieves only active sales agents.

**Authorization:** Admin, Sales

**Response:**
- 200: Returns the list of active sales agents
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/SalesAgents/by-company/{resellerCompanyId}

Retrieves sales agents by reseller company.

**Authorization:** Admin, Sales

**Parameters:**
- `resellerCompanyId` (path): The reseller company ID

**Response:**
- 200: Returns the list of sales agents
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/SalesAgents/{id}

Retrieves a specific sales agent by ID.

**Authorization:** Admin

**Parameters:**
- `id` (path): The sales agent ID

**Response:**
- 200: Returns the sales agent
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If sales agent is not found
- 500: If an internal server error occurs

### POST /api/v1/SalesAgents

Creates a new sales agent.

**Authorization:** Admin

**Request Body:**
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@reseller.com",
  "phone": "+1-555-0456",
  "resellerCompanyId": 123,
  "commissionRate": 0.08,
  "isActive": true
}
```

**Response:**
- 201: Returns the newly created sales agent
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/SalesAgents/{id}

Updates an existing sales agent.

**Authorization:** Admin

**Parameters:**
- `id` (path): The sales agent ID to update

**Request Body:**
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith.updated@reseller.com",
  "phone": "+1-555-0456",
  "resellerCompanyId": 123,
  "commissionRate": 0.09,
  "isActive": true
}
```

**Response:**
- 200: Returns the updated sales agent
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If sales agent is not found
- 500: If an internal server error occurs

### DELETE /api/v1/SalesAgents/{id}

Deletes a sales agent.

**Authorization:** Admin

**Parameters:**
- `id` (path): The sales agent ID to delete

**Response:**
- 204: If the sales agent was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If sales agent is not found
- 500: If an internal server error occurs

## Data Models

### SalesAgentDto

```json
{
  "id": 0,
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "phone": "string",
  "resellerCompanyId": 0,
  "commissionRate": 0.0,
  "isActive": true,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateSalesAgentDto

```json
{
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "phone": "string",
  "resellerCompanyId": 0,
  "commissionRate": 0.0,
  "isActive": true
}
```

### UpdateSalesAgentDto

```json
{
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "phone": "string",
  "resellerCompanyId": 0,
  "commissionRate": 0.0,
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

- **v1.0:** Initial release of SalesAgents API