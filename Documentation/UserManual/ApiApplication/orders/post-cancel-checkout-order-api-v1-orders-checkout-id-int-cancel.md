# POST CancelCheckoutOrder

Manages customer orders including creation, retrieval, updates, and deletion

## Endpoint

```
POST /api/v1/orders/checkout/{id:int}/cancel
```

## Authorization

Requires authentication. Policy: **Order.Checkout**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[OrderDto](../dtos/order-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 409 | Conflict | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



