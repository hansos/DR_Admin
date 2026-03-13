# PUT Update

Updates an existing profit margin setting.

## Endpoint

```
PUT /api/v1/profit-margin-settings/{id:int}
```

## Authorization

Requires authentication. Policy: **ProfitMargin.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `dto` | Body | `[UpdateProfitMarginSettingDto](../dtos/update-profit-margin-setting-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[ProfitMarginSettingDto](../dtos/profit-margin-setting-dto.md)` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)



