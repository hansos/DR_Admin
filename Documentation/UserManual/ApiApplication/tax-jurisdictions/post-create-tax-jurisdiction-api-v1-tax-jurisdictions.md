# POST CreateTaxJurisdiction

Creates a tax jurisdiction.

## Endpoint

```
POST /api/v1/tax-jurisdictions
```

## Authorization

Requires authentication. Policy: **TaxJurisdiction.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `CreateTaxJurisdictionDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `TaxJurisdictionDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)
