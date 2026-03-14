# CommunicationThreadDto

Represents a communication thread summary for list views.

## Source

`DR_Admin/DTOs/CommunicationThreadDto.cs`

## TypeScript Interface

```ts
export interface CommunicationThreadDto {
  id: number;
  subject: string;
  customerId: number | null;
  userId: number | null;
  relatedEntityType: string | null;
  relatedEntityId: number | null;
  lastMessageAtUtc: string | null;
  status: string;
  unreadCount: number;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Subject` | `string` | `string` |
| `CustomerId` | `int?` | `number | null` |
| `UserId` | `int?` | `number | null` |
| `RelatedEntityType` | `string?` | `string | null` |
| `RelatedEntityId` | `int?` | `number | null` |
| `LastMessageAtUtc` | `DateTime?` | `string | null` |
| `Status` | `string` | `string` |
| `UnreadCount` | `int` | `number` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetThreads](../communication-threads/get-get-threads-api-v1-communication-threads.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
