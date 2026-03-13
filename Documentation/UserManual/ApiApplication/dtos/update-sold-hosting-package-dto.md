# UpdateSoldHostingPackageDto

Data transfer object for UpdateSoldHostingPackageDto.

## Source

`DR_Admin/DTOs/SoldHostingPackageDto.cs`

## TypeScript Interface

```ts
export interface UpdateSoldHostingPackageDto {
  registeredDomainId: number | null;
  status: string | null;
  billingCycle: string | null;
  setupFee: number | null;
  recurringPrice: number | null;
  currencyCode: string | null;
  activatedAt: string | null;
  nextBillingDate: string | null;
  expiresAt: string | null;
  autoRenew: boolean | null;
  configurationSnapshotJson: string | null;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `RegisteredDomainId` | `int?` | `number | null` |
| `Status` | `string?` | `string | null` |
| `BillingCycle` | `string?` | `string | null` |
| `SetupFee` | `decimal?` | `number | null` |
| `RecurringPrice` | `decimal?` | `number | null` |
| `CurrencyCode` | `string?` | `string | null` |
| `ActivatedAt` | `DateTime?` | `string | null` |
| `NextBillingDate` | `DateTime?` | `string | null` |
| `ExpiresAt` | `DateTime?` | `string | null` |
| `AutoRenew` | `bool?` | `boolean | null` |
| `ConfigurationSnapshotJson` | `string?` | `string | null` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
