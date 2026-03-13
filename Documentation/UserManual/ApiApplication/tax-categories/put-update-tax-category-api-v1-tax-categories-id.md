# PUT UpdateTaxCategory

Updates a tax category.

## Endpoint

```
PUT /api/v1/tax-categories/{id}
```

## Authorization

Requires authentication. Policy: **TaxCategory.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `dto` | Body | [UpdateTaxCategoryDto](../dtos/update-tax-category-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [TaxCategoryDto](../dtos/tax-category-dto.md) |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)




