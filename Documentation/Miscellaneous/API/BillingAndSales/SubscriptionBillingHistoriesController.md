# SubscriptionBillingHistories API Documentation

## Overview

Manages subscription billing history records for audit and tracking purposes

## Base URL

```
https://api.example.com/api/v1/SubscriptionBillingHistories
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Sales depending on the endpoint).

## Endpoints

### GET /api/v1/SubscriptionBillingHistories

Retrieves all subscription billing history records in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of billing histories
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "subscriptionId": 123,
    "invoiceId": 456,
    "paymentTransactionId": 789,
    "billingDate": "2023-01-01T00:00:00Z",
    "amountCharged": 29.99,
    "currencyCode": "EUR",
    "status": "Completed",
    "attemptCount": 1,
    "errorMessage": "",
    "periodStart": "2023-01-01T00:00:00Z",
    "periodEnd": "2023-01-31T23:59:59Z",
    "isAutomatic": true,
    "processedByUserId": null,
    "notes": "",
    "metadata": "{}",
    "createdAt": "2023-01-01T00:00:00Z",
    "updatedAt": "2023-01-01T00:00:00Z"
  }
]
```

### GET /api/v1/SubscriptionBillingHistories/subscription/{subscriptionId}

Retrieves all billing history records for a specific subscription.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `subscriptionId` (path): The unique identifier of the subscription

**Response:**
- 200: Returns the list of billing histories for the subscription
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/SubscriptionBillingHistories/{id}

Retrieves a specific billing history record by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the billing history record

**Response:**
- 200: Returns the billing history data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If billing history record is not found
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "subscriptionId": 123,
  "invoiceId": 456,
  "paymentTransactionId": 789,
  "billingDate": "2023-01-01T00:00:00Z",
  "amountCharged": 29.99,
  "currencyCode": "EUR",
  "status": "Completed",
  "attemptCount": 1,
  "errorMessage": "",
  "periodStart": "2023-01-01T00:00:00Z",
  "periodEnd": "2023-01-31T23:59:59Z",
  "isAutomatic": true,
  "processedByUserId": null,
  "notes": "",
  "metadata": "{}",
  "createdAt": "2023-01-01T00:00:00Z",
  "updatedAt": "2023-01-01T00:00:00Z"
}
```

### POST /api/v1/SubscriptionBillingHistories

Creates a new billing history record (typically used for manual entries).

**Authorization:** Admin

**Request Body:**
```json
{
  "subscriptionId": 123,
  "invoiceId": 456,
  "paymentTransactionId": 789,
  "billingDate": "2023-01-01T00:00:00Z",
  "amountCharged": 29.99,
  "currencyCode": "EUR",
  "status": "Completed",
  "attemptCount": 1,
  "errorMessage": "",
  "periodStart": "2023-01-01T00:00:00Z",
  "periodEnd": "2023-01-31T23:59:59Z",
  "isAutomatic": false,
  "processedByUserId": 1,
  "notes": "Manual billing entry",
  "metadata": "{}"
}
```

**Response:**
- 201: Returns the newly created billing history
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### DELETE /api/v1/SubscriptionBillingHistories/{id}

Deletes a billing history record.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the billing history record to delete

**Response:**
- 204: If the billing history was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If billing history record is not found
- 500: If an internal server error occurs

[Back to Controllers Index](index.md)