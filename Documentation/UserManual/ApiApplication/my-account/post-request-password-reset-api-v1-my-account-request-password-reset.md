# POST RequestPasswordReset

Requests a password reset by sending an email with a reset token

## Endpoint

```
POST /api/v1/my-account/request-password-reset
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | [RequestPasswordResetDto](../dtos/request-password-reset-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




