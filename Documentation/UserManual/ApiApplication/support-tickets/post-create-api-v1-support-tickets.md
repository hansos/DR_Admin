# POST Create

Creates a new support ticket for the authenticated customer.

## Endpoint

```
POST /api/v1/support-tickets
```

## Authorization

Requires authentication. Policy: **SupportTicket.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `CreateSupportTicketDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `SupportTicketDto` |
| 400 | Bad Request | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
