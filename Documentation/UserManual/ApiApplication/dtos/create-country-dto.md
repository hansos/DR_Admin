# CreateCountryDto

Data transfer object for creating a new country

## Source

`DR_Admin/DTOs/CountryDto.cs`

## TypeScript Interface

```ts
export interface CreateCountryDto {
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

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
