# CreateSoldOptionalServiceDto

Data transfer object for CreateSoldOptionalServiceDto.

## Source

`DR_Admin/DTOs/SoldOptionalServiceDto.cs`

## TypeScript Interface

```ts
export interface CreateSoldOptionalServiceDto {
  customerId: number;
  serviceId: number;
  registeredDomainId: number | null;
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
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerId` | `int` | `number` |
| `ServiceId` | `int` | `number` |
| `RegisteredDomainId` | `int?` | `number | null` |
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

## Used By Endpoints

- [POST Create](../sold-optional-services/post-create-api-v1-sold-optional-services.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

