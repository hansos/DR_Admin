# SupportTicketMessageDto

Data transfer object representing a support ticket message.

## Source

`DR_Admin/DTOs/SupportTicketDto.cs`

## TypeScript Interface

```ts
export interface SupportTicketMessageDto {
  id: number;
  supportTicketId: number;
  senderUserId: number;
  senderUsername: string;
  senderRole: string;
  message: string;
  createdAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `SupportTicketId` | `int` | `number` |
| `SenderUserId` | `int` | `number` |
| `SenderUsername` | `string` | `string` |
| `SenderRole` | `string` | `string` |
| `Message` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

