# ContactPersonDto

Data transfer object representing a contact person

## Source

`DR_Admin/DTOs/ContactPersonDto.cs`

## TypeScript Interface

```ts
export interface ContactPersonDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  position: string | null;
  department: string | null;
  isPrimary: boolean;
  isActive: boolean;
  notes: string | null;
  customerId: number | null;
  createdAt: string;
  updatedAt: string;
  isDefaultOwner: boolean;
  isDefaultBilling: boolean;
  isDefaultTech: boolean;
  isDefaultAdministrator: boolean;
  isDomainGlobal: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `FirstName` | `string` | `string` |
| `LastName` | `string` | `string` |
| `Email` | `string` | `string` |
| `Phone` | `string` | `string` |
| `Position` | `string?` | `string | null` |
| `Department` | `string?` | `string | null` |
| `IsPrimary` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |
| `CustomerId` | `int?` | `number | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `IsDefaultOwner` | `bool` | `boolean` |
| `IsDefaultBilling` | `bool` | `boolean` |
| `IsDefaultTech` | `bool` | `boolean` |
| `IsDefaultAdministrator` | `bool` | `boolean` |
| `IsDomainGlobal` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
