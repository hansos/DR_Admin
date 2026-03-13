# CreateDomainContactDto

Data transfer object for creating a new domain contact

## Source

`DR_Admin/DTOs/DomainContactDto.cs`

## TypeScript Interface

```ts
export interface CreateDomainContactDto {
  contactType: string;
  firstName: string;
  lastName: string;
  organization: string | null;
  email: string;
  phone: string;
  fax: string | null;
  address1: string;
  address2: string | null;
  city: string;
  state: string | null;
  postalCode: string;
  countryCode: string;
  isActive: boolean;
  notes: string | null;
  domainId: number;
  sourceContactPersonId: number | null;
  registrarContactId: string | null;
  registrarType: string | null;
  isPrivacyProtected: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ContactType` | `string` | `string` |
| `FirstName` | `string` | `string` |
| `LastName` | `string` | `string` |
| `Organization` | `string?` | `string | null` |
| `Email` | `string` | `string` |
| `Phone` | `string` | `string` |
| `Fax` | `string?` | `string | null` |
| `Address1` | `string` | `string` |
| `Address2` | `string?` | `string | null` |
| `City` | `string` | `string` |
| `State` | `string?` | `string | null` |
| `PostalCode` | `string` | `string` |
| `CountryCode` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |
| `DomainId` | `int` | `number` |
| `SourceContactPersonId` | `int?` | `number | null` |
| `RegistrarContactId` | `string?` | `string | null` |
| `RegistrarType` | `string?` | `string | null` |
| `IsPrivacyProtected` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
