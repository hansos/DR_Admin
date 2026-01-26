# SentEmails API Documentation

## Overview

The SentEmails API provides endpoints for managing sent email records in the system. It allows administrators and support staff to track, retrieve, and manage email communications sent to customers and users.

## Base URL

```
https://api.example.com/api/v1/SentEmails
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Sales depending on the endpoint).

## Endpoints

### GET /api/v1/SentEmails

Retrieves all sent email records in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of sent emails
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "subject": "Invoice #123",
    "body": "Email content...",
    "toEmail": "customer@example.com",
    "fromEmail": "noreply@company.com",
    "sentAt": "2023-01-15T10:30:00Z",
    "messageId": "abc123",
    "status": "Sent",
    "customerId": 456,
    "userId": 789,
    "relatedEntityType": "Invoice",
    "relatedEntityId": 123
  }
]
```

### GET /api/v1/SentEmails/by-customer/{customerId}

Retrieves sent emails for a specific customer.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `customerId` (path): The customer ID

**Response:**
- 200: Returns the list of sent emails
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/SentEmails/by-user/{userId}

Retrieves sent emails by a specific user.

**Authorization:** Admin, Support

**Parameters:**
- `userId` (path): The user ID

**Response:**
- 200: Returns the list of sent emails
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/SentEmails/by-entity/{entityType}/{entityId}

Retrieves sent emails related to a specific entity.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `entityType` (path): The entity type (e.g., Invoice, Order)
- `entityId` (path): The entity ID

**Response:**
- 200: Returns the list of sent emails
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/SentEmails/by-date-range

Retrieves sent emails within a date range.

**Authorization:** Admin, Support

**Query Parameters:**
- `startDate`: The start date (format: yyyy-MM-dd)
- `endDate`: The end date (format: yyyy-MM-dd)

**Response:**
- 200: Returns the list of sent emails
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/SentEmails/by-message-id/{messageId}

Retrieves a sent email by message ID.

**Authorization:** Admin, Support

**Parameters:**
- `messageId` (path): The message ID

**Response:**
- 200: Returns the sent email
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If sent email is not found
- 500: If an internal server error occurs

### GET /api/v1/SentEmails/{id}

Retrieves a specific sent email by ID.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The sent email ID

**Response:**
- 200: Returns the sent email
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If sent email is not found
- 500: If an internal server error occurs

### POST /api/v1/SentEmails

Creates a new sent email record.

**Authorization:** Admin, Support

**Request Body:**
```json
{
  "subject": "Email Subject",
  "body": "Email body content",
  "toEmail": "recipient@example.com",
  "fromEmail": "sender@company.com",
  "messageId": "unique-message-id",
  "status": "Sent",
  "customerId": 123,
  "userId": 456,
  "relatedEntityType": "Invoice",
  "relatedEntityId": 789
}
```

**Response:**
- 201: Returns the newly created sent email
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/SentEmails/{id}

Updates an existing sent email record.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The sent email ID to update

**Request Body:**
```json
{
  "subject": "Updated Subject",
  "body": "Updated body",
  "toEmail": "newrecipient@example.com",
  "status": "Delivered"
}
```

**Response:**
- 200: Returns the updated sent email
- 400: If the request data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If sent email is not found
- 500: If an internal server error occurs

### DELETE /api/v1/SentEmails/{id}

Deletes a sent email record.

**Authorization:** Admin

**Parameters:**
- `id` (path): The sent email ID to delete

**Response:**
- 204: If the sent email was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If sent email is not found
- 500: If an internal server error occurs

## Data Models

### SentEmailDto

```json
{
  "id": 0,
  "subject": "string",
  "body": "string",
  "toEmail": "string",
  "fromEmail": "string",
  "sentAt": "2023-01-15T10:30:00Z",
  "messageId": "string",
  "status": "string",
  "customerId": 0,
  "userId": 0,
  "relatedEntityType": "string",
  "relatedEntityId": 0
}
```

### CreateSentEmailDto

```json
{
  "subject": "string",
  "body": "string",
  "toEmail": "string",
  "fromEmail": "string",
  "messageId": "string",
  "status": "string",
  "customerId": 0,
  "userId": 0,
  "relatedEntityType": "string",
  "relatedEntityId": 0
}
```

### UpdateSentEmailDto

```json
{
  "subject": "string",
  "body": "string",
  "toEmail": "string",
  "fromEmail": "string",
  "status": "string"
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

- **v1.0:** Initial release of SentEmails API