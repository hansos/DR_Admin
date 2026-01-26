# OrdersController

## Overview

Manages customer orders including creation, retrieval, updates, and deletion

## Endpoints

### GET /api/v1/orders

**Description:** Retrieves all orders in the system

**Authorization:** Admin, Support, Sales roles required

**Responses:**
- 200: Returns the list of orders
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/orders/{id}

**Description:** Retrieves a specific order by its unique identifier

**Parameters:**
- id (int): The unique identifier of the order

**Authorization:** Admin, Support, Sales roles required

**Responses:**
- 200: Returns the order data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If order is not found
- 500: If an internal server error occurs

### POST /api/v1/orders

**Description:** Creates a new order in the system

**Request Body:** CreateOrderDto

**Authorization:** Admin, Sales roles required

**Responses:**
- 201: Returns the newly created order
- 400: If the order data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/orders/{id}

**Description:** Updates an existing order's information

**Parameters:**
- id (int): The unique identifier of the order to update

**Request Body:** UpdateOrderDto

**Authorization:** Admin, Sales roles required

**Responses:**
- 200: Returns the updated order
- 400: If the order data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If order is not found
- 500: If an internal server error occurs

### DELETE /api/v1/orders/{id}

**Description:** Deletes an order from the system

**Parameters:**
- id (int): The unique identifier of the order to delete

**Authorization:** Admin role required

**Responses:**
- 204: If order was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If order is not found
- 500: If an internal server error occurs

[Back to Billing and Sales](index.md)