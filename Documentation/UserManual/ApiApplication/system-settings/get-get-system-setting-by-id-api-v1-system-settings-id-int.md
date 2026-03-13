# GET GetSystemSettingById

Manages system settings including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/system-settings/{id:int}
```

## Authorization

Requires authentication. Policy: **SystemSetting.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [SystemSettingDto](../dtos/system-setting-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




