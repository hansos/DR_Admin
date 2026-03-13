# PUT UpdateTaxJurisdiction

Updates a tax jurisdiction.

## Endpoint

```
PUT /api/v1/tax-jurisdictions/{id}
```

## Authorization

Requires authentication. Policy: **TaxJurisdiction.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `dto` | Body | `[UpdateTaxJurisdictionDto](../dtos/update-tax-jurisdiction-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[TaxJurisdictionDto](../dtos/tax-jurisdiction-dto.md)` |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)



