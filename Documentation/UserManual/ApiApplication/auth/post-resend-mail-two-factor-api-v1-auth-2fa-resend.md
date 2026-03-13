# POST ResendMailTwoFactor

Resends a mail-based two-factor authentication code for an active challenge.

## Endpoint

```
POST /api/v1/auth/2fa/resend
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `[ResendMailTwoFactorRequestDto](../dtos/resend-mail-two-factor-request-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |

[Back to API Manual index](../index.md)



