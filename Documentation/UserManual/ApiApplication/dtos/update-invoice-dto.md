# UpdateInvoiceDto

Data transfer object for updating an existing invoice

## Source

`DR_Admin/DTOs/InvoiceDto.cs`

## TypeScript Interface

```ts
export interface UpdateInvoiceDto {
  invoiceNumber: string;
  orderId: number | null;
  orderTaxSnapshotId: number | null;
  status: InvoiceStatus;
  issueDate: string;
  dueDate: string;
  paidAt: string | null;
  subTotal: number;
  taxAmount: number;
  totalAmount: number;
  amountPaid: number;
  amountDue: number;
  currencyCode: string;
  taxRate: number;
  taxName: string;
  customerName: string;
  customerAddress: string;
  customerTaxId: string;
  paymentReference: string;
  paymentMethod: string;
  notes: string;
  internalComment: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `InvoiceNumber` | `string` | `string` |
| `OrderId` | `int?` | `number | null` |
| `OrderTaxSnapshotId` | `int?` | `number | null` |
| `Status` | `InvoiceStatus` | `InvoiceStatus` |
| `IssueDate` | `DateTime` | `string` |
| `DueDate` | `DateTime` | `string` |
| `PaidAt` | `DateTime?` | `string | null` |
| `SubTotal` | `decimal` | `number` |
| `TaxAmount` | `decimal` | `number` |
| `TotalAmount` | `decimal` | `number` |
| `AmountPaid` | `decimal` | `number` |
| `AmountDue` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `TaxRate` | `decimal` | `number` |
| `TaxName` | `string` | `string` |
| `CustomerName` | `string` | `string` |
| `CustomerAddress` | `string` | `string` |
| `CustomerTaxId` | `string` | `string` |
| `PaymentReference` | `string` | `string` |
| `PaymentMethod` | `string` | `string` |
| `Notes` | `string` | `string` |
| `InternalComment` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
