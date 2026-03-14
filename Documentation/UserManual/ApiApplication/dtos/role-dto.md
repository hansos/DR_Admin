# RoleDto

Data transfer object representing a user role

## Source

`DR_Admin/DTOs/RoleDto.cs`

## TypeScript Interface

```ts
export interface RoleDto {
  id: number;
  name: string;
  description: string;
  code: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `Code` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetRoleById](../roles/get-get-role-by-id-api-v1-roles-id.md)
- [POST CreateRole](../roles/post-create-role-api-v1-roles.md)
- [PUT UpdateRole](../roles/put-update-role-api-v1-roles-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

