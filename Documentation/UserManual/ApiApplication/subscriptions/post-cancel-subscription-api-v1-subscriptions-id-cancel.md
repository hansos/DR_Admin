# POST CancelSubscription

Cancels a subscription

## Endpoint

```
POST /api/v1/subscriptions/{id}/cancel
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `cancelDto` | Body | `CancelSubscriptionDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `SubscriptionDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
