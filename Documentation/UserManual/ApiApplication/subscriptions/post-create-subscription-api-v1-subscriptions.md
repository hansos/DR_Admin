# POST CreateSubscription

Creates a new subscription

## Endpoint

```
POST /api/v1/subscriptions
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateSubscriptionDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `SubscriptionDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
