# GET GetTaxRuleById

Manages tax rules for automatic tax calculation

## Endpoint

```
GET /api/v1/tax-rules/{id}
```

## Authorization

Requires authentication. Policy: **TaxRule.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [TaxRuleDto](../dtos/tax-rule-dto.md) |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




