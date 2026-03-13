# POST CreateSystemSetting

Creates a new system setting

## Endpoint

```
POST /api/v1/system-settings
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateSystemSettingDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `SystemSettingDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
