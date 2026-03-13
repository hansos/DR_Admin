# CreateTldDto

Data transfer object for creating a new TLD

## Source

`DR_Admin/DTOs/TldDto.cs`

## TypeScript Interface

```ts
export interface CreateTldDto {
  extension: string;
  description: string;
  isSecondLevel: boolean;
  isActive: boolean;
  defaultRegistrationYears: number | null;
  maxRegistrationYears: number | null;
  requiresPrivacy: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Extension` | `string` | `string` |
| `Description` | `string` | `string` |
| `IsSecondLevel` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `DefaultRegistrationYears` | `int?` | `number | null` |
| `MaxRegistrationYears` | `int?` | `number | null` |
| `RequiresPrivacy` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
