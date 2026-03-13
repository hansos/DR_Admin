# CreateCustomerPaymentMethodDto

Data transfer object for creating a customer payment method

## Source

`DR_Admin/DTOs/CreateCustomerPaymentMethodDto.cs`

## TypeScript Interface

```ts
export interface CreateCustomerPaymentMethodDto {
  customerId: number;
  paymentInstrument: string;
  type: PaymentMethodType;
  paymentMethodToken: string;
  isDefault: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerId` | `int` | `number` |
| `PaymentInstrument` | `string` | `string` |
| `Type` | `PaymentMethodType` | `PaymentMethodType` |
| `PaymentMethodToken` | `string` | `string` |
| `IsDefault` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
