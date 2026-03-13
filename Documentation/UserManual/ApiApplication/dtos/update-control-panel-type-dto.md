# UpdateControlPanelTypeDto

Data transfer object for updating an existing control panel type

## Source

`DR_Admin/DTOs/ControlPanelTypeDto.cs`

## TypeScript Interface

```ts
export interface UpdateControlPanelTypeDto {
  name: string;
  displayName: string;
  description: string | null;
  version: string | null;
  websiteUrl: string | null;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `DisplayName` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `Version` | `string?` | `string | null` |
| `WebsiteUrl` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
