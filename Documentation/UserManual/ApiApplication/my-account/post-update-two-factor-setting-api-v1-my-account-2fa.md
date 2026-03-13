# POST UpdateTwoFactorSetting

Updates mail-based two-factor authentication setting for the authenticated user.

## Endpoint

```
POST /api/v1/my-account/2fa
```

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `UpdateTwoFactorSettingsRequestDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
