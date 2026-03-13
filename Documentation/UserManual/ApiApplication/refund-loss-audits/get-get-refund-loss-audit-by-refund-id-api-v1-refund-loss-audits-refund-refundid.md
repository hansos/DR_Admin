# GET GetRefundLossAuditByRefundId

GET GetRefundLossAuditByRefundId

## Endpoint

```
GET /api/v1/refund-loss-audits/refund/{refundId}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `refundId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `RefundLossAuditDto` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
