# POST CreateTaxCategory

Creates a tax category.

## Endpoint

```
POST /api/v1/tax-categories
```

## Authorization

Requires authentication. Policy: **TaxCategory.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `CreateTaxCategoryDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `TaxCategoryDto` |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)
