# GET GetRefundLossAuditById

Manages refund loss audits

## Endpoint

```
GET /api/v1/refund-loss-audits/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[RefundLossAuditDto](../dtos/refund-loss-audit-dto.md)` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)



