# GET GetSystemSettingByKey

Retrieves a specific system setting by its key

## Endpoint

```
GET /api/v1/system-settings/key/{key}
```

## Authorization

Requires authentication. Policy: **SystemSetting.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `key` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[SystemSettingDto](../dtos/system-setting-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



