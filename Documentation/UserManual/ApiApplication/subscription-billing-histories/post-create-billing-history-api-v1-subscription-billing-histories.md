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
| `createDto` | Body | `[CreateSubscriptionBillingHistoryDto](../dtos/create-subscription-billing-history-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[SubscriptionBillingHistoryDto](../dtos/subscription-billing-history-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



