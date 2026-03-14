# CreateSoldHostingPackageDto

Data transfer object for CreateSoldHostingPackageDto.

## Source

`DR_Admin/DTOs/SoldHostingPackageDto.cs`

## TypeScript Interface

```ts
export interface CreateSoldHostingPackageDto {
  customerId: number;
  hostingPackageId: number;
  registeredDomainId: number | null;
  orderId: number;
  orderLineId: number | null;
  status: string;
  billingCycle: string;
  setupFee: number;
  recurringPrice: number;
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
| `HostingPackageId` | `int` | `number` |
| `RegisteredDomainId` | `int?` | `number | null` |
| `OrderId` | `int` | `number` |
| `OrderLineId` | `int?` | `number | null` |
| `Status` | `string` | `string` |
| `BillingCycle` | `string` | `string` |
| `SetupFee` | `decimal` | `number` |
| `RecurringPrice` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `ActivatedAt` | `DateTime` | `string` |
| `NextBillingDate` | `DateTime` | `string` |
| `ExpiresAt` | `DateTime?` | `string | null` |
| `AutoRenew` | `bool` | `boolean` |
| `ConfigurationSnapshotJson` | `string` | `string` |
| `Notes` | `string` | `string` |

## Used By Endpoints

- [POST Create](../sold-hosting-packages/post-create-api-v1-sold-hosting-packages.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

