# ProcessVendorPayoutDto

Data transfer object for processing vendor payouts

## Source

`DR_Admin/DTOs/ProcessVendorPayoutDto.cs`

## TypeScript Interface

```ts
export interface ProcessVendorPayoutDto {
  vendorPayoutId: number;
  transactionReference: string;
  paymentGatewayResponse: string;
  isSuccessful: boolean;
  failureReason: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `VendorPayoutId` | `int` | `number` |
| `TransactionReference` | `string` | `string` |
| `PaymentGatewayResponse` | `string` | `string` |
| `IsSuccessful` | `bool` | `boolean` |
| `FailureReason` | `string?` | `string | null` |

## Used By Endpoints

- [POST ProcessVendorPayout](../vendor-payouts/post-process-vendor-payout-api-v1-vendor-payouts-process.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

