# RegisterAccountResponseDto

Response DTO for successful registration

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface RegisterAccountResponseDto {
  userId: number;
  email: string;
  message: string;
  emailConfirmationToken: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `UserId` | `int` | `number` |
| `Email` | `string` | `string` |
| `Message` | `string` | `string` |
| `EmailConfirmationToken` | `string?` | `string | null` |

## Used By Endpoints

- [POST Register](../my-account/post-register-api-v1-my-account-register.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

