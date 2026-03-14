# UploadRegistrarTldsCsvDto

Data transfer object for uploading a CSV file with TLDs for a registrar

## Source

`DR_Admin/DTOs/UploadRegistrarTldsCsvDto.cs`

## TypeScript Interface

```ts
export interface UploadRegistrarTldsCsvDto {
  file: IFormFile | null;
  defaultRegistrationCost: number | null;
  defaultRegistrationPrice: number | null;
  defaultRenewalCost: number | null;
  defaultRenewalPrice: number | null;
  defaultTransferCost: number | null;
  defaultTransferPrice: number | null;
  isAvailable: boolean;
  activateNewTlds: boolean;
  currency: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `File` | `IFormFile?` | `IFormFile | null` |
| `DefaultRegistrationCost` | `decimal?` | `number | null` |
| `DefaultRegistrationPrice` | `decimal?` | `number | null` |
| `DefaultRenewalCost` | `decimal?` | `number | null` |
| `DefaultRenewalPrice` | `decimal?` | `number | null` |
| `DefaultTransferCost` | `decimal?` | `number | null` |
| `DefaultTransferPrice` | `decimal?` | `number | null` |
| `IsAvailable` | `bool` | `boolean` |
| `ActivateNewTlds` | `bool` | `boolean` |
| `Currency` | `string` | `string` |

## Used By Endpoints

- [POST UploadRegistrarTldsCsv](../registrar-tlds/post-upload-registrar-tlds-csv-api-v1-registrar-tlds-registrar-registrarid-upload-csv.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

