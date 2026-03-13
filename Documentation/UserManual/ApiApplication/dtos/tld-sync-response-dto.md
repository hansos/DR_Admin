# TldSyncResponseDto

Data transfer object for TLD synchronization response

## Source

`DR_Admin/DTOs/TldSyncDto.cs`

## TypeScript Interface

```ts
export interface TldSyncResponseDto {
  success: boolean;
  message: string;
  tldsAdded: number;
  tldsUpdated: number;
  totalTldsInSource: number;
  syncTimestamp: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `TldsAdded` | `int` | `number` |
| `TldsUpdated` | `int` | `number` |
| `TotalTldsInSource` | `int` | `number` |
| `SyncTimestamp` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
