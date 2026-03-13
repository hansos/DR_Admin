# DocumentTemplates API Documentation

## Overview

The DocumentTemplates API provides endpoints for managing document templates used for generating invoices, orders, emails, and other documents. It allows administrators to upload, manage, and retrieve document templates.

## Base URL

```
https://api.example.com/api/v1/DocumentTemplates
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin, Support, or Sales depending on the endpoint).

## Endpoints

### GET /api/v1/DocumentTemplates

Retrieves all document templates in the system.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of document templates
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "name": "Invoice Template",
    "description": "Standard invoice template",
    "type": "Invoice",
    "fileName": "invoice.docx",
    "mimeType": "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "isActive": true,
    "isDefault": true,
    "createdAt": "2023-01-15T10:30:00Z",
    "updatedAt": "2023-01-15T10:30:00Z"
  }
]
```

### GET /api/v1/DocumentTemplates/active

Retrieves all active document templates.

**Authorization:** Admin, Support

**Response:**
- 200: Returns the list of active document templates
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/DocumentTemplates/type/{type}

Retrieves document templates filtered by type.

**Authorization:** Admin, Support

**Parameters:**
- `type` (path): The template type (Invoice, Order, Email, etc.)

**Response:**
- 200: Returns the list of document templates matching the type
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/DocumentTemplates/{id}

Retrieves a specific document template by its unique identifier.

**Authorization:** Admin, Support

**Parameters:**
- `id` (path): The unique identifier of the document template

**Response:**
- 200: Returns the document template data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If document template is not found
- 500: If an internal server error occurs

### GET /api/v1/DocumentTemplates/default/{type}

Retrieves the default template for a specific type.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `type` (path): The template type

**Response:**
- 200: Returns the default document template
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If no default template exists for the type
- 500: If an internal server error occurs

### GET /api/v1/DocumentTemplates/{id}/download

Downloads the file content of a specific document template.

**Authorization:** Admin, Support, Sales

**Parameters:**
- `id` (path): The unique identifier of the document template

**Response:**
- 200: Returns the file content
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If document template is not found
- 500: If an internal server error occurs

### POST /api/v1/DocumentTemplates/upload

Uploads a new document template.

**Authorization:** Admin

**Request Body (Form Data):**
- `name`: Template name
- `description`: Template description
- `type`: Template type (Invoice, Order, Email, etc.)
- `file`: The template file

**Response:**
- 201: Returns the newly created document template
- 400: If the template data or file is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/DocumentTemplates/{id}

Updates an existing document template's information and optionally replaces the file.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the document template to update

**Request Body (Form Data):**
- `name`: Updated template name
- `description`: Updated template description
- `type`: Updated template type
- `file`: New template file (optional)

**Response:**
- 200: Returns the updated document template
- 400: If the template data or file is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If document template is not found
- 500: If an internal server error occurs

### PUT /api/v1/DocumentTemplates/{id}/set-default

Sets a document template as the default for its type.

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the document template to set as default

**Response:**
- 204: If the template was successfully set as default
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If document template is not found
- 500: If an internal server error occurs

### DELETE /api/v1/DocumentTemplates/{id}

Deletes a document template (soft delete).

**Authorization:** Admin

**Parameters:**
- `id` (path): The unique identifier of the document template to delete

**Response:**
- 204: If the template was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If document template is not found
- 500: If an internal server error occurs

## Data Models

### DocumentTemplateDto

```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "type": "string",
  "fileName": "string",
  "mimeType": "string",
  "isActive": true,
  "isDefault": false,
  "createdAt": "2023-01-15T10:30:00Z",
  "updatedAt": "2023-01-15T10:30:00Z"
}
```

### CreateDocumentTemplateDto

```json
{
  "name": "string",
  "description": "string",
  "type": "string",
  "file": "file"
}
```

### UpdateDocumentTemplateDto

```json
{
  "name": "string",
  "description": "string",
  "type": "string",
  "file": "file"
}
```

## Template Types

- **Invoice**: Templates for invoice documents
- **Order**: Templates for order documents
- **Email**: Templates for email content
- **Quote**: Templates for quote documents

## Supported File Formats

- Microsoft Word (.docx)
- PDF (.pdf)
- Other document formats as needed

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

- **v1.0:** Initial release of DocumentTemplates API