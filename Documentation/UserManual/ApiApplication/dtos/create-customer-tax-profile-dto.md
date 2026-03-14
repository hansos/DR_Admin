# CreateCustomerTaxProfileDto

Data transfer object for creating customer tax profiles

## Source

`DR_Admin/DTOs/CreateCustomerTaxProfileDto.cs`

## TypeScript Interface

```ts
export interface CreateCustomerTaxProfileDto {
  customerId: number;
  taxIdNumber: string | null;
  taxIdType: TaxIdType;
  taxResidenceCountry: string;
  customerType: CustomerType;
  taxExempt: boolean;
  taxExemptionReason: string | null;
  taxExemptionCertificateUrl: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerId` | `int` | `number` |
| `TaxIdNumber` | `string?` | `string | null` |
| `TaxIdType` | `TaxIdType` | `TaxIdType` |
| `TaxResidenceCountry` | `string` | `string` |
| `CustomerType` | `CustomerType` | `CustomerType` |
| `TaxExempt` | `bool` | `boolean` |
| `TaxExemptionReason` | `string?` | `string | null` |
| `TaxExemptionCertificateUrl` | `string?` | `string | null` |

## Used By Endpoints

- [POST CreateCustomerTaxProfile](../customer-tax-profiles/post-create-customer-tax-profile-api-v1-customer-tax-profiles.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

