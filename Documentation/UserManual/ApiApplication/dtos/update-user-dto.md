# UpdateUserDto

Data transfer object for updating an existing user

## Source

`DR_Admin/DTOs/UserDto.cs`

## TypeScript Interface

```ts
export interface UpdateUserDto {
  customerId: number | null;
  username: string;
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
| `Email` | `string` | `string` |
| `Roles` | `List<string>?` | `string[] | null` |
| `IsActive` | `bool` | `boolean` |

## Used By Endpoints

- [PUT UpdateUser](../users/put-update-user-api-v1-users-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

