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

## Used By Endpoints

- [PUT UpdateMyPaymentMethod](../customer-payment-methods/put-update-my-payment-method-api-v1-customer-payment-methods-mine-id.md)
- [PUT UpdatePaymentMethod](../customer-payment-methods/put-update-payment-method-api-v1-customer-payment-methods-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

