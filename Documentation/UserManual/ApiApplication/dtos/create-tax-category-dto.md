# CreateTaxCategoryDto

Data transfer object for creating a tax category.

## Source

`DR_Admin/DTOs/CreateTaxCategoryDto.cs`

## TypeScript Interface

```ts
export interface CreateTaxCategoryDto {
  code: string;
  name: string;
  countryCode: string;
  stateCode: string | null;
  description: string;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `CountryCode` | `string` | `string` |
| `StateCode` | `string?` | `string | null` |
| `Description` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
