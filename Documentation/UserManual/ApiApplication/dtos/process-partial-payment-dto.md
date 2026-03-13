# ProcessPartialPaymentDto

Request for partial payment

## Source

`DR_Admin/DTOs/PaymentProcessingDtos.cs`

## TypeScript Interface

```ts
export interface ProcessPartialPaymentDto {
  invoiceId: number;
  amount: number;
  customerPaymentMethodId: number;
  ipAddress: string;
  userAgent: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `InvoiceId` | `int` | `number` |
| `Amount` | `decimal` | `number` |
| `CustomerPaymentMethodId` | `int` | `number` |
| `IpAddress` | `string` | `string` |
| `UserAgent` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
