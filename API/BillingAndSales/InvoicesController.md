# Invoices API Documentation

## Overview

The Invoices API provides endpoints for managing customer invoices in the system. It allows administrators, support staff, and sales personnel to create, retrieve, update, and delete invoice records.

## Base URL

```
https://api.example.com/api/v1/Invoices
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Sales depending on the endpoint).

## Endpoints

### GET /api/v1/Invoices

Retrieves all invoices in the system.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of invoices
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "invoiceNumber": "INV-001",
    "customerId": 123,
    "status": "Unpaid",
    "totalAmount": 99.99,
    "dueDate": "2023-02-15T00:00:00Z",
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/Invoices/customer/{customerId}

Retrieves all invoices for a specific customer.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `customerId` (path): The unique identifier of the customer

**Response:**
- 200: Returns the list of customer invoices
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/Invoices/status/{status}

Retrieves all invoices with a specific status.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `status` (path): The invoice status (Unpaid, Paid, Overdue, Cancelled)

**Response:**
- 200: Returns the list of invoices matching the status
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/Invoices/{id}

Retrieves a specific invoice by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the invoice

**Response:**
- 200: Returns the invoice data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If invoice is not found
- 500: If an internal server error occurs

### GET /api/v1/Invoices/number/{invoiceNumber}

Retrieves a specific invoice by its invoice number.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `invoiceNumber` (path): The invoice number

**Response:**
- 200: Returns the invoice data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If invoice is not found
- 500: If an internal server error occurs

### POST /api/v1/Invoices

Creates a new invoice in the system.

**Authorization:** Admin, Sales

**Request Body:**
```json
{
  "customerId": 123,
  "billingCycleId": 456,
  "dueDate": "2023-02-15T00:00:00Z",
  "notes": "Invoice notes",
  "invoiceLines": [
    {
      "description": "Domain Registration",
      "quantity": 1,
      "unitPrice": 15.99,
      "taxRate": 0.1
    }
  ]
}
```

**Response:**
- 201: Returns the newly created invoice
- 400: If the invoice data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/Invoices/{id}

Updates an existing invoice's information.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The unique identifier of the invoice to update

**Request Body:**
```json
{
  "status": "Paid",
  "notes": "Updated notes"
}
```

**Response:**
- 200: Returns the updated invoice
- 400: If the invoice data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If invoice is not found
- 500: If an internal server error occurs

### DELETE /api/v1/Invoices/{id}

Delete an invoice (soft delete).

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the invoice to delete

**Response:**
- 204: If invoice was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If invoice is not found
- 500: If an internal server error occurs

## Data Models

### InvoiceDto

```json
{
  "id": 0,
  "invoiceNumber": "string",
  "customerId": 0,
  "status": "string",
  "totalAmount": 0.0,
  "dueDate": "2023-01-15T10:30:00Z",
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z",
  "invoiceLines": [
    {
      "id": 0,
      "description": "string",
      "quantity": 0,
      "unitPrice": 0.0,
      "taxRate": 0.0,
      "total": 0.0
    }
  ]
}
```

### CreateInvoiceDto

```json
{
  "customerId": 0,
  "billingCycleId": 0,
  "dueDate": "2023-01-15T10:30:00Z",
  "notes": "string",
  "invoiceLines": [
    {
      "description": "string",
      "quantity": 0,
      "unitPrice": 0.0,
      "taxRate": 0.0
    }
  ]
}
```

### UpdateInvoiceDto

```json
{
  "status": "string",
  "notes": "string"
}
```

## Invoice Statuses

- **Unpaid**: Invoice has been issued but not paid
- **Paid**: Invoice has been paid in full
- **Overdue**: Invoice payment is past due date
- **Cancelled**: Invoice has been cancelled

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

- **v1.0:** Initial release of Invoices API