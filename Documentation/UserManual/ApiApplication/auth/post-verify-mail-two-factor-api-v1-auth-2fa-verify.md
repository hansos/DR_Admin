# POST VerifyMailTwoFactor

Verifies a mail-based two-factor authentication code and issues JWT tokens.

## Endpoint

```
POST /api/v1/auth/2fa/verify
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `[VerifyMailTwoFactorRequestDto](../dtos/verify-mail-two-factor-request-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[LoginResponseDto](../dtos/login-response-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |

[Back to API Manual index](../index.md)



