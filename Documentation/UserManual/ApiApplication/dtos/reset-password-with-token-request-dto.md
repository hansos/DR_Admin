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

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
