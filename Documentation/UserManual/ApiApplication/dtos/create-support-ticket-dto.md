# CreateSupportTicketDto

Data transfer object used when creating a support ticket.

## Source

`DR_Admin/DTOs/SupportTicketDto.cs`

## TypeScript Interface

```ts
export interface CreateSupportTicketDto {
  subject: string;
  message: string;
  priority: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Subject` | `string` | `string` |
| `Message` | `string` | `string` |
| `Priority` | `string` | `string` |

## Used By Endpoints

- [POST Create](../support-tickets/post-create-api-v1-support-tickets.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

