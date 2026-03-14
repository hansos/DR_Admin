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

## Used By Endpoints

- [POST AssignTldToRegistrarByDto](../registrars/post-assign-tld-to-registrar-by-dto-api-v1-registrars-registrarid-tld.md)
- [GET GetActiveTlds](../tlds/get-get-active-tlds-api-v1-tlds-active.md)
- [GET GetAllTlds](../tlds/get-get-all-tlds-api-v1-tlds.md)
- [GET GetSecondLevelTlds](../tlds/get-get-second-level-tlds-api-v1-tlds-secondlevel.md)
- [GET GetTldByExtension](../tlds/get-get-tld-by-extension-api-v1-tlds-extension-extension.md)
- [GET GetTldById](../tlds/get-get-tld-by-id-api-v1-tlds-id.md)
- [GET GetTopLevelTlds](../tlds/get-get-top-level-tlds-api-v1-tlds-toplevel.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

