# GET GetBillingHistoryById

Manages subscription billing history records for audit and tracking purposes

## Endpoint

```
GET /api/v1/subscription-billing-histories/{id}
```

## Authorization

Requires authentication. Policy: **SubscriptionBillingHistory.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `SubscriptionBillingHistoryDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
