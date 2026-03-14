# LoginResponseDto

Data transfer object for login response containing authentication tokens and user information

## Source

`DR_Admin/DTOs/LoginResponseDto.cs`

## TypeScript Interface

```ts
export interface LoginResponseDto {
  userId: number;
  accessToken: string;
  refreshToken: string;
  username: string;
  expiresAt: string;
  roles: string[];
  requiresTwoFactor: boolean;
  twoFactorMethod: string | null;
  twoFactorChallengeToken: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `UserId` | `int` | `number` |
| `AccessToken` | `string` | `string` |
| `RefreshToken` | `string` | `string` |
| `Username` | `string` | `string` |
| `ExpiresAt` | `DateTime` | `string` |
| `Roles` | `IEnumerable<string>` | `string[]` |
| `RequiresTwoFactor` | `bool` | `boolean` |
| `TwoFactorMethod` | `string?` | `string | null` |
| `TwoFactorChallengeToken` | `string?` | `string | null` |

## Used By Endpoints

- [POST VerifyMailTwoFactor](../auth/post-verify-mail-two-factor-api-v1-auth-2fa-verify.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

