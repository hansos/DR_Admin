# OrderLineDto

Data transfer object representing a line item in an order

## Source

`DR_Admin/DTOs/OrderDto.cs`

## TypeScript Interface

```ts
export interface OrderLineDto {
  id: number;
  serviceId: number | null;
  lineNumber: number;
  description: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  isRecurring: boolean;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `ServiceId` | `int?` | `number | null` |
| `LineNumber` | `int` | `number` |
| `Description` | `string` | `string` |
| `Quantity` | `int` | `number` |
| `UnitPrice` | `decimal` | `number` |
| `TotalPrice` | `decimal` | `number` |
| `IsRecurring` | `bool` | `boolean` |
| `Notes` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
