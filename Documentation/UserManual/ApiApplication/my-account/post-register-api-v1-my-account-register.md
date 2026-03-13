# POST Register

Manages user account operations including registration, email confirmation, and password management

## Endpoint

```
POST /api/v1/my-account/register
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `[RegisterAccountRequestDto](../dtos/register-account-request-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[RegisterAccountResponseDto](../dtos/register-account-response-dto.md)` |
| 400 | Bad Request | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



