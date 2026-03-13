# AddressTypeDto

Data transfer object representing an address type

## Source

`DR_Admin/DTOs/AddressTypeDto.cs`

## TypeScript Interface

```ts
export interface AddressTypeDto {
  id: number;
  code: string;
  name: string;
  description: string | null;
  isActive: boolean;
  isDefault: boolean;
  sortOrder: number;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `SortOrder` | `int` | `number` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
