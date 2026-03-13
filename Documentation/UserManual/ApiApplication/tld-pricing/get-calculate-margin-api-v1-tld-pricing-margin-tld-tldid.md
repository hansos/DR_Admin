# GET CalculateMargin

Calculates margin for a specific TLD and operation type

## Endpoint

```
GET /api/v1/tld-pricing/margin/tld/{tldId}
```

## Authorization

Requires authentication. Policy: **Pricing.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `tldId` | Route | `int` |
| `operationType` | Query | `string` |
| `registrarId` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[MarginAnalysisResult](../dtos/margin-analysis-result.md)` |

[Back to API Manual index](../index.md)



