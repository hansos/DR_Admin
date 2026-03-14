# UnitDto

Data transfer object representing a unit of measurement

## Source

`DR_Admin/DTOs/UnitDto.cs`

## TypeScript Interface

```ts
export interface UnitDto {
  id: number;
  code: string;
  name: string;
  description: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  deletedAt: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `DeletedAt` | `DateTime?` | `string | null` |

## Used By Endpoints

- [GET GetUnitByCode](../units/get-get-unit-by-code-api-v1-units-code-code.md)
- [GET GetUnitById](../units/get-get-unit-by-id-api-v1-units-id.md)
- [POST CreateUnit](../units/post-create-unit-api-v1-units.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

