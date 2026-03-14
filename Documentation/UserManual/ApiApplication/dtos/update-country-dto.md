# UpdateCountryDto

Data transfer object for updating an existing country

## Source

`DR_Admin/DTOs/CountryDto.cs`

## TypeScript Interface

```ts
export interface UpdateCountryDto {
  code: string;
  tld: string;
  iso3: string | null;
  numeric: number | null;
  englishName: string;
  localName: string;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Code` | `string` | `string` |
| `Tld` | `string` | `string` |
| `Iso3` | `string?` | `string | null` |
| `Numeric` | `int?` | `number | null` |
| `EnglishName` | `string` | `string` |
| `LocalName` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |

## Used By Endpoints

- [PUT UpdateCountry](../countries/put-update-country-api-v1-countries-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

