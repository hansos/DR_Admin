# DomainContactDto

Data transfer object representing a domain contact person

## Source

`DR_Admin/DTOs/DomainContactDto.cs`

## TypeScript Interface

```ts
export interface DomainContactDto {
  id: number;
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
  lastSyncedAt: string | null;
  needsSync: boolean;
  registrarContactId: string | null;
  registrarType: string | null;
  isPrivacyProtected: boolean;
  isCurrentVersion: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
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
| `LastSyncedAt` | `DateTime?` | `string | null` |
| `NeedsSync` | `bool` | `boolean` |
| `RegistrarContactId` | `string?` | `string | null` |
| `RegistrarType` | `string?` | `string | null` |
| `IsPrivacyProtected` | `bool` | `boolean` |
| `IsCurrentVersion` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
