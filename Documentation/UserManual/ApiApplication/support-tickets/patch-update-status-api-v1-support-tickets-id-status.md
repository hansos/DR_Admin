# PATCH UpdateStatus

Updates the status and assignee of a support ticket.

## Endpoint

```
PATCH /api/v1/support-tickets/{id}/status
```

## Authorization

Requires authentication. Policy: **SupportTicket.Manage**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `dto` | Body | `[UpdateSupportTicketStatusDto](../dtos/update-support-ticket-status-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[SupportTicketDto](../dtos/support-ticket-dto.md)` |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



