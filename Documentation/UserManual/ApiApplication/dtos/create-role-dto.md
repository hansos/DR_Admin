# CreateRoleDto

Data transfer object for creating a new role

## Source

`DR_Admin/DTOs/RoleDto.cs`

## TypeScript Interface

```ts
export interface CreateRoleDto {
  name: string;
  description: string;
  code: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `Code` | `string` | `string` |

## Used By Endpoints

- [POST CreateRole](../roles/post-create-role-api-v1-roles.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

