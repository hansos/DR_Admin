# PaymentInstrumentDto

Payment instrument data transfer object

## Source

`DR_Admin/DTOs/PaymentInstrumentDto.cs`

## TypeScript Interface

```ts
export interface PaymentInstrumentDto {
  id: number;
  code: string;
  name: string;
  description: string;
  isActive: boolean;
  displayOrder: number;
  defaultGatewayId: number | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `DisplayOrder` | `int` | `number` |
| `DefaultGatewayId` | `int?` | `number | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
