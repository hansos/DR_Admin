# LoginHistoryDto

Data transfer object representing a login attempt.

## Source

`DR_Admin/DTOs/LoginHistoryDto.cs`

## TypeScript Interface

```ts
export interface LoginHistoryDto {
  id: number;
  userId: number | null;
  username: string | null;
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
| `Id` | `int` | `number` |
| `UserId` | `int?` | `number | null` |
| `Username` | `string?` | `string | null` |
| `Identifier` | `string` | `string` |
| `IsSuccessful` | `bool` | `boolean` |
| `AttemptedAt` | `DateTime` | `string` |
| `IPAddress` | `string` | `string` |
| `UserAgent` | `string` | `string` |
| `FailureReason` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
