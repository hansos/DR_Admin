# DocumentTemplateDto

Data transfer object representing a document template

## Source

`DR_Admin/DTOs/DocumentTemplateDto.cs`

## TypeScript Interface

```ts
export interface DocumentTemplateDto {
  id: number;
  name: string;
  description: string;
  templateType: DocumentTemplateType;
  fileName: string;
  fileSize: number;
  mimeType: string;
  isActive: boolean;
  isDefault: boolean;
  placeholderVariables: string;
  createdAt: string;
  updatedAt: string;
  deletedAt: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `TemplateType` | `DocumentTemplateType` | `DocumentTemplateType` |
| `FileName` | `string` | `string` |
| `FileSize` | `long` | `number` |
| `MimeType` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `PlaceholderVariables` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `DeletedAt` | `DateTime?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
