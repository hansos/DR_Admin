# CategorizedContactPersonDto

Contact person with categorization for role-based selection

## Source

`DR_Admin/DTOs/CategorizedContactPersonDto.cs`

## TypeScript Interface

```ts
export interface CategorizedContactPersonDto {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone: string;
  position: string | null;
  department: string | null;
  isPrimary: boolean;
  isActive: boolean;
  category: ContactPersonCategory;
  usageCount: number;
  customerId: number | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `FirstName` | `string` | `string` |
| `LastName` | `string` | `string` |
| `FullName` | `string` | `string` |
| `Email` | `string` | `string` |
| `Phone` | `string` | `string` |
| `Position` | `string?` | `string | null` |
| `Department` | `string?` | `string | null` |
| `IsPrimary` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `Category` | `ContactPersonCategory` | `ContactPersonCategory` |
| `UsageCount` | `int` | `number` |
| `CustomerId` | `int?` | `number | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
