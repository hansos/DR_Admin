# ResendMailTwoFactorRequestDto

Request DTO for resending a mail two-factor authentication code.

## Source

`DR_Admin/DTOs/AuthTwoFactorDto.cs`

## TypeScript Interface

```ts
export interface ResendMailTwoFactorRequestDto {
  challengeToken: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ChallengeToken` | `string` | `string` |

## Used By Endpoints

- [POST ResendMailTwoFactor](../auth/post-resend-mail-two-factor-api-v1-auth-2fa-resend.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

