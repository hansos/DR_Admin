# CreatePaymentInstrumentDto

DTO for creating payment instrument

## Source

`DR_Admin/DTOs/PaymentInstrumentDto.cs`

## TypeScript Interface

```ts
export interface CreatePaymentInstrumentDto {
  code: string;
  name: string;
  description: string;
  isActive: boolean;
  displayOrder: number;
  defaultGatewayId: number | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `DisplayOrder` | `int` | `number` |
| `DefaultGatewayId` | `int?` | `number | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
