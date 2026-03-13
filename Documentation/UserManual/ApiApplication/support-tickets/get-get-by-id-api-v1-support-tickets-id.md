# GET GetById

Retrieves one support ticket by identifier.

## Endpoint

```
GET /api/v1/support-tickets/{id}
```

## Authorization

Requires authentication. Policy: **SupportTicket.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [SupportTicketDto](../dtos/support-ticket-dto.md) |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




