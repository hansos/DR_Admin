# OrderDto

Data transfer object representing a customer order for a service

## Source

`DR_Admin/DTOs/OrderDto.cs`

## TypeScript Interface

```ts
export interface OrderDto {
  id: number;
  orderNumber: string;
  customerId: number;
  serviceId: number | null;
  quoteId: number | null;
  orderType: OrderType;
  status: OrderStatus;
  startDate: string;
  endDate: string;
  nextBillingDate: string;
  setupFee: number;
  recurringAmount: number;
  discountAmount: number;
  currencyCode: string;
  baseCurrencyCode: string;
  exchangeRate: number | null;
  exchangeRateDate: string | null;
  trialEndsAt: string | null;
  autoRenew: boolean;
  createdAt: string;
  updatedAt: string;
  orderLines: OrderLineDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `OrderNumber` | `string` | `string` |
| `CustomerId` | `int` | `number` |
| `ServiceId` | `int?` | `number | null` |
| `QuoteId` | `int?` | `number | null` |
| `OrderType` | `OrderType` | `OrderType` |
| `Status` | `OrderStatus` | `OrderStatus` |
| `StartDate` | `DateTime` | `string` |
| `EndDate` | `DateTime` | `string` |
| `NextBillingDate` | `DateTime` | `string` |
| `SetupFee` | `decimal` | `number` |
| `RecurringAmount` | `decimal` | `number` |
| `DiscountAmount` | `decimal` | `number` |
| `CurrencyCode` | `string` | `string` |
| `BaseCurrencyCode` | `string` | `string` |
| `ExchangeRate` | `decimal?` | `number | null` |
| `ExchangeRateDate` | `DateTime?` | `string | null` |
| `TrialEndsAt` | `DateTime?` | `string | null` |
| `AutoRenew` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `OrderLines` | `List<OrderLineDto>` | `OrderLineDto[]` |

## Used By Endpoints

- [GET GetOrderById](../orders/get-get-order-by-id-api-v1-orders-id.md)
- [POST CancelCheckoutOrder](../orders/post-cancel-checkout-order-api-v1-orders-checkout-id-int-cancel.md)
- [POST CreateCheckoutOrder](../orders/post-create-checkout-order-api-v1-orders-checkout.md)
- [POST CreateOrder](../orders/post-create-order-api-v1-orders.md)
- [PUT UpdateOrder](../orders/put-update-order-api-v1-orders-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

