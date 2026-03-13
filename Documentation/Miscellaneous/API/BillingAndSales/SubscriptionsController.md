# Subscriptions API Documentation

## Overview

Manages recurring billing subscriptions including creation, updates, cancellation, and billing operations

## Base URL

```
https://api.example.com/api/v1/Subscriptions
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Sales depending on the endpoint).

## Endpoints

### GET /api/v1/Subscriptions

Retrieves all subscriptions in the system.

**Authorization:** Admin, Support, Sales

**Response:**
- 200: Returns the list of subscriptions
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "customerId": 123,
    "serviceId": 456,
    "billingCycleId": 1,
    "customerPaymentMethodId": 789,
    "status": "Active",
    "startDate": "2023-01-01T00:00:00Z",
    "endDate": null,
    "nextBillingDate": "2023-02-01T00:00:00Z",
    "currentPeriodStart": "2023-01-01T00:00:00Z",
    "currentPeriodEnd": "2023-01-31T23:59:59Z",
    "amount": 29.99,
    "currencyCode": "EUR",
    "billingPeriodCount": 1,
    "billingPeriodUnit": "Months",
    "trialEndDate": null,
    "isInTrial": false,
    "retryCount": 0,
    "maxRetryAttempts": 3,
    "lastBillingAttempt": "2023-01-01T00:00:00Z",
    "lastSuccessfulBilling": "2023-01-01T00:00:00Z",
    "cancelledAt": null,
    "cancellationReason": "",
    "pausedAt": null,
    "pauseReason": "",
    "metadata": "{}",
    "notes": "",
    "quantity": 1,
    "sendEmailNotifications": true,
    "autoRetryFailedPayments": true,
    "createdAt": "2023-01-01T00:00:00Z",
    "updatedAt": "2023-01-01T00:00:00Z"
  }
]
```

### GET /api/v1/Subscriptions/customer/{customerId}

Retrieves all subscriptions for a specific customer.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `customerId` (path): The unique identifier of the customer

**Response:**
- 200: Returns the list of customer subscriptions
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/Subscriptions/status/{status}

Retrieves all subscriptions with a specific status.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `status` (path): The subscription status

**Response:**
- 200: Returns the list of subscriptions matching the status
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/Subscriptions/due

Retrieves subscriptions that are due for billing.

**Authorization:** Admin, Support

**Parameters:**
- `dueDate` (query, optional): Date to check for due subscriptions

**Response:**
- 200: Returns the list of due subscriptions
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/Subscriptions/{id}

Retrieves a specific subscription by its unique identifier.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the subscription

**Response:**
- 200: Returns the subscription data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If subscription is not found
- 500: If an internal server error occurs

### POST /api/v1/Subscriptions

Creates a new subscription.

**Authorization:** Admin, Sales

**Request Body:**
```json
{
  "customerId": 123,
  "serviceId": 456,
  "billingCycleId": 1,
  "customerPaymentMethodId": 789,
  "startDate": "2023-01-01T00:00:00Z",
  "amount": 29.99,
  "currencyCode": "EUR",
  "billingPeriodCount": 1,
  "billingPeriodUnit": "Months",
  "trialEndDate": null,
  "maxRetryAttempts": 3,
  "sendEmailNotifications": true,
  "autoRetryFailedPayments": true,
  "notes": ""
}
```

**Response:**
- 201: Returns the newly created subscription
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/Subscriptions/{id}

Updates an existing subscription.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The unique identifier of the subscription to update

**Request Body:**
```json
{
  "amount": 39.99,
  "billingCycleId": 2,
  "notes": "Updated pricing"
}
```

**Response:**
- 200: Returns the updated subscription
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If subscription is not found
- 500: If an internal server error occurs

### POST /api/v1/Subscriptions/{id}/cancel

Cancels a subscription.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The unique identifier of the subscription to cancel

**Request Body:**
```json
{
  "reason": "Customer requested cancellation",
  "effectiveDate": "2023-12-31T23:59:59Z"
}
```

**Response:**
- 200: Returns the cancelled subscription
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If subscription is not found
- 500: If an internal server error occurs

### POST /api/v1/Subscriptions/{id}/pause

Pauses a subscription.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The unique identifier of the subscription to pause

**Request Body:**
```json
{
  "reason": "Temporary hold",
  "resumeDate": "2023-06-01T00:00:00Z"
}
```

**Response:**
- 200: Returns the paused subscription
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If subscription is not found
- 500: If an internal server error occurs

### POST /api/v1/Subscriptions/{id}/resume

Resumes a paused subscription.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The unique identifier of the subscription to resume

**Response:**
- 200: Returns the resumed subscription
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If subscription is not found
- 500: If an internal server error occurs

### DELETE /api/v1/Subscriptions/{id}

Deletes a subscription.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the subscription to delete

**Response:**
- 204: If the subscription was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If subscription is not found
- 500: If an internal server error occurs

### POST /api/v1/Subscriptions/{id}/process-billing

Manually processes billing for a specific subscription.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the subscription to bill

**Response:**
- 200: If billing was processed successfully
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If subscription is not found
- 500: If an internal server error occurs or billing fails

[Back to Controllers Index](index.md)