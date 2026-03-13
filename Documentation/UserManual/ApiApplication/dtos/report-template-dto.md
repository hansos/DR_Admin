# ReportTemplateDto

Data transfer object representing a report template

## Source

`DR_Admin/DTOs/ReportTemplateDto.cs`

## TypeScript Interface

```ts
export interface ReportTemplateDto {
  id: number;
  name: string;
  description: string;
  templateType: ReportTemplateType;
  reportEngine: string;
  fileName: string;
  fileSize: number;
  mimeType: string;
  isActive: boolean;
  isDefault: boolean;
  dataSourceInfo: string;
  version: string;
  tags: string;
  defaultExportFormat: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `TemplateType` | `ReportTemplateType` | `ReportTemplateType` |
| `ReportEngine` | `string` | `string` |
| `FileName` | `string` | `string` |
| `FileSize` | `long` | `number` |
| `MimeType` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `DataSourceInfo` | `string` | `string` |
| `Version` | `string` | `string` |
| `Tags` | `string` | `string` |
| `DefaultExportFormat` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
