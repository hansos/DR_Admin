# Quotes API Documentation

## Overview

The Quotes API provides endpoints for managing sales quotes and proposals in the system. It allows administrators, sales personnel, and support staff to create, retrieve, update, send, accept, reject, and convert quotes to orders.

## Base URL

```
https://api.example.com/api/v1/Quotes
```

## Authentication

Most endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Sales, or Support depending on the endpoint). The quote acceptance endpoint is public for customers.

## Endpoints

### GET /api/v1/Quotes

Retrieves all quotes in the system.

**Authorization:** Admin, Sales, Support

**Response:**
- 200: Returns the list of quotes
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "quoteNumber": "Q-001",
    "customerId": 123,
    "status": "Draft",
    "totalAmount": 199.99,
    "validUntil": "2023-02-15T00:00:00Z",
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/Quotes/{id}

Retrieves a specific quote by ID.

**Authorization:** Admin, Sales, Support

**Parameters:**
- `id` (path): The quote ID

**Response:**
- 200: Returns the quote
- 404: If the quote is not found
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/Quotes/customer/{customerId}

Retrieves all quotes for a specific customer.

**Authorization:** Admin, Sales, Support

**Parameters:**
- `customerId` (path): The customer ID

**Response:**
- 200: Returns the list of quotes
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/Quotes/status/{status}

Retrieves quotes filtered by status.

**Authorization:** Admin, Sales, Support

**Parameters:**
- `status` (path): The quote status (Draft, Sent, Accepted, Rejected, Expired)

**Response:**
- 200: Returns the list of quotes
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### POST /api/v1/Quotes

Creates a new quote.

**Authorization:** Admin, Sales

**Request Body:**
```json
{
  "customerId": 123,
  "quoteLines": [
    {
      "serviceId": 456,
      "description": "Domain Registration",
      "quantity": 1,
      "unitPrice": 15.99,
      "discount": 0
    }
  ],
  "validDays": 30,
  "notes": "Quote notes"
}
```

**Response:**
- 201: Returns the newly created quote
- 400: If the quote data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/Quotes/{id}

Updates an existing quote.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The quote ID

**Request Body:**
```json
{
  "quoteLines": [
    {
      "serviceId": 456,
      "description": "Updated description",
      "quantity": 2,
      "unitPrice": 15.99,
      "discount": 5.00
    }
  ],
  "notes": "Updated notes"
}
```

**Response:**
- 200: Returns the updated quote
- 400: If the quote data is invalid
- 404: If the quote is not found
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### DELETE /api/v1/Quotes/{id}

Deletes a quote (soft delete).

**Authorization:** Admin

**Parameters:**
- `id` (path): The quote ID

**Response:**
- 204: If the quote was successfully deleted
- 404: If the quote is not found
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### POST /api/v1/Quotes/{id}/send

Sends a quote to the customer via email.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The quote ID

**Response:**
- 200: If the quote was successfully sent
- 404: If the quote is not found
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### POST /api/v1/Quotes/accept/{token}

Accepts a quote using the acceptance token (public endpoint for customers).

**Authorization:** None (public endpoint)

**Parameters:**
- `token` (path): The acceptance token

**Response:**
- 200: If the quote was successfully accepted
- 404: If the quote is not found or token is invalid
- 500: If an internal server error occurs

### POST /api/v1/Quotes/{id}/reject

Rejects a quote.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The quote ID

**Request Body:**
```json
"Customer decided to go with another provider"
```

**Response:**
- 200: If the quote was successfully rejected
- 404: If the quote is not found
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### POST /api/v1/Quotes/{id}/convert

Converts a quote to an order.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The quote ID

**Response:**
- 200: Returns the created order ID
- 404: If the quote is not found
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "orderId": 789,
  "message": "Quote converted to order successfully"
}
```

### GET /api/v1/Quotes/{id}/pdf

Generates a PDF for the quote.

**Authorization:** Admin, Sales, Support

**Parameters:**
- `id` (path): The quote ID

**Response:**
- 200: Returns the PDF file
- 404: If the quote is not found
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

## Data Models

### QuoteDto

```json
{
  "id": 0,
  "quoteNumber": "string",
  "customerId": 0,
  "status": "string",
  "totalAmount": 0.0,
  "validUntil": "2023-01-15T10:30:00Z",
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z",
  "quoteLines": [
    {
      "id": 0,
      "serviceId": 0,
      "description": "string",
      "quantity": 0,
      "unitPrice": 0.0,
      "discount": 0.0,
      "total": 0.0
    }
  ]
}
```

### CreateQuoteDto

```json
{
  "customerId": 0,
  "quoteLines": [
    {
      "serviceId": 0,
      "description": "string",
      "quantity": 0,
      "unitPrice": 0.0,
      "discount": 0.0
    }
  ],
  "validDays": 0,
  "notes": "string"
}
```

### UpdateQuoteDto

```json
{
  "quoteLines": [
    {
      "serviceId": 0,
      "description": "string",
      "quantity": 0,
      "unitPrice": 0.0,
      "discount": 0.0
    }
  ],
  "notes": "string"
}
```

## Quote Statuses

- **Draft**: Quote is being prepared
- **Sent**: Quote has been sent to customer
- **Accepted**: Customer has accepted the quote
- **Rejected**: Customer has rejected the quote
- **Expired**: Quote validity period has expired

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

- **v1.0:** Initial release of Quotes API