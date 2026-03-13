# NameServerDto

Data transfer object representing a name server for a domain

## Source

`DR_Admin/DTOs/NameServerDto.cs`

## TypeScript Interface

```ts
export interface NameServerDto {
  id: number;
  domainIds: number[];
  serverId: number | null;
  hostname: string;
  ipAddress: string | null;
  isPrimary: boolean;
  sortOrder: number;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `DomainIds` | `List<int>` | `number[]` |
| `ServerId` | `int?` | `number | null` |
| `Hostname` | `string` | `string` |
| `IpAddress` | `string?` | `string | null` |
| `IsPrimary` | `bool` | `boolean` |
| `SortOrder` | `int` | `number` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
