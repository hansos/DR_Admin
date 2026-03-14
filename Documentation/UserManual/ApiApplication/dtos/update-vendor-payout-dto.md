# UpdateVendorPayoutDto

Data transfer object for updating vendor payouts

## Source

`DR_Admin/DTOs/UpdateVendorPayoutDto.cs`

## TypeScript Interface

```ts
export interface UpdateVendorPayoutDto {
  status: VendorPayoutStatus;
  scheduledDate: string;
  failureReason: string;
  transactionReference: string;
  requiresManualIntervention: boolean;
  interventionReason: string;
  internalNotes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Status` | `VendorPayoutStatus` | `VendorPayoutStatus` |
| `ScheduledDate` | `DateTime` | `string` |
| `FailureReason` | `string` | `string` |
| `TransactionReference` | `string` | `string` |
| `RequiresManualIntervention` | `bool` | `boolean` |
| `InterventionReason` | `string` | `string` |
| `InternalNotes` | `string` | `string` |

## Used By Endpoints

- [PUT UpdateVendorPayout](../vendor-payouts/put-update-vendor-payout-api-v1-vendor-payouts-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

