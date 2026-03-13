# POST ValidateVatNumber

Validates a VAT number (EU VIES check)

## Endpoint

```
POST /api/v1/tax-rules/validate-vat
```

## Authorization

Requires authentication. Policy: **TaxRule.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `vatNumber` | Query | `string` |
| `countryCode` | Query | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `object` |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
