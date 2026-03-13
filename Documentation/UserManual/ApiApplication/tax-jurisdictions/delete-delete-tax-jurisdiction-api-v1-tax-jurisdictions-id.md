# DELETE DeleteTaxJurisdiction

Deletes a tax jurisdiction.

## Endpoint

```
DELETE /api/v1/tax-jurisdictions/{id}
```

## Authorization

Requires authentication. Policy: **TaxJurisdiction.Delete**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)
