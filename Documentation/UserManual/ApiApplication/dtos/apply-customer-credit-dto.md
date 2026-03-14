# ApplyCustomerCreditDto

Request to apply customer credit to invoice

## Source

`DR_Admin/DTOs/PaymentProcessingDtos.cs`

## TypeScript Interface

```ts
export interface ApplyCustomerCreditDto {
  invoiceId: number;
  amount: number;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `InvoiceId` | `int` | `number` |
| `Amount` | `decimal` | `number` |
| `Notes` | `string` | `string` |

## Used By Endpoints

- [POST ApplyCustomerCredit](../payments/post-apply-customer-credit-api-v1-payments-apply-credit.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

