# POST CreateOrder

Creates a new order in the system

## Endpoint

```
POST /api/v1/orders
```

## Authorization

Requires authentication. Policy: **Order.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateOrderDto](../dtos/create-order-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[OrderDto](../dtos/order-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



