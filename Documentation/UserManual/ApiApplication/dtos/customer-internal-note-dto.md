# CustomerInternalNoteDto

Represents an internal customer note.

## Source

`DR_Admin/DTOs/CustomerInternalNoteDto.cs`

## TypeScript Interface

```ts
export interface CustomerInternalNoteDto {
  id: number;
  customerId: number;
  note: string;
  createdByUserId: number | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `Note` | `string` | `string` |
| `CreatedByUserId` | `int?` | `number | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [POST CreateInternalNote](../customers/post-create-internal-note-api-v1-customers-id-internal-notes.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

