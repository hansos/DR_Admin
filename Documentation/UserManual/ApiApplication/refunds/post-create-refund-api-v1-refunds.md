# POST CreateRefund

Retrieves all refunds for a specific invoice

## Endpoint

```
POST /api/v1/refunds
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateRefundDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `RefundDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
