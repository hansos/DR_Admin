# CustomerAddressDto

Data transfer object representing a customer address

## Source

`DR_Admin/DTOs/CustomerAddressDto.cs`

## TypeScript Interface

```ts
export interface CustomerAddressDto {
  id: number;
  customerId: number;
  addressTypeId: number;
  addressTypeCode: string;
  addressTypeName: string;
  postalCodeId: number;
  addressLine1: string;
  addressLine2: string | null;
  addressLine3: string | null;
  addressLine4: string | null;
  city: string;
  state: string | null;
  postalCode: string;
  countryCode: string;
  isPrimary: boolean;
  isActive: boolean;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `AddressTypeId` | `int` | `number` |
| `AddressTypeCode` | `string` | `string` |
| `AddressTypeName` | `string` | `string` |
| `PostalCodeId` | `int` | `number` |
| `AddressLine1` | `string` | `string` |
| `AddressLine2` | `string?` | `string | null` |
| `AddressLine3` | `string?` | `string | null` |
| `AddressLine4` | `string?` | `string | null` |
| `City` | `string` | `string` |
| `State` | `string?` | `string | null` |
| `PostalCode` | `string` | `string` |
| `CountryCode` | `string` | `string` |
| `IsPrimary` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
