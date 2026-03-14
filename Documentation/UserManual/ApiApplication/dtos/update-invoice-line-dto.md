# UpdateInvoiceLineDto

Data transfer object for creating a new invoice line item

## Source

`DR_Admin/DTOs/InvoiceLineDto.cs`

## TypeScript Interface

```ts
export interface UpdateInvoiceLineDto {
  invoiceId: number;
  serviceId: number | null;
  lineNumber: number;
  description: string;
  unit: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  totalPrice: number;
  taxRate: number;
  taxAmount: number;
  totalWithTax: number;
  serviceNameSnapshot: string;
  accountingCode: string;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `InvoiceId` | `int` | `number` |
| `ServiceId` | `int?` | `number | null` |
| `LineNumber` | `int` | `number` |
| `Description` | `string` | `string` |
| `Unit` | `string` | `string` |
| `Quantity` | `int` | `number` |
| `UnitPrice` | `decimal` | `number` |
| `Discount` | `decimal` | `number` |
| `TotalPrice` | `decimal` | `number` |
| `TaxRate` | `decimal` | `number` |
| `TaxAmount` | `decimal` | `number` |
| `TotalWithTax` | `decimal` | `number` |
| `ServiceNameSnapshot` | `string` | `string` |
| `AccountingCode` | `string` | `string` |
| `Notes` | `string` | `string` |

## Used By Endpoints

- [PUT UpdateInvoiceLine](../invoice-lines/put-update-invoice-line-api-v1-invoice-lines-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

