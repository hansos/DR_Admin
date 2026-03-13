# UpdateDocumentTemplateDto

Data transfer object for updating an existing document template

## Source

`DR_Admin/DTOs/DocumentTemplateDto.cs`

## TypeScript Interface

```ts
export interface UpdateDocumentTemplateDto {
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

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
