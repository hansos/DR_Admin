# AuthController

## Overview

Handles authentication operations including login, logout, token refresh, and verification.

## Endpoints

### POST /api/v1/auth/login

**Description:** Login endpoint to get JWT token. Accepts both username and email address as identification.

**Request Body:**
- `username` (string): Username or email
- `password` (string): Password

**Responses:**
- 200: LoginResponseDto
- 400: Bad request
- 401: Invalid credentials

### POST /api/v1/auth/refresh

**Description:** Refresh access token using refresh token

**Request Body:**
- `refreshToken` (string)

**Responses:**
- 200: LoginResponseDto
- 400: Bad request
- 401: Invalid token

### POST /api/v1/auth/logout

**Description:** Logs out the current user by revoking their refresh token

**Request Body:**
- `refreshToken` (string)

**Responses:**
- 200: Success
- 400: Bad request
- 401: Unauthorized

### GET /api/v1/auth/verify

**Description:** Test endpoint to verify authentication is working

**Responses:**
- 200: User info
- 401: Unauthorized

[Back to Authentication](index.md)