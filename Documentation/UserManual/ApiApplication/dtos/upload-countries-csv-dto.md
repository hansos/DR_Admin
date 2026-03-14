# UploadCountriesCsvDto

Data transfer object for uploading a CSV file with countries

## Source

`DR_Admin/DTOs/UploadCountriesCsvDto.cs`

## TypeScript Interface

```ts
export interface UploadCountriesCsvDto {
  file: IFormFile | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `File` | `IFormFile?` | `IFormFile | null` |

## Used By Endpoints

- [POST UploadCountriesCsv](../countries/post-upload-countries-csv-api-v1-countries-upload-csv.md)
- [POST UploadLocalizedNamesCsv](../countries/post-upload-localized-names-csv-api-v1-countries-upload-localized-names-csv.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

