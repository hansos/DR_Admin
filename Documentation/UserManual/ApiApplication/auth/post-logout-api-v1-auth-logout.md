# POST Logout

Logs out the current user by revoking their refresh token

## Endpoint

```
POST /api/v1/auth/logout
```

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | [RefreshTokenRequestDto](../dtos/refresh-token-request-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |

[Back to API Manual index](../index.md)




