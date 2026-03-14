# CustomerPaymentMethodDto

Data transfer object representing a customer payment method

## Source

`DR_Admin/DTOs/CustomerPaymentMethodDto.cs`

## TypeScript Interface

```ts
export interface CustomerPaymentMethodDto {
  id: number;
  customerId: number;
  paymentGatewayId: number;
  paymentGatewayName: string;
  type: PaymentMethodType;
  last4Digits: string;
  expiryMonth: number | null;
  expiryYear: number | null;
  cardBrand: string;
  cardholderName: string;
  isDefault: boolean;
  isActive: boolean;
  isVerified: boolean;
  createdAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `PaymentGatewayId` | `int` | `number` |
| `PaymentGatewayName` | `string` | `string` |
| `Type` | `PaymentMethodType` | `PaymentMethodType` |
| `Last4Digits` | `string` | `string` |
| `ExpiryMonth` | `int?` | `number | null` |
| `ExpiryYear` | `int?` | `number | null` |
| `CardBrand` | `string` | `string` |
| `CardholderName` | `string` | `string` |
| `IsDefault` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `IsVerified` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetDefaultPaymentMethod](../customer-payment-methods/get-get-default-payment-method-api-v1-customer-payment-methods-customer-customerid-default.md)
- [GET GetPaymentMethodById](../customer-payment-methods/get-get-payment-method-by-id-api-v1-customer-payment-methods-id.md)
- [POST CreateMyPaymentMethod](../customer-payment-methods/post-create-my-payment-method-api-v1-customer-payment-methods-mine.md)
- [POST CreatePaymentMethod](../customer-payment-methods/post-create-payment-method-api-v1-customer-payment-methods.md)
- [PUT UpdateMyPaymentMethod](../customer-payment-methods/put-update-my-payment-method-api-v1-customer-payment-methods-mine-id.md)
- [PUT UpdatePaymentMethod](../customer-payment-methods/put-update-payment-method-api-v1-customer-payment-methods-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

