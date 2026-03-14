# CreateTokenDto

Data transfer object for creating a new token

## Source

`DR_Admin/DTOs/TokenDto.cs`

## TypeScript Interface

```ts
export interface CreateTokenDto {
  userId: number;
  tokenType: string;
  tokenValue: string;
  expiry: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `UserId` | `int` | `number` |
| `TokenType` | `string` | `string` |
| `TokenValue` | `string` | `string` |
| `Expiry` | `DateTime` | `string` |

## Used By Endpoints

- [POST CreateToken](../tokens/post-create-token-api-v1-tokens.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

