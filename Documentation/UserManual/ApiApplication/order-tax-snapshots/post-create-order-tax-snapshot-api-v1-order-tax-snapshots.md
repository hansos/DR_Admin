# POST CreateOrderTaxSnapshot

Creates an order tax snapshot.

## Endpoint

```
POST /api/v1/order-tax-snapshots
```

## Authorization

Requires authentication. Policy: **OrderTaxSnapshot.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `CreateOrderTaxSnapshotDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `OrderTaxSnapshotDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)
