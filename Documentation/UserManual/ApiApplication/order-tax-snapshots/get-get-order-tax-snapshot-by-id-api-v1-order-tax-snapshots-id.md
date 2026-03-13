# GET GetOrderTaxSnapshotById

Manages immutable order tax snapshots.

## Endpoint

```
GET /api/v1/order-tax-snapshots/{id}
```

## Authorization

Requires authentication. Policy: **OrderTaxSnapshot.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `OrderTaxSnapshotDto` |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)
