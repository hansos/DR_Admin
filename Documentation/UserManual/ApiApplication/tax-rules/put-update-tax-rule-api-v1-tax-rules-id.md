# PUT UpdateTaxRule

Updates an existing tax rule

## Endpoint

```
PUT /api/v1/tax-rules/{id}
```

## Authorization

Requires authentication. Policy: **TaxRule.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateTaxRuleDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `TaxRuleDto` |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
