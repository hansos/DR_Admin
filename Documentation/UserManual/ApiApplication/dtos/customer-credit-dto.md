# CustomerCreditDto

Data transfer object representing customer credit

## Source

`DR_Admin/DTOs/CustomerCreditDto.cs`

## TypeScript Interface

```ts
export interface CustomerCreditDto {
  id: number;
  customerId: number;
  balance: number;
  currencyCode: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `Balance` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetCustomerCredit](../customer-credits/get-get-customer-credit-api-v1-customer-credits-customer-customerid.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

