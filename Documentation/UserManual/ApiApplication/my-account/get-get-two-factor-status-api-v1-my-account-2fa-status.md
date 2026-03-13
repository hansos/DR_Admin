# GET GetTwoFactorStatus

Gets current mail-based two-factor authentication status for the authenticated user.

## Endpoint

```
GET /api/v1/my-account/2fa/status
```

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [TwoFactorStatusDto](../dtos/two-factor-status-dto.md) |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




