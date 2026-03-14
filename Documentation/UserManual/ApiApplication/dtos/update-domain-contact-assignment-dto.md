# UpdateDomainContactAssignmentDto

Data transfer object for updating an existing domain contact assignment

## Source

`DR_Admin/DTOs/DomainContactAssignmentDto.cs`

## TypeScript Interface

```ts
export interface UpdateDomainContactAssignmentDto {
  contactPersonId: number;
  roleType: string;
  isActive: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ContactPersonId` | `int` | `number` |
| `RoleType` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

