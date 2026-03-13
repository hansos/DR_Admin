# GET CalculateTax

Calculates tax for a customer and amount

## Endpoint

```
GET /api/v1/tax-rules/calculate
```

## Authorization

Requires authentication. Policy: **TaxRule.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Query | `int` |
| `amount` | Query | `decimal` |
| `isSetupFee` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `object` |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
