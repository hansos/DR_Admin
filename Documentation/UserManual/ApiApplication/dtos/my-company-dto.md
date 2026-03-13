# MyCompanyDto

Data transfer object representing the reseller's own company profile.

## Source

`DR_Admin/DTOs/MyCompanyDto.cs`

## TypeScript Interface

```ts
export interface MyCompanyDto {
  id: number;
  name: string;
  legalName: string | null;
  email: string | null;
  phone: string | null;
  addressLine1: string | null;
  addressLine2: string | null;
  postalCode: string | null;
  city: string | null;
  state: string | null;
  countryCode: string | null;
  organizationNumber: string | null;
  taxId: string | null;
  vatNumber: string | null;
  invoiceEmail: string | null;
  website: string | null;
  logoUrl: string | null;
  letterheadFooter: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `LegalName` | `string?` | `string | null` |
| `Email` | `string?` | `string | null` |
| `Phone` | `string?` | `string | null` |
| `AddressLine1` | `string?` | `string | null` |
| `AddressLine2` | `string?` | `string | null` |
| `PostalCode` | `string?` | `string | null` |
| `City` | `string?` | `string | null` |
| `State` | `string?` | `string | null` |
| `CountryCode` | `string?` | `string | null` |
| `OrganizationNumber` | `string?` | `string | null` |
| `TaxId` | `string?` | `string | null` |
| `VatNumber` | `string?` | `string | null` |
| `InvoiceEmail` | `string?` | `string | null` |
| `Website` | `string?` | `string | null` |
| `LogoUrl` | `string?` | `string | null` |
| `LetterheadFooter` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
