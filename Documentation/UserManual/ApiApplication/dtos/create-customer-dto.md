# CreateCustomerDto

Data transfer object for creating a new customer

## Source

`DR_Admin/DTOs/CustomerDto.cs`

## TypeScript Interface

```ts
export interface CreateCustomerDto {
  name: string;
  email: string;
  phone: string;
  countryCode: string | null;
  customerName: string | null;
  taxId: string | null;
  vatNumber: string | null;
  isCompany: boolean;
  isActive: boolean;
  isSelfRegistered: boolean;
  status: string;
  creditLimit: number;
  notes: string | null;
  billingEmail: string | null;
  preferredPaymentMethod: string | null;
  preferredCurrency: string;
  allowCurrencyOverride: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `Email` | `string` | `string` |
| `Phone` | `string` | `string` |
| `CountryCode` | `string?` | `string | null` |
| `CustomerName` | `string?` | `string | null` |
| `TaxId` | `string?` | `string | null` |
| `VatNumber` | `string?` | `string | null` |
| `IsCompany` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `IsSelfRegistered` | `bool` | `boolean` |
| `Status` | `string` | `string` |
| `CreditLimit` | `decimal` | `number` |
| `Notes` | `string?` | `string | null` |
| `BillingEmail` | `string?` | `string | null` |
| `PreferredPaymentMethod` | `string?` | `string | null` |
| `PreferredCurrency` | `string` | `string` |
| `AllowCurrencyOverride` | `bool` | `boolean` |

## Used By Endpoints

- [POST CreateCustomer](../customers/post-create-customer-api-v1-customers.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

