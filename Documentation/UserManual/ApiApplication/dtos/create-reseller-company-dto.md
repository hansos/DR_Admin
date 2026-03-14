# CreateResellerCompanyDto

Data transfer object for creating a new reseller company

## Source

`DR_Admin/DTOs/ResellerCompanyDto.cs`

## TypeScript Interface

```ts
export interface CreateResellerCompanyDto {
  name: string;
  contactPerson: string | null;
  email: string | null;
  phone: string | null;
  address: string | null;
  city: string | null;
  state: string | null;
  postalCode: string | null;
  countryCode: string | null;
  companyRegistrationNumber: string | null;
  taxId: string | null;
  vatNumber: string | null;
  isActive: boolean;
  isDefault: boolean;
  notes: string | null;
  defaultCurrency: string;
  supportedCurrencies: string | null;
  applyCurrencyMarkup: boolean;
  defaultCurrencyMarkup: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `ContactPerson` | `string?` | `string | null` |
| `Email` | `string?` | `string | null` |
| `Phone` | `string?` | `string | null` |
| `Address` | `string?` | `string | null` |
| `City` | `string?` | `string | null` |
| `State` | `string?` | `string | null` |
| `PostalCode` | `string?` | `string | null` |
| `CountryCode` | `string?` | `string | null` |
| `CompanyRegistrationNumber` | `string?` | `string | null` |
| `TaxId` | `string?` | `string | null` |
| `VatNumber` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |
| `DefaultCurrency` | `string` | `string` |
| `SupportedCurrencies` | `string?` | `string | null` |
| `ApplyCurrencyMarkup` | `bool` | `boolean` |
| `DefaultCurrencyMarkup` | `decimal` | `number` |

## Used By Endpoints

- [POST CreateResellerCompany](../reseller-companies/post-create-reseller-company-api-v1-reseller-companies.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

