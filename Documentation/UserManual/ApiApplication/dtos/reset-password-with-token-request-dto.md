# ResetPasswordWithTokenRequestDto

Request DTO for resetting password with token

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface ResetPasswordWithTokenRequestDto {
  token: string;
  newPassword: string;
  confirmPassword: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Token` | `string` | `string` |
| `NewPassword` | `string` | `string` |
| `ConfirmPassword` | `string` | `string` |

## Used By Endpoints

- [POST ResetPassword](../my-account/post-reset-password-api-v1-my-account-reset-password.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

