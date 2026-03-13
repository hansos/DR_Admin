# PatchCustomerInfoRequestDto

Request DTO for updating customer information

## Source

`DR_Admin/DTOs/MyAccountDto.cs`

## TypeScript Interface

```ts
export interface PatchCustomerInfoRequestDto {
  name: string | null;
  email: string | null;
  phone: string | null;
  address: string | null;
  customerName: string | null;
  taxId: string | null;
  vatNumber: string | null;
  isCompany: boolean | null;
  isSelfRegistered: boolean | null;
  isActive: boolean | null;
  status: string | null;
  balance: number | null;
  creditLimit: number | null;
  notes: string | null;
  billingEmail: string | null;
  preferredPaymentMethod: string | null;
  preferredCurrency: string | null;
  allowCurrencyOverride: boolean | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string?` | `string | null` |
| `Email` | `string?` | `string | null` |
| `Phone` | `string?` | `string | null` |
| `Address` | `string?` | `string | null` |
| `CustomerName` | `string?` | `string | null` |
| `TaxId` | `string?` | `string | null` |
| `VatNumber` | `string?` | `string | null` |
| `IsCompany` | `bool?` | `boolean | null` |
| `IsSelfRegistered` | `bool?` | `boolean | null` |
| `IsActive` | `bool?` | `boolean | null` |
| `Status` | `string?` | `string | null` |
| `Balance` | `decimal?` | `number | null` |
| `CreditLimit` | `decimal?` | `number | null` |
| `Notes` | `string?` | `string | null` |
| `BillingEmail` | `string?` | `string | null` |
| `PreferredPaymentMethod` | `string?` | `string | null` |
| `PreferredCurrency` | `string?` | `string | null` |
| `AllowCurrencyOverride` | `bool?` | `boolean | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
