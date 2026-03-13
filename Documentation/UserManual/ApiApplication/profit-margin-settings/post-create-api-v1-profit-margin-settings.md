# POST Create

Creates a new profit margin setting.

## Endpoint

```
POST /api/v1/profit-margin-settings
```

## Authorization

Requires authentication. Policy: **ProfitMargin.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `CreateProfitMarginSettingDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `ProfitMarginSettingDto` |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)
