# CreateDocumentTemplateDto

Data transfer object for creating a new document template

## Source

`DR_Admin/DTOs/DocumentTemplateDto.cs`

## TypeScript Interface

```ts
export interface CreateDocumentTemplateDto {
  name: string;
  description: string;
  templateType: DocumentTemplateType;
  file: IFormFile | null;
  isActive: boolean;
  isDefault: boolean;
  placeholderVariables: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `TemplateType` | `DocumentTemplateType` | `DocumentTemplateType` |
| `File` | `IFormFile?` | `IFormFile | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `PlaceholderVariables` | `string` | `string` |

## Used By Endpoints

- [POST UploadTemplate](../document-templates/post-upload-template-api-v1-document-templates-upload.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

