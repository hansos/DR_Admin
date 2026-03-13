# InvoiceDto

Data transfer object representing an invoice

## Source

`DR_Admin/DTOs/InvoiceDto.cs`

## TypeScript Interface

```ts
export interface InvoiceDto {
  id: number;
  invoiceNumber: string;
  customerId: number;
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
  baseCurrencyCode: string;
  displayCurrencyCode: string;
  exchangeRate: number | null;
  baseTotalAmount: number | null;
  exchangeRateDate: string | null;
  customerName: string;
  customerAddress: string;
  customerTaxId: string;
  paymentReference: string;
  paymentMethod: string;
  notes: string;
  internalComment: string;
  invoiceLines: InvoiceLineDto[];
  createdAt: string;
  updatedAt: string;
  deletedAt: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `InvoiceNumber` | `string` | `string` |
| `CustomerId` | `int` | `number` |
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
| `BaseCurrencyCode` | `string` | `string` |
| `DisplayCurrencyCode` | `string` | `string` |
| `ExchangeRate` | `decimal?` | `number | null` |
| `BaseTotalAmount` | `decimal?` | `number | null` |
| `ExchangeRateDate` | `DateTime?` | `string | null` |
| `CustomerName` | `string` | `string` |
| `CustomerAddress` | `string` | `string` |
| `CustomerTaxId` | `string` | `string` |
| `PaymentReference` | `string` | `string` |
| `PaymentMethod` | `string` | `string` |
| `Notes` | `string` | `string` |
| `InternalComment` | `string` | `string` |
| `InvoiceLines` | `List<InvoiceLineDto>` | `InvoiceLineDto[]` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `DeletedAt` | `DateTime?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
