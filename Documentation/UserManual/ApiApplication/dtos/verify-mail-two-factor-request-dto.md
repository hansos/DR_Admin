# VerifyMailTwoFactorRequestDto

Request DTO for verifying a two-factor authentication code.

## Source

`DR_Admin/DTOs/AuthTwoFactorDto.cs`

## TypeScript Interface

```ts
export interface VerifyMailTwoFactorRequestDto {
  challengeToken: string;
  code: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ChallengeToken` | `string` | `string` |
| `Code` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
