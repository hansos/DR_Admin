# POST SetPassword

Sets password for a new account or after password reset using a token

## Endpoint

```
POST /api/v1/my-account/set-password
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | [SetPasswordRequestDto](../dtos/set-password-request-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




