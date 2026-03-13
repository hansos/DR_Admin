# CreateHostProviderDto

Data transfer object for creating a new host provider

## Source

`DR_Admin/DTOs/HostProviderDto.cs`

## TypeScript Interface

```ts
export interface CreateHostProviderDto {
  name: string;
  displayName: string;
  description: string | null;
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
| `WebsiteUrl` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
