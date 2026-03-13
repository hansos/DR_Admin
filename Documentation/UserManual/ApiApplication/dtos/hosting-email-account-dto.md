# HostingEmailAccountDto

Data transfer object for HostingEmailAccountDto.

## Source

`DR_Admin/DTOs/HostingResourceDto.cs`

## TypeScript Interface

```ts
export interface HostingEmailAccountDto {
  id: number;
  hostingAccountId: number;
  emailAddress: string;
  username: string;
  quotaMB: number | null;
  usageMB: number | null;
  isForwarderOnly: boolean;
  forwardTo: string | null;
  autoResponderEnabled: boolean;
  spamFilterEnabled: boolean;
  externalEmailId: string | null;
  lastSyncedAt: string | null;
  syncStatus: string | null;
  createdAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `HostingAccountId` | `int` | `number` |
| `EmailAddress` | `string` | `string` |
| `Username` | `string` | `string` |
| `QuotaMB` | `int?` | `number | null` |
| `UsageMB` | `int?` | `number | null` |
| `IsForwarderOnly` | `bool` | `boolean` |
| `ForwardTo` | `string?` | `string | null` |
| `AutoResponderEnabled` | `bool` | `boolean` |
| `SpamFilterEnabled` | `bool` | `boolean` |
| `ExternalEmailId` | `string?` | `string | null` |
| `LastSyncedAt` | `DateTime?` | `string | null` |
| `SyncStatus` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
