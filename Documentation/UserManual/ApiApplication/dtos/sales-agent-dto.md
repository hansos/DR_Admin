# SalesAgentDto

Data transfer object representing a sales agent

## Source

`DR_Admin/DTOs/SalesAgentDto.cs`

## TypeScript Interface

```ts
export interface SalesAgentDto {
  id: number;
  resellerCompanyId: number | null;
  firstName: string;
  lastName: string;
  email: string | null;
  phone: string | null;
  mobilePhone: string | null;
  isActive: boolean;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `ResellerCompanyId` | `int?` | `number | null` |
| `FirstName` | `string` | `string` |
| `LastName` | `string` | `string` |
| `Email` | `string?` | `string | null` |
| `Phone` | `string?` | `string | null` |
| `MobilePhone` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
