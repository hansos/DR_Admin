# Coupons API Documentation

## Overview

Manages discount coupons

## Base URL

```
https://api.example.com/api/v1/Coupons
```

## Authentication

All endpoints require authentication using JWT tokens. Users must be authorized with appropriate roles (Admin or Sales depending on the endpoint).

## Endpoints

### GET /api/v1/Coupons

Retrieves all coupons in the system.

**Authorization:** Admin, Sales

**Response:**
- 200: Returns the list of coupons
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "code": "SAVE10",
    "name": "10% Off",
    "description": "10% discount on all services",
    "type": "Percentage",
    "value": 10.0,
    "appliesTo": "Total",
    "minimumAmount": null,
    "maximumDiscount": null,
    "validFrom": "2023-01-01T00:00:00Z",
    "validUntil": "2023-12-31T23:59:59Z",
    "maxUsages": null,
    "maxUsagesPerCustomer": null,
    "isActive": true,
    "usageCount": 0,
    "createdAt": "2023-01-01T00:00:00Z",
    "updatedAt": "2023-01-01T00:00:00Z"
  }
]
```

### GET /api/v1/Coupons/active

Retrieves all active coupons.

**Authorization:** Admin, Sales

**Response:**
- 200: Returns the list of active coupons
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
[
  {
    "id": 1,
    "code": "SAVE10",
    "name": "10% Off",
    "description": "10% discount on all services",
    "type": "Percentage",
    "value": 10.0,
    "appliesTo": "Total",
    "minimumAmount": null,
    "maximumDiscount": null,
    "validFrom": "2023-01-01T00:00:00Z",
    "validUntil": "2023-12-31T23:59:59Z",
    "maxUsages": null,
    "maxUsagesPerCustomer": null,
    "isActive": true,
    "usageCount": 0,
    "createdAt": "2023-01-01T00:00:00Z",
    "updatedAt": "2023-01-01T00:00:00Z"
  }
]
```

### GET /api/v1/Coupons/{id}

Retrieves a specific coupon by ID.

**Authorization:** Admin, Sales

**Parameters:**
- `id` (path): The coupon ID

**Response:**
- 200: Returns the coupon
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If the coupon is not found
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "code": "SAVE10",
  "name": "10% Off",
  "description": "10% discount on all services",
  "type": "Percentage",
  "value": 10.0,
  "appliesTo": "Total",
  "minimumAmount": null,
  "maximumDiscount": null,
  "validFrom": "2023-01-01T00:00:00Z",
  "validUntil": "2023-12-31T23:59:59Z",
  "maxUsages": null,
  "maxUsagesPerCustomer": null,
  "isActive": true,
  "usageCount": 0,
  "createdAt": "2023-01-01T00:00:00Z",
  "updatedAt": "2023-01-01T00:00:00Z"
}
```

### GET /api/v1/Coupons/code/{code}

Retrieves a coupon by code.

**Authorization:** None (authenticated users)

**Parameters:**
- `code` (path): The coupon code

**Response:**
- 200: Returns the coupon
- 401: If user is not authenticated
- 404: If the coupon is not found
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "code": "SAVE10",
  "name": "10% Off",
  "description": "10% discount on all services",
  "type": "Percentage",
  "value": 10.0,
  "appliesTo": "Total",
  "minimumAmount": null,
  "maximumDiscount": null,
  "validFrom": "2023-01-01T00:00:00Z",
  "validUntil": "2023-12-31T23:59:59Z",
  "maxUsages": null,
  "maxUsagesPerCustomer": null,
  "isActive": true,
  "usageCount": 0,
  "createdAt": "2023-01-01T00:00:00Z",
  "updatedAt": "2023-01-01T00:00:00Z"
}
```

### POST /api/v1/Coupons

Creates a new coupon.

**Authorization:** Admin

**Request Body:**
```json
{
  "code": "SAVE10",
  "name": "10% Off",
  "description": "10% discount on all services",
  "type": "Percentage",
  "value": 10.0,
  "appliesTo": "Total",
  "minimumAmount": null,
  "maximumDiscount": null,
  "validFrom": "2023-01-01T00:00:00Z",
  "validUntil": "2023-12-31T23:59:59Z",
  "maxUsages": null,
  "maxUsagesPerCustomer": null,
  "isActive": true,
  "allowedServiceTypeIds": null,
  "internalNotes": ""
}
```

**Response:**
- 201: Returns the newly created coupon
- 400: If the coupon data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "code": "SAVE10",
  "name": "10% Off",
  "description": "10% discount on all services",
  "type": "Percentage",
  "value": 10.0,
  "appliesTo": "Total",
  "minimumAmount": null,
  "maximumDiscount": null,
  "validFrom": "2023-01-01T00:00:00Z",
  "validUntil": "2023-12-31T23:59:59Z",
  "maxUsages": null,
  "maxUsagesPerCustomer": null,
  "isActive": true,
  "usageCount": 0,
  "createdAt": "2023-01-01T00:00:00Z",
  "updatedAt": "2023-01-01T00:00:00Z"
}
```

### PUT /api/v1/Coupons/{id}

Updates an existing coupon.

**Authorization:** Admin

**Parameters:**
- `id` (path): The coupon ID

**Request Body:**
```json
{
  "code": "SAVE10",
  "name": "10% Off",
  "description": "10% discount on all services",
  "type": "Percentage",
  "value": 10.0,
  "appliesTo": "Total",
  "minimumAmount": null,
  "maximumDiscount": null,
  "validFrom": "2023-01-01T00:00:00Z",
  "validUntil": "2023-12-31T23:59:59Z",
  "maxUsages": null,
  "maxUsagesPerCustomer": null,
  "isActive": true,
  "allowedServiceTypeIds": null,
  "internalNotes": ""
}
```

**Response:**
- 200: Returns the updated coupon
- 400: If the coupon data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If the coupon is not found
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "id": 1,
  "code": "SAVE10",
  "name": "10% Off",
  "description": "10% discount on all services",
  "type": "Percentage",
  "value": 10.0,
  "appliesTo": "Total",
  "minimumAmount": null,
  "maximumDiscount": null,
  "validFrom": "2023-01-01T00:00:00Z",
  "validUntil": "2023-12-31T23:59:59Z",
  "maxUsages": null,
  "maxUsagesPerCustomer": null,
  "isActive": true,
  "usageCount": 0,
  "createdAt": "2023-01-01T00:00:00Z",
  "updatedAt": "2023-01-01T00:00:00Z"
}
```

### DELETE /api/v1/Coupons/{id}

Deletes a coupon (soft delete).

**Authorization:** Admin

**Parameters:**
- `id` (path): The coupon ID

**Response:**
- 204: If the coupon was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If the coupon is not found
- 500: If an internal server error occurs

### POST /api/v1/Coupons/validate

Validates a coupon for an order.

**Authorization:** None (authenticated users)

**Request Body:**
```json
{
  "code": "SAVE10",
  "customerId": 123,
  "totalAmount": 100.0,
  "serviceTypeIds": [1, 2]
}
```

**Response:**
- 200: Returns the validation result
- 400: If the validation data is invalid
- 401: If user is not authenticated
- 500: If an internal server error occurs

**Response Body:**
```json
{
  "isValid": true,
  "message": "Coupon is valid",
  "discountAmount": 10.0,
  "coupon": {
    "id": 1,
    "code": "SAVE10",
    "name": "10% Off",
    "description": "10% discount on all services",
    "type": "Percentage",
    "value": 10.0,
    "appliesTo": "Total",
    "minimumAmount": null,
    "maximumDiscount": null,
    "validFrom": "2023-01-01T00:00:00Z",
    "validUntil": "2023-12-31T23:59:59Z",
    "maxUsages": null,
    "maxUsagesPerCustomer": null,
    "isActive": true,
    "usageCount": 0,
    "createdAt": "2023-01-01T00:00:00Z",
    "updatedAt": "2023-01-01T00:00:00Z"
  }
}
```

[Back to Controllers Index](index.md)