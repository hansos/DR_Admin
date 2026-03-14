# CustomerAccountDto

DTO for customer information in account context

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface CustomerAccountDto {
  id: number;
  referenceNumber: number;
  customerNumber: number | null;
  name: string;
  email: string;
  phone: string;
  address: string;
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
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `ReferenceNumber` | `long` | `number` |
| `CustomerNumber` | `long?` | `number | null` |
| `Name` | `string` | `string` |
| `Email` | `string` | `string` |
| `Phone` | `string` | `string` |
| `Address` | `string` | `string` |
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

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

