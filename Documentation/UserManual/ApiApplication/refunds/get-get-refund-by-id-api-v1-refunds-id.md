# GET GetRefundById

Manages payment refunds

## Endpoint

```
GET /api/v1/refunds/{id}
```

## Authorization

Requires authentication. Policy: **Refund.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [RefundDto](../dtos/refund-dto.md) |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




