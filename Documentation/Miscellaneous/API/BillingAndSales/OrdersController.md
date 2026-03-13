# Orders API Documentation

## Overview

The Orders API provides endpoints for managing customer orders in the system. It allows administrators, support staff, and sales personnel to create, retrieve, update, and delete order records.

## Base URL

```
https://api.example.com/api/v1/Orders
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Sales depending on the endpoint).

## Endpoints

### GET /api/v1/Orders

Retrieves all orders in the system.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of orders
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "orderNumber": "ORD-001",
    "customerId": 123,
    "status": "Pending",
    "totalAmount": 99.99,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/Orders/{id}

Retrieves a specific order by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the order

**Response:**
- 200: Returns the order data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If order is not found
- 500: If an internal server error occurs

### POST /api/v1/Orders

Creates a new order in the system.

**Authorization:** Admin, Sales

**Request Body:**
```json
{
  "customerId": 123,
  "orderLines": [
    {
      "serviceId": 456,
      "quantity": 1,
      "unitPrice": 99.99
    }
  ],
  "notes": "Order notes"
}
```

**Response:**
- 201: Returns the newly created order
- 400: If the order data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/Orders/{id}

Updates an existing order's information.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The unique identifier of the order to update

**Request Body:**
```json
{
  "status": "Completed",
  "notes": "Updated notes"
}
```

**Response:**
- 200: Returns the updated order
- 400: If the order data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If order is not found
- 500: If an internal server error occurs

### DELETE /api/v1/Orders/{id}

Deletes an order from the system.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the order to delete

**Response:**
- 204: If order was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If order is not found
- 500: If an internal server error occurs

## Data Models

### OrderDto

```json
{
  "id": 0,
  "orderNumber": "string",
  "customerId": 0,
  "status": "string",
  "totalAmount": 0.0,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateOrderDto

```json
{
  "customerId": 0,
  "orderLines": [
    {
      "serviceId": 0,
      "quantity": 0,
      "unitPrice": 0.0
    }
  ],
  "notes": "string"
}
```

### UpdateOrderDto

```json
{
  "status": "string",
  "notes": "string"
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

- **v1.0:** Initial release of Orders API