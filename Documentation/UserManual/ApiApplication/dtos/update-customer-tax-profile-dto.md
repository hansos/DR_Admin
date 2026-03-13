# UpdateCustomerTaxProfileDto

Data transfer object for updating customer tax profiles

## Source

`DR_Admin/DTOs/UpdateCustomerTaxProfileDto.cs`

## TypeScript Interface

```ts
export interface UpdateCustomerTaxProfileDto {
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
| `TaxIdNumber` | `string?` | `string | null` |
| `TaxIdType` | `TaxIdType` | `TaxIdType` |
| `TaxResidenceCountry` | `string` | `string` |
| `CustomerType` | `CustomerType` | `CustomerType` |
| `TaxExempt` | `bool` | `boolean` |
| `TaxExemptionReason` | `string?` | `string | null` |
| `TaxExemptionCertificateUrl` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
