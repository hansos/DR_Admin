# CountryDto

Data transfer object representing a country

## Source

`DR_Admin/DTOs/CountryDto.cs`

## TypeScript Interface

```ts
export interface CountryDto {
  id: number;
  code: string;
  tld: string;
  iso3: string | null;
  numeric: number | null;
  englishName: string;
  localName: string;
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
| `Tld` | `string` | `string` |
| `Iso3` | `string?` | `string | null` |
| `Numeric` | `int?` | `number | null` |
| `EnglishName` | `string` | `string` |
| `LocalName` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetActiveCountries](../countries/get-get-active-countries-api-v1-countries-active.md)
- [GET GetAllCountries](../countries/get-get-all-countries-api-v1-countries.md)
- [GET GetCountryByCode](../countries/get-get-country-by-code-api-v1-countries-code-code.md)
- [GET GetCountryById](../countries/get-get-country-by-id-api-v1-countries-id.md)
- [POST CreateCountry](../countries/post-create-country-api-v1-countries.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

