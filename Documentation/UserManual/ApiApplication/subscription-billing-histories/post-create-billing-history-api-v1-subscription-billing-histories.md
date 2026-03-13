# POST CreateBillingHistory

Creates a new billing history record (typically used for manual entries)

## Endpoint

```
POST /api/v1/subscription-billing-histories
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateSubscriptionBillingHistoryDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `SubscriptionBillingHistoryDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
