# UpdateVendorCostDto

Data transfer object for updating vendor costs

## Source

`DR_Admin/DTOs/UpdateVendorCostDto.cs`

## TypeScript Interface

```ts
export interface UpdateVendorCostDto {
  isRefundable: boolean;
  refundPolicy: RefundPolicy;
  refundDeadline: string | null;
  status: VendorCostStatus;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `IsRefundable` | `bool` | `boolean` |
| `RefundPolicy` | `RefundPolicy` | `RefundPolicy` |
| `RefundDeadline` | `DateTime?` | `string | null` |
| `Status` | `VendorCostStatus` | `VendorCostStatus` |
| `Notes` | `string` | `string` |

## Used By Endpoints

- [PUT UpdateVendorCost](../vendor-costs/put-update-vendor-cost-api-v1-vendor-costs-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

