# POST ChangePassword

Changes password for the currently authenticated user

## Endpoint

```
POST /api/v1/my-account/change-password
```

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | [ChangePasswordRequestDto](../dtos/change-password-request-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




