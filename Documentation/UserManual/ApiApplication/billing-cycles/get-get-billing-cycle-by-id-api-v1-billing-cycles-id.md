# GET GetBillingCycleById

Manages billing cycles including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/billing-cycles/{id}
```

## Authorization

Requires authentication. Policy: **BillingCycle.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[BillingCycleDto](../dtos/billing-cycle-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



