# SoldHostingPackageDto

Data transfer object for SoldHostingPackageDto.

## Source

`DR_Admin/DTOs/SoldHostingPackageDto.cs`

## TypeScript Interface

```ts
export interface SoldHostingPackageDto {
  id: number;
  customerId: number;
  hostingPackageId: number;
  registeredDomainId: number | null;
  connectedDomainName: string;
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
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `HostingPackageId` | `int` | `number` |
| `RegisteredDomainId` | `int?` | `number | null` |
| `ConnectedDomainName` | `string` | `string` |
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
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetById](../sold-hosting-packages/get-get-by-id-api-v1-sold-hosting-packages-id-int.md)
- [POST Create](../sold-hosting-packages/post-create-api-v1-sold-hosting-packages.md)
- [PUT Update](../sold-hosting-packages/put-update-api-v1-sold-hosting-packages-id-int.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

