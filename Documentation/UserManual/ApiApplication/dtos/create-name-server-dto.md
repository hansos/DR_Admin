# CreateNameServerDto

Data transfer object for creating a new name server

## Source

`DR_Admin/DTOs/NameServerDto.cs`

## TypeScript Interface

```ts
export interface CreateNameServerDto {
  domainIds: number[];
  serverId: number | null;
  hostname: string;
  ipAddress: string | null;
  isPrimary: boolean;
  sortOrder: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `DomainIds` | `List<int>` | `number[]` |
| `ServerId` | `int?` | `number | null` |
| `Hostname` | `string` | `string` |
| `IpAddress` | `string?` | `string | null` |
| `IsPrimary` | `bool` | `boolean` |
| `SortOrder` | `int` | `number` |

## Used By Endpoints

- [POST CreateNameServer](../name-servers/post-create-name-server-api-v1-name-servers.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

