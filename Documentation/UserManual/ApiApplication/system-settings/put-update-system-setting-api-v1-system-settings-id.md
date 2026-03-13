# PUT UpdateSystemSetting

Updates an existing system setting

## Endpoint

```
PUT /api/v1/system-settings/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateSystemSettingDto](../dtos/update-system-setting-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [SystemSettingDto](../dtos/system-setting-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




