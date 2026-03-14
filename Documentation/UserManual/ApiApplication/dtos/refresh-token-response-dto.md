# RefreshTokenResponseDto

Response DTO for token refresh

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface RefreshTokenResponseDto {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
  refreshTokenExpiresAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `AccessToken` | `string` | `string` |
| `RefreshToken` | `string` | `string` |
| `AccessTokenExpiresAt` | `DateTime` | `string` |
| `RefreshTokenExpiresAt` | `DateTime` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

