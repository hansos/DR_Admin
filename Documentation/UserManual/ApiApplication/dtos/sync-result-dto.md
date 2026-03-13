# SyncResultDto

Data transfer object for SyncResultDto.

## Source

`DR_Admin/DTOs/HostingAccountDto.cs`

## TypeScript Interface

```ts
export interface SyncResultDto {
  success: boolean;
  message: string;
  recordsSynced: number;
  syncedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `RecordsSynced` | `int` | `number` |
| `SyncedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
