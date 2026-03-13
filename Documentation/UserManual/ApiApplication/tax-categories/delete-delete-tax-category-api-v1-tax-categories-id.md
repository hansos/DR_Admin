# DELETE DeleteTaxCategory

Deletes a tax category.

## Endpoint

```
DELETE /api/v1/tax-categories/{id}
```

## Authorization

Requires authentication. Policy: **TaxCategory.Delete**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
