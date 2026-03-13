# PUT UpdateBillingCycle

Updates an existing billing cycle's information

## Endpoint

```
PUT /api/v1/billing-cycles/{id}
```

## Authorization

Requires authentication. Policy: **BillingCycle.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdateBillingCycleDto](../dtos/update-billing-cycle-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[BillingCycleDto](../dtos/billing-cycle-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



