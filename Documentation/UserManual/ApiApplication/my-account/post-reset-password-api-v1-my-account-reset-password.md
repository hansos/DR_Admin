# POST ResetPassword

Resets password using password reset token (no email required)

## Endpoint

```
POST /api/v1/my-account/reset-password
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `[ResetPasswordWithTokenRequestDto](../dtos/reset-password-with-token-request-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



