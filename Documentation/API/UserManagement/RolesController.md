# RolesController

## Overview

Manages user roles including creation, retrieval, updates, and deletion

## Endpoints

### GET /api/v1/roles

**Description:** Retrieves all roles in the system

**Authorization:** Admin role required

**Responses:**
- 200: Returns the list of roles
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### GET /api/v1/roles/{id}

**Description:** Retrieves a specific role by its unique identifier

**Parameters:**
- id (int): The unique identifier of the role

**Authorization:** Admin role required

**Responses:**
- 200: Returns the role data
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If role is not found
- 500: If an internal server error occurs

### POST /api/v1/roles

**Description:** Creates a new role in the system

**Request Body:** CreateRoleDto

**Authorization:** Admin role required

**Responses:**
- 201: Returns the newly created role
- 400: If the role data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 500: If an internal server error occurs

### PUT /api/v1/roles/{id}

**Description:** Updates an existing role's information

**Parameters:**
- id (int): The unique identifier of the role to update

**Request Body:** UpdateRoleDto

**Authorization:** Admin role required

**Responses:**
- 200: Returns the updated role
- 400: If the role data is invalid
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If role is not found
- 500: If an internal server error occurs

### DELETE /api/v1/roles/{id}

**Description:** Deletes a role from the system

**Parameters:**
- id (int): The unique identifier of the role to delete

**Authorization:** Admin role required

**Responses:**
- 204: If role was successfully deleted
- 401: If user is not authenticated
- 403: If user doesn't have Admin role
- 404: If role is not found
- 500: If an internal server error occurs

[Back to User Management](index.md)