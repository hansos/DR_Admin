# UserDto

Data transfer object representing a user

## Source

`DR_Admin/DTOs/UserDto.cs`

## TypeScript Interface

```ts
export interface UserDto {
  id: number;
  customerId: number | null;
  username: string;
  email: string;
  isActive: boolean;
  isCustomerSelfRegistered: boolean;
  roles: string[];
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int?` | `number | null` |
| `Username` | `string` | `string` |
| `Email` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `IsCustomerSelfRegistered` | `bool` | `boolean` |
| `Roles` | `List<string>` | `string[]` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetAllUsers](../users/get-get-all-users-api-v1-users.md)
- [GET GetUserById](../users/get-get-user-by-id-api-v1-users-id.md)
- [POST CreateUser](../users/post-create-user-api-v1-users.md)
- [PUT UpdateUser](../users/put-update-user-api-v1-users-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

