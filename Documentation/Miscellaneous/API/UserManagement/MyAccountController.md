# MyAccountController

## Overview

Manages user account operations including registration, email confirmation, and password management

## Endpoints

### POST /api/v1/myaccount/register

**Description:** Registers a new account with user and customer information

**Request Body:** RegisterAccountRequestDto

**Responses:**
- 201: Returns the registration result with confirmation token
- 400: If the registration data is invalid or user already exists
- 500: If an internal server error occurs

### POST /api/v1/myaccount/confirm-email

**Description:** Confirms user email address using the confirmation token sent during registration

**Request Body:** ConfirmEmailRequestDto

**Responses:**
- 200: If email was confirmed successfully
- 400: If email or token is missing, invalid, or expired
- 500: If an internal server error occurs

### POST /api/v1/myaccount/set-password

**Description:** Sets password for a new account or after password reset using a token

**Request Body:** SetPasswordRequestDto

**Responses:**
- 200: If password was set successfully
- 400: If required fields are missing, passwords don't match, or token is invalid/expired
- 500: If an internal server error occurs

### POST /api/v1/myaccount/change-password

**Description:** Changes password for the currently authenticated user

**Request Body:** ChangePasswordRequestDto

**Authorization:** Required

**Responses:**
- 200: If password was changed successfully
- 400: If required fields are missing, passwords don't match, or current password is incorrect
- 401: If user is not authenticated
- 500: If an internal server error occurs

### PATCH /api/v1/myaccount/email

**Description:** Updates the email address for the currently authenticated user

**Request Body:** PatchEmailRequestDto

**Authorization:** Required

**Responses:**
- 200: If email was updated successfully
- 400: If required fields are missing, password is incorrect, or email is already in use
- 401: If user is not authenticated
- 500: If an internal server error occurs

### PATCH /api/v1/myaccount/customer

**Description:** Update customer information for authenticated user

**Request Body:** PatchCustomerInfoRequestDto

**Authorization:** Required

**Responses:**
- 200: Updated customer information
- 400: Bad request
- 401: Unauthorized
- 500: Internal server error

### GET /api/v1/myaccount/me

**Description:** Get current authenticated user's account information

**Authorization:** Required

**Responses:**
- 200: User account information including customer details
- 401: Unauthorized
- 404: Not found
- 500: Internal server error

[Back to User Management](index.md)