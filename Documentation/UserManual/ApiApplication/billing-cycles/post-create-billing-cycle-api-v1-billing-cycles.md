# POST CreateBillingCycle

Creates a new billing cycle in the system

## Endpoint

```
POST /api/v1/billing-cycles
```

## Authorization

Requires authentication. Policy: **BillingCycle.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateBillingCycleDto](../dtos/create-billing-cycle-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [BillingCycleDto](../dtos/billing-cycle-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




