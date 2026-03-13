# POST ApproveRefundLoss

POST ApproveRefundLoss

## Endpoint

```
POST /api/v1/refund-loss-audits/approve
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `approveDto` | Body | `ApproveRefundLossDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `RefundLossAuditDto` |
| 400 | Bad Request | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
