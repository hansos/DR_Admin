# POST CheckStatuses

Manually runs subscription status checks against configured payment gateways.

## Endpoint

```
POST /api/v1/subscriptions/check-statuses
```

## Authorization

Requires authentication. Policy: **Subscription.Write**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
