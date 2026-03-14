# SetPasswordRequestDto

Request DTO for setting password (first time or after reset)

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface SetPasswordRequestDto {
  email: string;
  token: string;
  newPassword: string;
  confirmPassword: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Email` | `string` | `string` |
| `Token` | `string` | `string` |
| `NewPassword` | `string` | `string` |
| `ConfirmPassword` | `string` | `string` |

## Used By Endpoints

- [POST SetPassword](../my-account/post-set-password-api-v1-my-account-set-password.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

