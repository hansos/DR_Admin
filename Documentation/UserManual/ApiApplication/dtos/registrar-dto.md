# RegistrarDto

Data transfer object representing a domain registrar

## Source

`DR_Admin/DTOs/RegistrarDto.cs`

## TypeScript Interface

```ts
export interface RegistrarDto {
  id: number;
  name: string;
  code: string;
  isActive: boolean;
  contactEmail: string | null;
  contactPhone: string | null;
  website: string | null;
  notes: string | null;
  isDefault: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `Code` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `ContactEmail` | `string?` | `string | null` |
| `ContactPhone` | `string?` | `string | null` |
| `Website` | `string?` | `string | null` |
| `Notes` | `string?` | `string | null` |
| `IsDefault` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

