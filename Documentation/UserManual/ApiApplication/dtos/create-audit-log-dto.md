# CreateAuditLogDto

Data transfer object for CreateAuditLogDto.

## Source

`DR_Admin/DTOs/AuditLogDto.cs`

## TypeScript Interface

```ts
export interface CreateAuditLogDto {
  userId: number;
  actionType: string;
  entityType: string;
  entityId: number;
  details: string;
  iPAddress: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `UserId` | `int` | `number` |
| `ActionType` | `string` | `string` |
| `EntityType` | `string` | `string` |
| `EntityId` | `int` | `number` |
| `Details` | `string` | `string` |
| `IPAddress` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
