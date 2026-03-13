# DELETE DeleteOrderTaxSnapshot

Deletes an order tax snapshot.

## Endpoint

```
DELETE /api/v1/order-tax-snapshots/{id}
```

## Authorization

Requires authentication. Policy: **OrderTaxSnapshot.Delete**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)
