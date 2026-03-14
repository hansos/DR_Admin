# PostalCodeDto

Data transfer object representing a postal code with geographic information

## Source

`DR_Admin/DTOs/PostalCodeDto.cs`

## TypeScript Interface

```ts
export interface PostalCodeDto {
  id: number;
  code: string;
  countryCode: string;
  city: string;
  state: string | null;
  region: string | null;
  district: string | null;
  latitude: number | null;
  longitude: number | null;
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
| `CountryCode` | `string` | `string` |
| `City` | `string` | `string` |
| `State` | `string?` | `string | null` |
| `Region` | `string?` | `string | null` |
| `District` | `string?` | `string | null` |
| `Latitude` | `decimal?` | `number | null` |
| `Longitude` | `decimal?` | `number | null` |
| `IsActive` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetAllPostalCodes](../postal-codes/get-get-all-postal-codes-api-v1-postal-codes.md)
- [GET GetPostalCodeByCodeAndCountry](../postal-codes/get-get-postal-code-by-code-and-country-api-v1-postal-codes-code-country-countrycode.md)
- [GET GetPostalCodeById](../postal-codes/get-get-postal-code-by-id-api-v1-postal-codes-id.md)
- [GET LookupPostalCode](../postal-codes/get-lookup-postal-code-api-v1-postal-codes-lookup.md)
- [POST CreatePostalCode](../postal-codes/post-create-postal-code-api-v1-postal-codes.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

