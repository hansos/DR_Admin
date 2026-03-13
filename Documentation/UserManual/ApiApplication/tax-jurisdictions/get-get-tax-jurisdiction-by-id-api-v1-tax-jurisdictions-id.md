# GET GetTaxJurisdictionById

Manages tax jurisdictions used by VAT and TAX calculation.

## Endpoint

```
GET /api/v1/tax-jurisdictions/{id}
```

## Authorization

Requires authentication. Policy: **TaxJurisdiction.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [TaxJurisdictionDto](../dtos/tax-jurisdiction-dto.md) |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)




