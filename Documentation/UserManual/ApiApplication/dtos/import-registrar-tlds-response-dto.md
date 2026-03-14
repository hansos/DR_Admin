# ImportRegistrarTldsResponseDto

Response data transfer object for TLD import operation

## Source

`DR_Admin/DTOs/TldDto.cs`

## TypeScript Interface

```ts
export interface ImportRegistrarTldsResponseDto {
  success: boolean;
  message: string;
  tldsAdded: number;
  tldsExisting: number;
  registrarTldsCreated: number;
  registrarTldsExisting: number;
  linesSkipped: number;
  importTimestamp: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `TldsAdded` | `int` | `number` |
| `TldsExisting` | `int` | `number` |
| `RegistrarTldsCreated` | `int` | `number` |
| `RegistrarTldsExisting` | `int` | `number` |
| `LinesSkipped` | `int` | `number` |
| `ImportTimestamp` | `DateTime` | `string` |

## Used By Endpoints

- [POST ImportRegistrarTlds](../registrar-tlds/post-import-registrar-tlds-api-v1-registrar-tlds-registrar-registrarid-import.md)
- [POST UploadRegistrarTldsCsv](../registrar-tlds/post-upload-registrar-tlds-csv-api-v1-registrar-tlds-registrar-registrarid-upload-csv.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

