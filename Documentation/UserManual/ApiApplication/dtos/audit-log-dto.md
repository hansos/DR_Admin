# AuditLogDto

Data transfer object for AuditLogDto.

## Source

`DR_Admin/DTOs/AuditLogDto.cs`

## TypeScript Interface

```ts
export interface AuditLogDto {
  id: number;
  userId: number;
  actionType: string;
  entityType: string;
  entityId: number;
  timestamp: string;
  createdAt: string;
  updatedAt: string;
  details: string;
  iPAddress: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `UserId` | `int` | `number` |
| `ActionType` | `string` | `string` |
| `EntityType` | `string` | `string` |
| `EntityId` | `int` | `number` |
| `Timestamp` | `DateTime` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `Details` | `string` | `string` |
| `IPAddress` | `string` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

