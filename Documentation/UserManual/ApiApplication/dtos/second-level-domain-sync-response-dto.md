# SecondLevelDomainSyncResponseDto

Data transfer object for second-level domain synchronization response

## Source

`DR_Admin/DTOs/TldSyncDto.cs`

## TypeScript Interface

```ts
export interface SecondLevelDomainSyncResponseDto {
  success: boolean;
  message: string;
  secondLevelDomainsAdded: number;
  parentTldsProcessed: number;
  parentTldsSkipped: number;
  syncTimestamp: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `SecondLevelDomainsAdded` | `int` | `number` |
| `ParentTldsProcessed` | `int` | `number` |
| `ParentTldsSkipped` | `int` | `number` |
| `SyncTimestamp` | `DateTime` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

