# EmailQueue API Documentation

## Overview

The EmailQueue API provides endpoints for managing email queue operations for sending emails asynchronously. It allows administrators, support staff, and sales personnel to queue emails for sending and check their status.

## Base URL

```
https://api.example.com/api/v1/EmailQueue
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Sales).

## Endpoints

### POST /api/v1/EmailQueue/queue

Queues an email for asynchronous sending.

**Authorization:** Admin, Support, Sales

**Request Body:**
```json
{
  "to": "recipient@example.com",
  "cc": "cc@example.com",
  "bcc": "bcc@example.com",
  "subject": "Email Subject",
  "bodyText": "Plain text body",
  "bodyHtml": "<p>HTML body</p>",
  "customerId": 123,
  "userId": 456,
  "relatedEntityType": "Invoice",
  "relatedEntityId": 789,
  "attachments": [
    {
      "fileName": "document.pdf",
      "contentType": "application/pdf",
      "data": "base64encodeddata"
    }
  ]
}
```

**Response:**
- 201: Email queued successfully
- 400: Invalid request data
- 401: If user is not authenticated
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "status": "Queued",
  "message": "Email queued successfully"
}
```

### GET /api/v1/EmailQueue/status/{id}

Gets the status of a queued email.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): Email ID

**Response:**
- 200: Returns email status
- 404: Email not found
- 401: If user is not authenticated
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "subject": "Email Subject",
  "body": "Email body content",
  "toEmail": "recipient@example.com",
  "fromEmail": "noreply@company.com",
  "sentAt": "2023-01-15T10:30:00Z",
  "messageId": "abc123",
  "status": "Sent",
  "customerId": 123,
  "userId": 456,
  "relatedEntityType": "Invoice",
  "relatedEntityId": 789
}
```

## Data Models

### QueueEmailDto

```json
{
  "to": "string",
  "cc": "string",
  "bcc": "string",
  "subject": "string",
  "bodyText": "string",
  "bodyHtml": "string",
  "customerId": 0,
  "userId": 0,
  "relatedEntityType": "string",
  "relatedEntityId": 0,
  "attachments": [
    {
      "fileName": "string",
      "contentType": "string",
      "data": "string"
    }
  ]
}
```

### QueueEmailResponseDto

```json
{
  "id": 0,
  "status": "string",
  "message": "string"
}
```

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

## Email Statuses

- **Queued**: Email is queued for sending
- **Sending**: Email is being sent
- **Sent**: Email has been sent successfully
- **Failed**: Email sending failed
- **Retry**: Email will be retried

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

- **v1.0:** Initial release of EmailQueue API