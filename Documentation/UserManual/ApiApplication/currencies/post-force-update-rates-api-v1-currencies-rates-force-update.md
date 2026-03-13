# POST ForceUpdateRates

Forces an immediate download and update of exchange rates from the configured provider.

## Endpoint

```
POST /api/v1/currencies/rates/force-update
```

## Authorization

Requires authentication. Policy: **Currency.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `cancellationToken` | Route | `CancellationToken` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `object` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
