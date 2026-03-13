# POST RequestEmailConfirmation

Requests a new email confirmation link for the currently authenticated user.

## Endpoint

```
POST /api/v1/my-account/request-email-confirmation
```

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | [RequestEmailConfirmationDto](../dtos/request-email-confirmation-dto.md)? |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




