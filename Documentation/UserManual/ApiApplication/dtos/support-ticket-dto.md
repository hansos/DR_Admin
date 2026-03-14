# SupportTicketDto

Data transfer object representing a support ticket.

## Source

`DR_Admin/DTOs/SupportTicketDto.cs`

## TypeScript Interface

```ts
export interface SupportTicketDto {
  id: number;
  customerId: number;
  customerName: string;
  createdByUserId: number;
  createdByUsername: string;
  assignedToUserId: number | null;
  assignedToUsername: string | null;
  subject: string;
  status: string;
  priority: string;
  createdAt: string;
  updatedAt: string;
  lastMessageAt: string | null;
  closedAt: string | null;
  messages: SupportTicketMessageDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `CustomerName` | `string` | `string` |
| `CreatedByUserId` | `int` | `number` |
| `CreatedByUsername` | `string` | `string` |
| `AssignedToUserId` | `int?` | `number | null` |
| `AssignedToUsername` | `string?` | `string | null` |
| `Subject` | `string` | `string` |
| `Status` | `string` | `string` |
| `Priority` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `LastMessageAt` | `DateTime?` | `string | null` |
| `ClosedAt` | `DateTime?` | `string | null` |
| `Messages` | `List<SupportTicketMessageDto>` | `SupportTicketMessageDto[]` |

## Used By Endpoints

- [GET GetById](../support-tickets/get-get-by-id-api-v1-support-tickets-id.md)
- [GET GetTickets](../support-tickets/get-get-tickets-api-v1-support-tickets.md)
- [PATCH UpdateStatus](../support-tickets/patch-update-status-api-v1-support-tickets-id-status.md)
- [POST AddMessage](../support-tickets/post-add-message-api-v1-support-tickets-id-messages.md)
- [POST Create](../support-tickets/post-create-api-v1-support-tickets.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

