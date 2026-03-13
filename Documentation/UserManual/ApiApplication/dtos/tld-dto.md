# TldDto

Data transfer object representing a Top-Level Domain (TLD)

## Source

`DR_Admin/DTOs/TldDto.cs`

## TypeScript Interface

```ts
export interface TldDto {
  id: number;
  extension: string;
  description: string;
  isActive: boolean;
  defaultRegistrationYears: number | null;
  maxRegistrationYears: number | null;
  requiresPrivacy: boolean;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Extension` | `string` | `string` |
| `Description` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `DefaultRegistrationYears` | `int?` | `number | null` |
| `MaxRegistrationYears` | `int?` | `number | null` |
| `RequiresPrivacy` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
