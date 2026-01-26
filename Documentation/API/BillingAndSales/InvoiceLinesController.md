# InvoiceLines API Documentation

## Overview

The InvoiceLines API provides endpoints for managing invoice line items that represent individual charges on invoices. It allows administrators, support staff, and sales personnel to create, retrieve, update, and delete invoice line records.

## Base URL

```
https://api.example.com/api/v1/InvoiceLines
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Sales depending on the endpoint).

## Endpoints

### GET /api/v1/InvoiceLines

Retrieves all invoice lines in the system.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of invoice lines
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "invoiceId": 123,
    "description": "Domain Registration",
    "quantity": 1,
    "unitPrice": 15.99,
    "taxRate": 0.1,
    "total": 17.59
  }
]
```

### GET /api/v1/InvoiceLines/{id}

Retrieves a specific invoice line by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the invoice line

**Response:**
- 200: Returns the invoice line data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If invoice line is not found
- 500: If an internal server error occurs

### GET /api/v1/InvoiceLines/invoice/{invoiceId}

Retrieves all line items for a specific invoice.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `invoiceId` (path): The unique identifier of the invoice

**Response:**
- 200: Returns the list of invoice lines
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### POST /api/v1/InvoiceLines

Creates a new invoice line item.

**Authorization:** Admin, Sales

**Request Body:**
```json
{
  "invoiceId": 123,
  "description": "Domain Registration",
  "quantity": 1,
  "unitPrice": 15.99,
  "taxRate": 0.1
}
```

**Response:**
- 201: Returns the newly created invoice line
- 400: If the invoice line data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/InvoiceLines/{id}

Update an existing invoice line.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The unique identifier of the invoice line to update

**Request Body:**
```json
{
  "description": "Updated description",
  "quantity": 2,
  "unitPrice": 15.99,
  "taxRate": 0.1
}
```

**Response:**
- 200: Returns the updated invoice line
- 400: If the invoice line data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If invoice line is not found
- 500: If an internal server error occurs

### DELETE /api/v1/InvoiceLines/{id}

Delete an invoice line.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the invoice line to delete

**Response:**
- 204: If invoice line was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If invoice line is not found
- 500: If an internal server error occurs

## Data Models

### InvoiceLineDto

```json
{
  "id": 0,
  "invoiceId": 0,
  "description": "string",
  "quantity": 0,
  "unitPrice": 0.0,
  "taxRate": 0.0,
  "total": 0.0
}
```

### CreateInvoiceLineDto

```json
{
  "invoiceId": 0,
  "description": "string",
  "quantity": 0,
  "unitPrice": 0.0,
  "taxRate": 0.0
}
```

### UpdateInvoiceLineDto

```json
{
  "description": "string",
  "quantity": 0,
  "unitPrice": 0.0,
  "taxRate": 0.0
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

- **v1.0:** Initial release of InvoiceLines API