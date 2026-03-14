# InvoiceLineDto

Data transfer object representing a line item on an invoice

## Source

`DR_Admin/DTOs/InvoiceLineDto.cs`

## TypeScript Interface

```ts
export interface InvoiceLineDto {
  id: number;
  invoiceId: number;
  serviceId: number | null;
  unitId: number;
  lineNumber: number;
  description: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  totalPrice: number;
  taxRate: number;
  taxAmount: number;
  totalWithTax: number;
  serviceNameSnapshot: string;
  accountingCode: string;
  createdAt: string;
  updatedAt: string;
  deletedAt: string | null;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `InvoiceId` | `int` | `number` |
| `ServiceId` | `int?` | `number | null` |
| `UnitId` | `int` | `number` |
| `LineNumber` | `int` | `number` |
| `Description` | `string` | `string` |
| `Quantity` | `int` | `number` |
| `UnitPrice` | `decimal` | `number` |
| `Discount` | `decimal` | `number` |
| `TotalPrice` | `decimal` | `number` |
| `TaxRate` | `decimal` | `number` |
| `TaxAmount` | `decimal` | `number` |
| `TotalWithTax` | `decimal` | `number` |
| `ServiceNameSnapshot` | `string` | `string` |
| `AccountingCode` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `DeletedAt` | `DateTime?` | `string | null` |
| `Notes` | `string` | `string` |

## Used By Endpoints

- [GET GetAllInvoiceLines](../invoice-lines/get-get-all-invoice-lines-api-v1-invoice-lines.md)
- [GET GetInvoiceLineById](../invoice-lines/get-get-invoice-line-by-id-api-v1-invoice-lines-id.md)
- [POST CreateInvoiceLine](../invoice-lines/post-create-invoice-line-api-v1-invoice-lines.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

