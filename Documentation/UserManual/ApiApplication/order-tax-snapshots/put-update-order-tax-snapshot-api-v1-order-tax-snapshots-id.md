# PUT UpdateOrderTaxSnapshot

Updates an order tax snapshot.

## Endpoint

```
PUT /api/v1/order-tax-snapshots/{id}
```

## Authorization

Requires authentication. Policy: **OrderTaxSnapshot.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `dto` | Body | [UpdateOrderTaxSnapshotDto](../dtos/update-order-tax-snapshot-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [OrderTaxSnapshotDto](../dtos/order-tax-snapshot-dto.md) |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)




