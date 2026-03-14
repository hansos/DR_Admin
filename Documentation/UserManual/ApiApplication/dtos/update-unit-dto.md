# UpdateUnitDto

Data transfer object for updating an existing unit of measurement

## Source

`DR_Admin/DTOs/UnitDto.cs`

## TypeScript Interface

```ts
export interface UpdateUnitDto {
  code: string;
  name: string;
  description: string;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |

## Used By Endpoints

- [PUT UpdateUnit](../units/put-update-unit-api-v1-units-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

