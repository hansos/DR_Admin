# Quick Start: API Usage
Bootstrap Handover (Client programmers)

This document describes what must be entered up to and including first admin user registration, and which endpoints are used.

## Base URL

Use your environment API host + the paths below, for example:
`https://your-api-host/api/v1/...`

## 1) Check initialization status

**Endpoint**
- `GET /api/v1/Initialization/status` (open)

**Purpose**
- Verify whether the database is already initialized.

**Result**
- `isInitialized: true|false`

## 2) Register first admin user

**Endpoint**
- `POST /api/v1/Initialization/initialize-admin`

**Required request fields**
- `username`
- `password`
- `email`

**Optional request fields**
- `companyName`
- `companyEmail`
- `companyPhone`

**Important behavior**
- This endpoint only succeeds when no users exist yet.
- On success, the first admin account is created and assigned Admin role.

### DTO JSON examples

#### InitializationRequestDto (request body)

```json
{
  "username": "admin",
  "password": "Str0ng!Passw0rd",
  "email": "admin@example.com",
  "companyName": "Example Hosting AS",
  "companyEmail": "billing@example.com",
  "companyPhone": "+47 40 00 00 00"
}
```

#### InitializationResponseDto (success response)

```json
{
  "success": true,
  "message": "System initialized successfully with admin user.",
  "username": "admin"
}
```

#### InitializationStatusDto (status response)

```json
{
  "isInitialized": false
}
```

## 3) Automatically created during initialization

When `initialize-admin` succeeds, the system also ensures key code tables and defaults exist, including:
- Roles
- Customer statuses
- DNS record types
- Service types
- Payment instruments
- Address types
- System settings

If company fields are sent, a `MyCompany` record is also created.

## 4) Country table requirement

Country setup is **not required** to complete first admin bootstrap.

`Country` is not seeded by the admin bootstrap flow. If needed later, populate it after login via country management endpoints (authorized), e.g. CSV upload endpoints.

## 5) Useful open GET endpoints (no authentication)

- `/api/v1/Initialization/status`
- `/api/v1/System/health`
- `/api/v1/System/uptime`

## Related index

- [Documentation Index (Miscellaneous)](./Miscellaneous/Index.md)
