# POST CreateRefundLossAudit

POST CreateRefundLossAudit

## Endpoint

```
POST /api/v1/refund-loss-audits
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateRefundLossAuditDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `RefundLossAuditDto` |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)
