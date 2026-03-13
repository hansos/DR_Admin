# UpdateSoldOptionalServiceDto

Data transfer object for UpdateSoldOptionalServiceDto.

## Source

`DR_Admin/DTOs/SoldOptionalServiceDto.cs`

## TypeScript Interface

```ts
export interface UpdateSoldOptionalServiceDto {
  registeredDomainId: number | null;
  quantity: number | null;
  unitPrice: number | null;
  totalPrice: number | null;
  status: string | null;
  billingCycle: string | null;
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
| `Quantity` | `int?` | `number | null` |
| `UnitPrice` | `decimal?` | `number | null` |
| `TotalPrice` | `decimal?` | `number | null` |
| `Status` | `string?` | `string | null` |
| `BillingCycle` | `string?` | `string | null` |
| `CurrencyCode` | `string?` | `string | null` |
| `ActivatedAt` | `DateTime?` | `string | null` |
| `NextBillingDate` | `DateTime?` | `string | null` |
| `ExpiresAt` | `DateTime?` | `string | null` |
| `AutoRenew` | `bool?` | `boolean | null` |
| `ConfigurationSnapshotJson` | `string?` | `string | null` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
