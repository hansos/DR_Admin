# UserAccountDto

DTO for user account information

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface UserAccountDto {
  id: number;
  username: string;
  email: string;
  emailConfirmed: string | null;
  customer: CustomerAccountDto | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Username` | `string` | `string` |
| `Email` | `string` | `string` |
| `EmailConfirmed` | `DateTime?` | `string | null` |
| `Customer` | `CustomerAccountDto?` | `CustomerAccountDto | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
