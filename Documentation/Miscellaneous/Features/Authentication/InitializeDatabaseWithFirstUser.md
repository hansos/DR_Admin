# Initialize database with the first user

This document describes how to initialize the application's database with the very first administrative user by using the `Initialization` controller.

## Authentication

- The `initialize` endpoint is intentionally exposed as anonymous to allow creating the first account when no users exist.
- The endpoint should only succeed when the database contains zero users. Subsequent calls will fail with `400` (or the implementation's chosen status) to avoid account takeover.

## High-level steps

1. Confirm the database has no users (inspect the `Users` table or call an internal diagnostics endpoint).
2. Prepare the initialization payload with `Username`, `Password`, and `Email`.
3. Call the `Initialization` controller `POST /api/v1/Initialization/initialize` endpoint with the payload.
4. Verify the response indicates success and sign in with the new account.
5. Disable or remove the initialization endpoint from production after use, or ensure it remains guarded by a server-side feature flag.

## API usage

Typical endpoint implemented in this project:

- `POST /api/v1/Initialization/initialize`

Request body (fields reflect `InitializationRequestDto`):

```json
{
  "username": "admin",
  "password": "S3cureP@ss!",
  "email": "admin@example.com"
}
```

Notes about fields:
- `username` (string) - required. Must be unique.
- `password` (string) - required. Service currently stores this value into the `PasswordHash` property but does not perform secure hashing in this implementation (see notes). Use a strong password.
- `email` (string) - required. Must be unique.

Possible responses (implementation-specific):

- `200 OK` - Initialization succeeded. Returns an `InitializationResponseDto` with `Success`, `Message`, and `Username`.
- `400 Bad Request` - Missing/invalid fields, duplicate username/email, or users already exist.
- `500 Internal Server Error` - Unexpected server error during initialization.

## Service behavior and implementation notes

Based on `InitializationService` in the codebase:

- The service first checks the `Users` table count; if any users exist the method returns failure to prevent further initialization.
- The service validates required fields and checks for duplicate username/email.
- If an `Admin` role does not exist the service creates it and persists it to the `Roles` table.
- The service creates a `User` entity and assigns the `Admin` role to that user by creating a `UserRole` entry.
- Important: the current implementation assigns `request.Password` directly to `User.PasswordHash` and includes a `// TODO: Hash this password properly` comment. In production you must ensure the password is hashed with a secure algorithm (PBKDF2, bcrypt, Argon2, etc.) before persisting.

## Example curl

```bash
curl -X POST "https://your-host/api/v1/Initialization/initialize" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "S3cureP@ss!",
    "email": "admin@example.com"
  }'
```

## Verification steps

1. Attempt to sign in with the new account via the normal authentication endpoint to obtain a JWT.
2. Call a protected endpoint (for example `GET /api/v1/Users/me`) to confirm the token and roles work.
3. Inspect the `Users`, `UserRoles`, and `Roles` tables to confirm the user and role were created.

## Notes and best practices

- Ensure the initialization endpoint is only callable when there are no users; otherwise attackers could create administrator accounts.
- Do not rely on this endpoint for routine admin account creation. Create administrative accounts through guarded admin tooling after initialization.
- Require a server-side feature flag or environment variable to enable the endpoint in non-development environments if desired.
- Rotate the initial user's credentials after first sign-in and create separate accounts for day-to-day admin operations.
- Log and audit the initialization call with requester IP and timestamp.

Refer to the `InitializationController`, `InitializationService`, and DTOs in the repository for exact request/response shapes and any future changes.
