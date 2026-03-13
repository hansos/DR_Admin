# POST ConfirmEmail

Confirms user email address using the confirmation token sent during registration

## Endpoint

```
POST /api/v1/my-account/confirm-email
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `[ConfirmEmailRequestDto](../dtos/confirm-email-request-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



