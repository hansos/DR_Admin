# CreateInvoiceDto

Data transfer object for creating a new invoice

## Source

`DR_Admin/DTOs/InvoiceDto.cs`

## TypeScript Interface

```ts
export interface CreateInvoiceDto {
  invoiceNumber: string;
  customerId: number;
  orderId: number | null;
  orderTaxSnapshotId: number | null;
  status: InvoiceStatus;
  issueDate: string;
  dueDate: string;
  subTotal: number;
  taxAmount: number;
  totalAmount: number;
  currencyCode: string;
  taxRate: number;
  taxName: string;
  displayCurrencyCode: string | null;
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
| `CustomerId` | `int` | `number` |
| `OrderId` | `int?` | `number | null` |
| `OrderTaxSnapshotId` | `int?` | `number | null` |
| `Status` | `InvoiceStatus` | `InvoiceStatus` |
| `IssueDate` | `DateTime` | `string` |
| `DueDate` | `DateTime` | `string` |
| `SubTotal` | `decimal` | `number` |
| `TaxAmount` | `decimal` | `number` |
| `TotalAmount` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `TaxRate` | `decimal` | `number` |
| `TaxName` | `string` | `string` |
| `DisplayCurrencyCode` | `string?` | `string | null` |
| `CustomerName` | `string` | `string` |
| `CustomerAddress` | `string` | `string` |
| `CustomerTaxId` | `string` | `string` |
| `PaymentReference` | `string` | `string` |
| `PaymentMethod` | `string` | `string` |
| `Notes` | `string` | `string` |
| `InternalComment` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
