# RegisteredDomainHistoryDto

Data transfer object representing a registered domain history entry.

## Source

`DR_Admin/DTOs/RegisteredDomainHistoryDto.cs`

## TypeScript Interface

```ts
export interface RegisteredDomainHistoryDto {
  id: number;
  registeredDomainId: number;
  domainName: string | null;
  actionType: RegisteredDomainHistoryActionType;
  action: string;
  details: string | null;
  occurredAt: string;
  sourceEntityType: string | null;
  sourceEntityId: number | null;
  performedByUserId: number | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `RegisteredDomainId` | `int` | `number` |
| `DomainName` | `string?` | `string | null` |
| `ActionType` | `RegisteredDomainHistoryActionType` | `RegisteredDomainHistoryActionType` |
| `Action` | `string` | `string` |
| `Details` | `string?` | `string | null` |
| `OccurredAt` | `DateTime` | `string` |
| `SourceEntityType` | `string?` | `string | null` |
| `SourceEntityId` | `int?` | `number | null` |
| `PerformedByUserId` | `int?` | `number | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
