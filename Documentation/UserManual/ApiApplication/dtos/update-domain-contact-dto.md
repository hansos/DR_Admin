# UpdateDomainContactDto

Data transfer object for updating an existing domain contact

## Source

`DR_Admin/DTOs/DomainContactDto.cs`

## TypeScript Interface

```ts
export interface UpdateDomainContactDto {
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

## Used By Endpoints

- [PUT UpdateDomainContact](../domain-contacts/put-update-domain-contact-api-v1-domain-contacts-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

