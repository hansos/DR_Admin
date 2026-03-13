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

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
