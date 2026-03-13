# CreateCustomerAddressDto

Data transfer object for creating a new customer address

## Source

`DR_Admin/DTOs/CustomerAddressDto.cs`

## TypeScript Interface

```ts
export interface CreateCustomerAddressDto {
  addressTypeId: number;
  postalCodeId: number;
  addressLine1: string;
  addressLine2: string | null;
  addressLine3: string | null;
  addressLine4: string | null;
  isPrimary: boolean;
  isActive: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `AddressTypeId` | `int` | `number` |
| `PostalCodeId` | `int` | `number` |
| `AddressLine1` | `string` | `string` |
| `AddressLine2` | `string?` | `string | null` |
| `AddressLine3` | `string?` | `string | null` |
| `AddressLine4` | `string?` | `string | null` |
| `IsPrimary` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
