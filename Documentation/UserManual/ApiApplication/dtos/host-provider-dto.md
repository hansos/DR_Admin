# HostProviderDto

Data transfer object representing a hosting provider

## Source

`DR_Admin/DTOs/HostProviderDto.cs`

## TypeScript Interface

```ts
export interface HostProviderDto {
  id: number;
  name: string;
  displayName: string;
  description: string | null;
  websiteUrl: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `DisplayName` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `WebsiteUrl` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetHostProviderById](../host-providers/get-get-host-provider-by-id-api-v1-host-providers-id.md)
- [POST CreateHostProvider](../host-providers/post-create-host-provider-api-v1-host-providers.md)
- [PUT UpdateHostProvider](../host-providers/put-update-host-provider-api-v1-host-providers-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

