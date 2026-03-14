# TokenDto

Data transfer object representing an authentication or refresh token

## Source

`DR_Admin/DTOs/TokenDto.cs`

## TypeScript Interface

```ts
export interface TokenDto {
  id: number;
  userId: number;
  tokenType: string;
  expiry: string;
  createdAt: string;
  updatedAt: string;
  revokedAt: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `UserId` | `int` | `number` |
| `TokenType` | `string` | `string` |
| `Expiry` | `DateTime` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `RevokedAt` | `DateTime?` | `string | null` |

## Used By Endpoints

- [GET GetTokenById](../tokens/get-get-token-by-id-api-v1-tokens-id.md)
- [POST CreateToken](../tokens/post-create-token-api-v1-tokens.md)
- [PUT UpdateToken](../tokens/put-update-token-api-v1-tokens-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

