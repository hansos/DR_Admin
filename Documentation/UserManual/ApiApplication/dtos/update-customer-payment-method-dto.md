# UpdateCustomerPaymentMethodDto

Data transfer object for updating a customer payment method

## Source

`DR_Admin/DTOs/UpdateCustomerPaymentMethodDto.cs`

## TypeScript Interface

```ts
export interface UpdateCustomerPaymentMethodDto {
  paymentInstrument: string;
  type: PaymentMethodType;
  isDefault: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `PaymentInstrument` | `string` | `string` |
| `Type` | `PaymentMethodType` | `PaymentMethodType` |
| `IsDefault` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
