# GET GetTickets

Provides support ticket endpoints for customers and support staff.

## Endpoint

```
GET /api/v1/support-tickets
```

## Authorization

Requires authentication. Policy: **SupportTicket.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `status` | Query | `string?` |
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | IEnumerable<[SupportTicketDto](../dtos/support-ticket-dto.md)> |
| 200 | OK | [PagedResult](../dtos/paged-result.md)<[SupportTicketDto](../dtos/support-ticket-dto.md)> |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




