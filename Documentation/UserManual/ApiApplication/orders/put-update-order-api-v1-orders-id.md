# PUT UpdateOrder

Updates an existing order's information

## Endpoint

```
PUT /api/v1/orders/{id}
```

## Authorization

Requires authentication. Policy: **Order.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateOrderDto](../dtos/update-order-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [OrderDto](../dtos/order-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




