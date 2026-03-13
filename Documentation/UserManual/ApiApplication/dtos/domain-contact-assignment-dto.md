# DomainContactAssignmentDto

Data transfer object representing a domain contact assignment

## Source

`DR_Admin/DTOs/DomainContactAssignmentDto.cs`

## TypeScript Interface

```ts
export interface DomainContactAssignmentDto {
  id: number;
  registeredDomainId: number;
  contactPersonId: number;
  roleType: string;
  assignedAt: string;
  isActive: boolean;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
  contactPerson: ContactPersonDto | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `RegisteredDomainId` | `int` | `number` |
| `ContactPersonId` | `int` | `number` |
| `RoleType` | `string` | `string` |
| `AssignedAt` | `DateTime` | `string` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `ContactPerson` | `ContactPersonDto?` | `ContactPersonDto | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
