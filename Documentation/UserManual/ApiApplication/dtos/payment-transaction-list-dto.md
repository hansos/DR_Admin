# PaymentTransactionListDto

Represents a payment transaction item for list views.

## Source

`DR_Admin/DTOs/PaymentTransactionListDto.cs`

## TypeScript Interface

```ts
export interface PaymentTransactionListDto {
  id: number;
  invoiceId: number;
  invoiceNumber: string;
  customerId: number;
  customerName: string;
  paymentMethod: string;
  status: string;
  transactionId: string;
  amount: number;
  currencyCode: string;
  paymentGatewayId: number | null;
  paymentGatewayName: string;
  processedAt: string | null;
  refundedAmount: number;
  createdAt: string;
  allocations: PaymentTransactionAllocationDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `InvoiceId` | `int` | `number` |
| `InvoiceNumber` | `string` | `string` |
| `CustomerId` | `int` | `number` |
| `CustomerName` | `string` | `string` |
| `PaymentMethod` | `string` | `string` |
| `Status` | `string` | `string` |
| `TransactionId` | `string` | `string` |
| `Amount` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `PaymentGatewayId` | `int?` | `number | null` |
| `PaymentGatewayName` | `string` | `string` |
| `ProcessedAt` | `DateTime?` | `string | null` |
| `RefundedAmount` | `decimal` | `number` |
| `CreatedAt` | `DateTime` | `string` |
| `Allocations` | `List<PaymentTransactionAllocationDto>` | `PaymentTransactionAllocationDto[]` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

