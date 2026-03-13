# SoldOptionalServiceDto

Data transfer object for SoldOptionalServiceDto.

## Source

`DR_Admin/DTOs/SoldOptionalServiceDto.cs`

## TypeScript Interface

```ts
export interface SoldOptionalServiceDto {
  id: number;
  customerId: number;
  serviceId: number;
  registeredDomainId: number | null;
  serviceName: string;
  connectedDomainName: string;
  orderId: number;
  orderLineId: number | null;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  status: string;
  billingCycle: string;
  currencyCode: string;
  activatedAt: string;
  nextBillingDate: string;
  expiresAt: string | null;
  autoRenew: boolean;
  configurationSnapshotJson: string;
  notes: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `ServiceId` | `int` | `number` |
| `RegisteredDomainId` | `int?` | `number | null` |
| `ServiceName` | `string` | `string` |
| `ConnectedDomainName` | `string` | `string` |
| `OrderId` | `int` | `number` |
| `OrderLineId` | `int?` | `number | null` |
| `Quantity` | `int` | `number` |
| `UnitPrice` | `decimal` | `number` |
| `TotalPrice` | `decimal` | `number` |
| `Status` | `string` | `string` |
| `BillingCycle` | `string` | `string` |
| `CurrencyCode` | `string` | `string` |
| `ActivatedAt` | `DateTime` | `string` |
| `NextBillingDate` | `DateTime` | `string` |
| `ExpiresAt` | `DateTime?` | `string | null` |
| `AutoRenew` | `bool` | `boolean` |
| `ConfigurationSnapshotJson` | `string` | `string` |
| `Notes` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
