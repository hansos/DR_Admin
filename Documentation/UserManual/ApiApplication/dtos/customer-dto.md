# CustomerDto

Data transfer object representing a customer

## Source

`DR_Admin/DTOs/CustomerDto.cs`

## TypeScript Interface

```ts
export interface CustomerDto {
  id: number;
  referenceNumber: number;
  formattedReferenceNumber: string | null;
  customerNumber: number | null;
  formattedCustomerNumber: string | null;
  name: string;
  email: string;
  phone: string;
  countryCode: string | null;
  customerName: string | null;
  taxId: string | null;
  vatNumber: string | null;
  isCompany: boolean;
  isSelfRegistered: boolean;
  isActive: boolean;
  status: string;
  balance: number;
  creditLimit: number;
  notes: string | null;
  billingEmail: string | null;
  preferredPaymentMethod: string | null;
  preferredCurrency: string;
  allowCurrencyOverride: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `ReferenceNumber` | `long` | `number` |
| `FormattedReferenceNumber` | `string?` | `string | null` |
| `CustomerNumber` | `long?` | `number | null` |
| `FormattedCustomerNumber` | `string?` | `string | null` |
| `Name` | `string` | `string` |
| `Email` | `string` | `string` |
| `Phone` | `string` | `string` |
| `CountryCode` | `string?` | `string | null` |
| `CustomerName` | `string?` | `string | null` |
| `TaxId` | `string?` | `string | null` |
| `VatNumber` | `string?` | `string | null` |
| `IsCompany` | `bool` | `boolean` |
| `IsSelfRegistered` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `Status` | `string` | `string` |
| `Balance` | `decimal` | `number` |
| `CreditLimit` | `decimal` | `number` |
| `Notes` | `string?` | `string | null` |
| `BillingEmail` | `string?` | `string | null` |
| `PreferredPaymentMethod` | `string?` | `string | null` |
| `PreferredCurrency` | `string` | `string` |
| `AllowCurrencyOverride` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetAllCustomers](../customers/get-get-all-customers-api-v1-customers.md)
- [GET GetCustomerById](../customers/get-get-customer-by-id-api-v1-customers-id.md)
- [POST CreateCustomer](../customers/post-create-customer-api-v1-customers.md)
- [PUT UpdateCustomer](../customers/put-update-customer-api-v1-customers-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

