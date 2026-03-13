# CreateDomainContactAssignmentDto

Data transfer object for creating a new domain contact assignment

## Source

`DR_Admin/DTOs/DomainContactAssignmentDto.cs`

## TypeScript Interface

```ts
export interface CreateDomainContactAssignmentDto {
  registeredDomainId: number;
  contactPersonId: number;
  roleType: string;
  isActive: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `RegisteredDomainId` | `int` | `number` |
| `ContactPersonId` | `int` | `number` |
| `RoleType` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
