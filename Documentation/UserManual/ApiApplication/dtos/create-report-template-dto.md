# CreateReportTemplateDto

Data transfer object for creating a new report template

## Source

`DR_Admin/DTOs/CreateReportTemplateDto.cs`

## TypeScript Interface

```ts
export interface CreateReportTemplateDto {
  name: string;
  description: string;
  templateType: ReportTemplateType;
  reportEngine: string;
  file: IFormFile | null;
  isActive: boolean;
  isDefault: boolean;
  dataSourceInfo: string;
  version: string;
  tags: string;
  defaultExportFormat: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `TemplateType` | `ReportTemplateType` | `ReportTemplateType` |
| `ReportEngine` | `string` | `string` |
| `File` | `IFormFile?` | `IFormFile | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `DataSourceInfo` | `string` | `string` |
| `Version` | `string` | `string` |
| `Tags` | `string` | `string` |
| `DefaultExportFormat` | `string` | `string` |

## Used By Endpoints

- [POST CreateTemplate](../report-templates/post-create-template-api-v1-report-templates.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

