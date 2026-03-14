# CustomerChangeDto

Represents a tracked customer change.

## Source

`DR_Admin/DTOs/CustomerInternalNoteDto.cs`

## TypeScript Interface

```ts
export interface CustomerChangeDto {
  id: number;
  customerId: number;
  changeType: string;
  fieldName: string | null;
  oldValue: string | null;
  newValue: string | null;
  changedAt: string;
  changedByUserId: number | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `ChangeType` | `string` | `string` |
| `FieldName` | `string?` | `string | null` |
| `OldValue` | `string?` | `string | null` |
| `NewValue` | `string?` | `string | null` |
| `ChangedAt` | `DateTime` | `string` |
| `ChangedByUserId` | `int?` | `number | null` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

