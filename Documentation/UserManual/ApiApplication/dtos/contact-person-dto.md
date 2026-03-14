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

## Used By Endpoints

- [GET GetAllContactPersons](../contact-persons/get-get-all-contact-persons-api-v1-contact-persons.md)
- [GET GetContactPersonById](../contact-persons/get-get-contact-person-by-id-api-v1-contact-persons-id.md)
- [PATCH PatchContactPersonIsDomainGlobal](../contact-persons/patch-patch-contact-person-is-domain-global-api-v1-contact-persons-id-domain-global.md)
- [POST CreateContactPerson](../contact-persons/post-create-contact-person-api-v1-contact-persons.md)
- [PUT UpdateContactPerson](../contact-persons/put-update-contact-person-api-v1-contact-persons-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

