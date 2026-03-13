# GET GetByProductClass

Retrieves active profit margin setting for a product class.

## Endpoint

```
GET /api/v1/profit-margin-settings/by-class/{productClass}
```

## Authorization

Requires authentication. Policy: **ProfitMargin.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `productClass` | Route | `ProfitProductClass` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[ProfitMarginSettingDto](../dtos/profit-margin-setting-dto.md)` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)



