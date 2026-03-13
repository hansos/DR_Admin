# POST AddMessage

Adds a message to a support ticket conversation.

## Endpoint

```
POST /api/v1/support-tickets/{id}/messages
```

## Authorization

Requires authentication. Policy: **SupportTicket.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `dto` | Body | [CreateSupportTicketMessageDto](../dtos/create-support-ticket-message-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [SupportTicketDto](../dtos/support-ticket-dto.md) |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




