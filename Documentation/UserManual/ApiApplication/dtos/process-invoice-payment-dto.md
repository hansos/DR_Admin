# ProcessInvoicePaymentDto

Request to process an invoice payment

## Source

`DR_Admin/DTOs/PaymentProcessingDtos.cs`

## TypeScript Interface

```ts
export interface ProcessInvoicePaymentDto {
  invoiceId: number;
  customerPaymentMethodId: number;
  ipAddress: string;
  userAgent: string;
  returnUrl: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `InvoiceId` | `int` | `number` |
| `CustomerPaymentMethodId` | `int` | `number` |
| `IpAddress` | `string` | `string` |
| `UserAgent` | `string` | `string` |
| `ReturnUrl` | `string` | `string` |

## Used By Endpoints

- [POST ProcessInvoicePayment](../payments/post-process-invoice-payment-api-v1-payments-process.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

