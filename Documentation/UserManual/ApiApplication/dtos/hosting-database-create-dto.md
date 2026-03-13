# HostingDatabaseCreateDto

Data transfer object for HostingDatabaseCreateDto.

## Source

`DR_Admin/DTOs/HostingResourceDto.cs`

## TypeScript Interface

```ts
export interface HostingDatabaseCreateDto {
  hostingAccountId: number;
  databaseName: string;
  databaseType: string;
  characterSet: string | null;
  collation: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `HostingAccountId` | `int` | `number` |
| `DatabaseName` | `string` | `string` |
| `DatabaseType` | `string` | `string` |
| `CharacterSet` | `string?` | `string | null` |
| `Collation` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
