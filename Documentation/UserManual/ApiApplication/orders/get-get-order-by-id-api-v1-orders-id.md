# GET GetOrderById

Retrieves all orders in the system

## Endpoint

```
GET /api/v1/orders/{id}
```

## Authorization

Requires authentication. Policy: **Order.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [OrderDto](../dtos/order-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




