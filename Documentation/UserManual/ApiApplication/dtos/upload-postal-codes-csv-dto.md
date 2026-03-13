# UploadPostalCodesCsvDto

Data transfer object for uploading postal codes CSV file

## Source

`DR_Admin/DTOs/PostalCodeDto.cs`

## TypeScript Interface

```ts
export interface UploadPostalCodesCsvDto {
  countryCode: string;
  file: IFormFile | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CountryCode` | `string` | `string` |
| `File` | `IFormFile?` | `IFormFile | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
