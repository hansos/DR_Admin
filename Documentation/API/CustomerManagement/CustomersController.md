# CustomersController

## Overview

Manages customer information including creation, retrieval, updates, and deletion

## Endpoints

### GET /api/v1/customers

**Description:** Retrieves all customers in the system

**Authorization:** Admin, Support, Sales roles required

**Responses:**
- 200: Returns the list of customers
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/customers/{id}

**Description:** Retrieves a specific customer by their unique identifier

**Parameters:**
- id (int): The unique identifier of the customer

**Authorization:** Admin, Support, Sales roles required

**Responses:**
- 200: Returns the customer data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If customer is not found
- 500: If an internal server error occurs

### POST /api/v1/customers

**Description:** Creates a new customer in the system

**Request Body:** CreateCustomerDto

**Authorization:** Admin, Support, Sales roles required

**Responses:**
- 201: Returns the newly created customer
- 400: If the customer data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### PUT /api/v1/customers/{id}

**Description:** Updates an existing customer's information

**Parameters:**
- id (int): The unique identifier of the customer to update

**Request Body:** UpdateCustomerDto

**Authorization:** Admin, Support, Sales roles required

**Responses:**
- 200: Returns the updated customer
- 400: If the customer data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If customer is not found
- 500: If an internal server error occurs

### DELETE /api/v1/customers/{id}

**Description:** Deletes a customer from the system

**Parameters:**
- id (int): The unique identifier of the customer to delete

**Authorization:** Admin, Support, Sales roles required

**Responses:**
- 204: If customer was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If customer is not found
- 500: If an internal server error occurs

[Back to Customer Management](index.md)