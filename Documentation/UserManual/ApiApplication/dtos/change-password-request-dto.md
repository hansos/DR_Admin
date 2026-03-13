# ChangePasswordRequestDto

Request DTO for changing password (authenticated user)

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface ChangePasswordRequestDto {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CurrentPassword` | `string` | `string` |
| `NewPassword` | `string` | `string` |
| `ConfirmPassword` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
