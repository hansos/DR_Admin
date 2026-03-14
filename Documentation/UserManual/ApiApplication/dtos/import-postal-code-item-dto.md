# ImportPostalCodeItemDto

Data transfer object for a postal code import item

## Source

`DR_Admin/DTOs/PostalCodeDto.cs`

## TypeScript Interface

```ts
export interface ImportPostalCodeItemDto {
  code: string;
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
| `City` | `string` | `string` |
| `State` | `string?` | `string | null` |
| `Region` | `string?` | `string | null` |
| `District` | `string?` | `string | null` |
| `Latitude` | `decimal?` | `number | null` |
| `Longitude` | `decimal?` | `number | null` |
| `IsActive` | `bool` | `boolean` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

