# CreateUserDto

Data transfer object for creating a new user

## Source

`DR_Admin/DTOs/UserDto.cs`

## TypeScript Interface

```ts
export interface CreateUserDto {
  customerId: number | null;
  username: string;
  password: string;
  email: string;
  roles: string[] | null;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerId` | `int?` | `number | null` |
| `Username` | `string` | `string` |
| `Password` | `string` | `string` |
| `Email` | `string` | `string` |
| `Roles` | `List<string>?` | `string[] | null` |
| `IsActive` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
