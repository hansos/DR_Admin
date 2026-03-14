# UpdateOrderDto

Data transfer object for updating an existing order

## Source

`DR_Admin/DTOs/OrderDto.cs`

## TypeScript Interface

```ts
export interface UpdateOrderDto {
  serviceId: number | null;
  status: OrderStatus;
  startDate: string;
  endDate: string;
  nextBillingDate: string;
  autoRenew: boolean;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ServiceId` | `int?` | `number | null` |
| `Status` | `OrderStatus` | `OrderStatus` |
| `StartDate` | `DateTime` | `string` |
| `EndDate` | `DateTime` | `string` |
| `NextBillingDate` | `DateTime` | `string` |
| `AutoRenew` | `bool` | `boolean` |
| `Notes` | `string` | `string` |

## Used By Endpoints

- [PUT UpdateOrder](../orders/put-update-order-api-v1-orders-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

