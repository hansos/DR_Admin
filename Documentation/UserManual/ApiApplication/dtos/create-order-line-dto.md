# CreateOrderLineDto

Data transfer object for creating a new order line

## Source

`DR_Admin/DTOs/OrderDto.cs`

## TypeScript Interface

```ts
export interface CreateOrderLineDto {
  serviceId: number | null;
  description: string;
  quantity: number;
  unitPrice: number;
  isRecurring: boolean;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ServiceId` | `int?` | `number | null` |
| `Description` | `string` | `string` |
| `Quantity` | `int` | `number` |
| `UnitPrice` | `decimal` | `number` |
| `IsRecurring` | `bool` | `boolean` |
| `Notes` | `string` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

