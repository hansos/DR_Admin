# UsersController

## Overview

Manages user accounts including creation, retrieval, updates, and deletion

## Endpoints

### GET /api/v1/users

**Description:** Retrieves all users in the system

**Authorization:** Admin, Support roles required

**Responses:**
- 200: Returns the list of users
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 500: If an internal server error occurs

### GET /api/v1/users/{id}

**Description:** Retrieves a specific user by their unique identifier

**Parameters:**
- id (int): The unique identifier of the user

**Authorization:** Admin, Support roles required

**Responses:**
- 200: Returns the user data
- 401: If user is not authenticated
- 403: If user doesn't have required role
- 404: If user is not found
- 500: If an internal server error occurs

### POST /api/v1/users

**Description:** Creates a new user in the system

**Request Body:** CreateUserDto

**Authorization:** Admin role required

**Responses:**
- 201: Returns the newly created user
- 400: If the user data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/users/{id}

**Description:** Updates an existing user's information

**Parameters:**
- id (int): The unique identifier of the user to update

**Request Body:** UpdateUserDto

**Authorization:** Admin role required

**Responses:**
- 200: Returns the updated user
- 400: If the user data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If user is not found
- 500: If an internal server error occurs

### DELETE /api/v1/users/{id}

**Description:** Deletes a user from the system

**Parameters:**
- id (int): The unique identifier of the user to delete

**Authorization:** Admin role required

**Responses:**
- 204: If user was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If user is not found
- 500: If an internal server error occurs

[Back to User Management](index.md)