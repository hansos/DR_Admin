# SyncComparisonDto

Data transfer object for SyncComparisonDto.

## Source

`DR_Admin/DTOs/HostingAccountDto.cs`

## TypeScript Interface

```ts
export interface SyncComparisonDto {
  hostingAccountId: number;
  inSync: boolean;
  differences: string[];
  lastChecked: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `HostingAccountId` | `int` | `number` |
| `InSync` | `bool` | `boolean` |
| `Differences` | `List<string>` | `string[]` |
| `LastChecked` | `DateTime` | `string` |

## Used By Endpoints

- [GET CompareWithServer](../hosting-sync/get-compare-with-server-api-v1-hosting-sync-compare-hostingaccountid.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

