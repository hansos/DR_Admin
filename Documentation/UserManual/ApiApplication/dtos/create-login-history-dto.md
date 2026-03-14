# CreateLoginHistoryDto

Data transfer object for creating a login history entry.

## Source

`DR_Admin/DTOs/LoginHistoryDto.cs`

## TypeScript Interface

```ts
export interface CreateLoginHistoryDto {
  userId: number | null;
  identifier: string;
  isSuccessful: boolean;
  attemptedAt: string;
  iPAddress: string;
  userAgent: string;
  failureReason: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `UserId` | `int?` | `number | null` |
| `Identifier` | `string` | `string` |
| `IsSuccessful` | `bool` | `boolean` |
| `AttemptedAt` | `DateTime` | `string` |
| `IPAddress` | `string` | `string` |
| `UserAgent` | `string` | `string` |
| `FailureReason` | `string?` | `string | null` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

