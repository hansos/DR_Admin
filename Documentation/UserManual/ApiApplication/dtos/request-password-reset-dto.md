# RequestPasswordResetDto

Request DTO for requesting a password reset

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface RequestPasswordResetDto {
  email: string;
  siteCode: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Email` | `string` | `string` |
| `SiteCode` | `string?` | `string | null` |

## Used By Endpoints

- [POST RequestPasswordReset](../my-account/post-request-password-reset-api-v1-my-account-request-password-reset.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

