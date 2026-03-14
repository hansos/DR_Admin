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

## Used By Endpoints

- [POST VerifyMailTwoFactor](../auth/post-verify-mail-two-factor-api-v1-auth-2fa-verify.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

