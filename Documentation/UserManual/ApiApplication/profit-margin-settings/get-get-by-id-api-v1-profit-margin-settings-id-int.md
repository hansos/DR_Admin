# GET GetById

Manages profit margin settings for product classes.

## Endpoint

```
GET /api/v1/profit-margin-settings/{id:int}
```

## Authorization

Requires authentication. Policy: **ProfitMargin.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ProfitMarginSettingDto](../dtos/profit-margin-setting-dto.md) |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)




