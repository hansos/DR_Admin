# PATCH PatchEmail

Updates the email address for the currently authenticated user

## Endpoint

```
PATCH /api/v1/my-account/email
```

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `[PatchEmailRequestDto](../dtos/patch-email-request-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



