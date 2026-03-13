# POST CreateCheckoutOrder

Creates a new checkout order for the currently authenticated user.

## Endpoint

```
POST /api/v1/orders/checkout
```

## Authorization

Requires authentication. Policy: **Order.Checkout**.

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
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



