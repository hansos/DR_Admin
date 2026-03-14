# PatchEmailRequestDto

Request DTO for updating email address

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface PatchEmailRequestDto {
  newEmail: string;
  password: string;
  siteCode: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `NewEmail` | `string` | `string` |
| `Password` | `string` | `string` |
| `SiteCode` | `string?` | `string | null` |

## Used By Endpoints

- [PATCH PatchEmail](../my-account/patch-patch-email-api-v1-my-account-email.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

