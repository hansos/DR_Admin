# POST DeactivateExpiredRates

Deactivates all expired currency exchange rates

## Endpoint

```
POST /api/v1/currencies/rates/deactivate-expired
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `int` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
