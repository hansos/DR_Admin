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

## Used By Endpoints

- [POST CreateMyPaymentMethod](../customer-payment-methods/post-create-my-payment-method-api-v1-customer-payment-methods-mine.md)
- [POST CreatePaymentMethod](../customer-payment-methods/post-create-payment-method-api-v1-customer-payment-methods.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

