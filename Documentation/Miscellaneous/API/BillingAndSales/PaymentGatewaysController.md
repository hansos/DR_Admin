# PaymentGateways API Documentation

## Overview

Manages payment gateway configurations including creation, retrieval, updates, and deletion

## Base URL

```
https://api.example.com/api/v1/PaymentGateways
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin or Support depending on the endpoint).

## Endpoints

### GET /api/v1/PaymentGateways

Retrieves all payment gateways in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of payment gateways
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "Stripe",
    "providerCode": "stripe",
    "isActive": true,
    "isDefault": true,
    "apiKey": "sk_test_...",
    "useSandbox": true,
    "webhookUrl": "https://api.example.com/webhooks/stripe",
    "displayOrder": 1,
    "description": "Stripe payment gateway",
    "logoUrl": "/images/stripe-logo.png",
    "supportedCurrencies": "USD,EUR,GBP",
    "feePercentage": 2.9,
    "fixedFee": 0.30,
    "notes": "",
    "createdAt": "2023-01-01T00:00:00Z",
    "updatedAt": "2023-01-01T00:00:00Z",
    "deletedAt": null
  }
]
```

### GET /api/v1/PaymentGateways/active

Retrieves all active payment gateways.

**Authorization:** None (authenticated users)

**Response:**
- 200: Returns the list of active payment gateways
- 401: If user is not authenticated
- 500: If an internal server error occurs

### GET /api/v1/PaymentGateways/default

Retrieves the default payment gateway.

**Authorization:** None (authenticated users)

**Response:**
- 200: Returns the default payment gateway
- 401: If user is not authenticated
- 404: If no default payment gateway is found
- 500: If an internal server error occurs

### GET /api/v1/PaymentGateways/{id}

Retrieves a specific payment gateway by its unique identifier.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The unique identifier of the payment gateway

**Response:**
- 200: Returns the payment gateway data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If payment gateway is not found
- 500: If an internal server error occurs

### GET /api/v1/PaymentGateways/provider/{providerCode}

Retrieves a payment gateway by provider code.

**Authorization:** None (authenticated users)

**Parameters:**
- `providerCode` (path): The provider code (stripe, paypal, square)

**Response:**
- 200: Returns the payment gateway data
- 401: If user is not authenticated
- 404: If payment gateway is not found
- 500: If an internal server error occurs

### POST /api/v1/PaymentGateways

Creates a new payment gateway.

**Authorization:** Admin

**Request Body:**
```json
{
  "name": "Stripe",
  "providerCode": "stripe",
  "isActive": true,
  "isDefault": false,
  "apiKey": "sk_test_...",
  "useSandbox": true,
  "webhookUrl": "https://api.example.com/webhooks/stripe",
  "displayOrder": 1,
  "description": "Stripe payment gateway",
  "logoUrl": "/images/stripe-logo.png",
  "supportedCurrencies": "USD,EUR,GBP",
  "feePercentage": 2.9,
  "fixedFee": 0.30,
  "notes": ""
}
```

**Response:**
- 201: Returns the newly created payment gateway
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/PaymentGateways/{id}

Updates an existing payment gateway.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the payment gateway to update

**Request Body:**
```json
{
  "name": "Stripe",
  "providerCode": "stripe",
  "isActive": true,
  "isDefault": false,
  "apiKey": "sk_test_...",
  "useSandbox": true,
  "webhookUrl": "https://api.example.com/webhooks/stripe",
  "displayOrder": 1,
  "description": "Stripe payment gateway",
  "logoUrl": "/images/stripe-logo.png",
  "supportedCurrencies": "USD,EUR,GBP",
  "feePercentage": 2.9,
  "fixedFee": 0.30,
  "notes": ""
}
```

**Response:**
- 200: Returns the updated payment gateway
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If payment gateway is not found
- 500: If an internal server error occurs

### POST /api/v1/PaymentGateways/{id}/set-default

Sets a payment gateway as the default.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the payment gateway

**Response:**
- 200: If the payment gateway was set as default successfully
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If payment gateway is not found
- 500: If an internal server error occurs

### POST /api/v1/PaymentGateways/{id}/set-active

Activates or deactivates a payment gateway.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the payment gateway

**Request Body:**
```json
true
```

**Response:**
- 200: If the payment gateway status was updated successfully
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If payment gateway is not found
- 500: If an internal server error occurs

### DELETE /api/v1/PaymentGateways/{id}

Deletes a payment gateway (soft delete).

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the payment gateway to delete

**Response:**
- 200: If the payment gateway was deleted successfully
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If payment gateway is not found
- 500: If an internal server error occurs

[Back to Controllers Index](index.md)