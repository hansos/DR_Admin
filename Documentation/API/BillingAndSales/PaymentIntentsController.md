# PaymentIntents API Documentation

## Overview

Manages payment intents for processing payments

## Base URL

```
https://api.example.com/api/v1/PaymentIntents
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles depending on the endpoint.

## Endpoints

### GET /api/v1/PaymentIntents

Retrieves all payment intents in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of payment intents
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "invoiceId": 123,
    "orderId": null,
    "customerId": 456,
    "amount": 99.99,
    "currency": "EUR",
    "status": "RequiresPaymentMethod",
    "paymentGatewayId": 1,
    "gatewayIntentId": "pi_123456789",
    "clientSecret": "pi_123456789_secret_...",
    "description": "Payment for invoice #123",
    "authorizedAt": null,
    "capturedAt": null,
    "failedAt": null,
    "failureReason": "",
    "createdAt": "2023-01-01T00:00:00Z"
  }
]
```

### GET /api/v1/PaymentIntents/{id}

Retrieves a specific payment intent by ID.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The payment intent ID

**Response:**
- 200: Returns the payment intent
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If the payment intent is not found
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "invoiceId": 123,
  "orderId": null,
  "customerId": 456,
  "amount": 99.99,
  "currency": "EUR",
  "status": "RequiresPaymentMethod",
  "paymentGatewayId": 1,
  "gatewayIntentId": "pi_123456789",
  "clientSecret": "pi_123456789_secret_...",
  "description": "Payment for invoice #123",
  "authorizedAt": null,
  "capturedAt": null,
  "failedAt": null,
  "failureReason": "",
  "createdAt": "2023-01-01T00:00:00Z"
}
```

### GET /api/v1/PaymentIntents/customer/{customerId}

Retrieves all payment intents for a specific customer.

**Authorization:** Admin, Support

**Parameters:**
- `customerId` (path): The customer ID

**Response:**
- 200: Returns the list of payment intents
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### POST /api/v1/PaymentIntents

Creates a new payment intent.

**Authorization:** None (authenticated users)

**Request Body:**
```json
{
  "invoiceId": 123,
  "orderId": null,
  "amount": 99.99,
  "currency": "EUR",
  "paymentGatewayId": 1,
  "returnUrl": "https://example.com/success",
  "cancelUrl": "https://example.com/cancel",
  "description": "Payment for invoice #123"
}
```

**Response:**
- 201: Returns the newly created payment intent
- 400: If the payment intent data is invalid
- 401: If user is not authenticated
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "invoiceId": 123,
  "orderId": null,
  "customerId": 456,
  "amount": 99.99,
  "currency": "EUR",
  "status": "RequiresPaymentMethod",
  "paymentGatewayId": 1,
  "gatewayIntentId": "pi_123456789",
  "clientSecret": "pi_123456789_secret_...",
  "description": "Payment for invoice #123",
  "authorizedAt": null,
  "capturedAt": null,
  "failedAt": null,
  "failureReason": "",
  "createdAt": "2023-01-01T00:00:00Z"
}
```

### POST /api/v1/PaymentIntents/{id}/confirm

Confirms a payment intent with a payment method.

**Authorization:** None (authenticated users)

**Parameters:**
- `id` (path): The payment intent ID

**Request Body:**
```json
"pm_123456789"
```

**Response:**
- 200: If the payment intent was confirmed successfully
- 401: If user is not authenticated
- 404: If the payment intent is not found
- 500: If an internal server error occurs

### POST /api/v1/PaymentIntents/{id}/cancel

Cancels a payment intent.

**Authorization:** None (authenticated users)

**Parameters:**
- `id` (path): The payment intent ID

**Response:**
- 200: If the payment intent was cancelled successfully
- 401: If user is not authenticated
- 404: If the payment intent is not found
- 500: If an internal server error occurs

### POST /api/v1/PaymentIntents/webhook/{gatewayId}

Processes a webhook from a payment gateway.

**Authorization:** None

**Parameters:**
- `gatewayId` (path): The payment gateway ID

**Request Body:**
```json
"webhook_payload_string"
```

**Response:**
- 200: If the webhook was processed successfully
- 400: If the payload is invalid
- 500: If an internal server error occurs

[Back to Controllers Index](index.md)