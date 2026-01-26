# Refunds API Documentation

## Overview

The Refunds API provides endpoints for managing payment refunds in the system. It allows administrators and support staff to create, retrieve, and process refund records.

## Base URL

```
https://api.example.com/api/v1/Refunds
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin or Support depending on the endpoint).

## Endpoints

### GET /api/v1/Refunds

Retrieves all refunds in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of refunds
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "paymentTransactionId": 123,
    "amount": 50.00,
    "reason": "Customer request",
    "status": "Pending",
    "processedAt": null,
    "createdAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/Refunds/{id}

Retrieves a specific refund by ID.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The refund ID

**Response:**
- 200: Returns the refund
- 404: If the refund is not found
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/Refunds/invoice/{invoiceId}

Retrieves all refunds for a specific invoice.

**Authorization:** Admin, Support

**Parameters:**
- `invoiceId` (path): The invoice ID

**Response:**
- 200: Returns the list of refunds
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### POST /api/v1/Refunds

Creates a new refund.

**Authorization:** Admin

**Request Body:**
```json
{
  "paymentTransactionId": 123,
  "amount": 50.00,
  "reason": "Customer requested refund"
}
```

**Response:**
- 201: Returns the newly created refund
- 400: If the refund data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### POST /api/v1/Refunds/{id}/process

Processes a pending refund.

**Authorization:** Admin

**Parameters:**
- `id` (path): The refund ID

**Response:**
- 200: If the refund was successfully processed
- 404: If the refund is not found
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

## Data Models

### RefundDto

```json
{
  "id": 0,
  "paymentTransactionId": 0,
  "amount": 0.0,
  "reason": "string",
  "status": "string",
  "processedAt": "2023-01-15T10:30:00Z",
  "createdAt": "2023-01-15T10:30:00Z"
}
```

### CreateRefundDto

```json
{
  "paymentTransactionId": 0,
  "amount": 0.0,
  "reason": "string"
}
```

## Refund Statuses

- **Pending**: Refund has been requested but not yet processed
- **Processed**: Refund has been successfully processed
- **Failed**: Refund processing failed

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

- **v1.0:** Initial release of Refunds API