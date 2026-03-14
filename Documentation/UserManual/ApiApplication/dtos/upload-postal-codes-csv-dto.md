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

## Used By Endpoints

- [POST UploadPostalCodesCsv](../postal-codes/post-upload-postal-codes-csv-api-v1-postal-codes-upload-csv.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

