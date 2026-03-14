# UpdatePostalCodeDto

Data transfer object for updating an existing postal code

## Source

`DR_Admin/DTOs/PostalCodeDto.cs`

## TypeScript Interface

```ts
export interface UpdatePostalCodeDto {
  code: string;
  countryCode: string;
  city: string;
  state: string | null;
  region: string | null;
  district: string | null;
  latitude: number | null;
  longitude: number | null;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Code` | `string` | `string` |
| `CountryCode` | `string` | `string` |
| `City` | `string` | `string` |
| `State` | `string?` | `string | null` |
| `Region` | `string?` | `string | null` |
| `District` | `string?` | `string | null` |
| `Latitude` | `decimal?` | `number | null` |
| `Longitude` | `decimal?` | `number | null` |
| `IsActive` | `bool` | `boolean` |

## Used By Endpoints

- [PUT UpdatePostalCode](../postal-codes/put-update-postal-code-api-v1-postal-codes-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

