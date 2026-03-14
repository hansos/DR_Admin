# ImportPostalCodesDto

Data transfer object for importing postal codes

## Source

`DR_Admin/DTOs/PostalCodeDto.cs`

## TypeScript Interface

```ts
export interface ImportPostalCodesDto {
  countryCode: string;
  postalCodes: ImportPostalCodeItemDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CountryCode` | `string` | `string` |
| `PostalCodes` | `List<ImportPostalCodeItemDto>` | `ImportPostalCodeItemDto[]` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

