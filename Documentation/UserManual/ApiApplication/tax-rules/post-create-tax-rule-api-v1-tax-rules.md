# POST CreateTaxRule

Retrieves tax rules by location

## Endpoint

```
POST /api/v1/tax-rules
```

## Authorization

Requires authentication. Policy: **TaxRule.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateTaxRuleDto](../dtos/create-tax-rule-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[TaxRuleDto](../dtos/tax-rule-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



