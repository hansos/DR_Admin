# UpdateRoleDto

Data transfer object for updating an existing role

## Source

`DR_Admin/DTOs/RoleDto.cs`

## TypeScript Interface

```ts
export interface UpdateRoleDto {
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

- [PUT UpdateRole](../roles/put-update-role-api-v1-roles-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

