# GET GetTaxCategoryById

Manages tax categories by country and optional state.

## Endpoint

```
GET /api/v1/tax-categories/{id}
```

## Authorization

Requires authentication. Policy: **TaxCategory.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `TaxCategoryDto` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
