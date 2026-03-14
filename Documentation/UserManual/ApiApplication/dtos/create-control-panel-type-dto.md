# CreateControlPanelTypeDto

Data transfer object for creating a new control panel type

## Source

`DR_Admin/DTOs/ControlPanelTypeDto.cs`

## TypeScript Interface

```ts
export interface CreateControlPanelTypeDto {
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

## Used By Endpoints

- [POST CreateControlPanelType](../control-panel-types/post-create-control-panel-type-api-v1-control-panel-types.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

