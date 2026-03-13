# POST Finalize

Finalizes tax calculation and persists an immutable order tax snapshot.

## Endpoint

```
POST /api/v1/tax/finalize
```

## Authorization

Requires authentication. Policy: **TaxCalculation.Finalize**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `TaxQuoteRequestDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `TaxQuoteResultDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)
