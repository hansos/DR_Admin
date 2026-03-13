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
| `approveDto` | Body | `[ApproveRefundLossDto](../dtos/approve-refund-loss-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[RefundLossAuditDto](../dtos/refund-loss-audit-dto.md)` |
| 400 | Bad Request | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)



