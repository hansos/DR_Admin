# TaxCategoryDto

Data transfer object representing a tax category.

## Source

`DR_Admin/DTOs/TaxCategoryDto.cs`

## TypeScript Interface

```ts
export interface TaxCategoryDto {
  id: number;
  code: string;
  name: string;
  countryCode: string;
  stateCode: string | null;
  description: string;
  isActive: boolean;
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
| `CountryCode` | `string` | `string` |
| `StateCode` | `string?` | `string | null` |
| `Description` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetTaxCategoryById](../tax-categories/get-get-tax-category-by-id-api-v1-tax-categories-id.md)
- [POST CreateTaxCategory](../tax-categories/post-create-tax-category-api-v1-tax-categories.md)
- [PUT UpdateTaxCategory](../tax-categories/put-update-tax-category-api-v1-tax-categories-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

