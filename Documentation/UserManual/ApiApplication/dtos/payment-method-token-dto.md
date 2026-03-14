# PaymentMethodTokenDto

Payment method token DTO

## Source

`DR_Admin/DTOs/PaymentProcessingDtos.cs`

## TypeScript Interface

```ts
export interface PaymentMethodTokenDto {
  id: number;
  customerPaymentMethodId: number;
  gatewayCustomerId: string;
  gatewayPaymentMethodId: string;
  expiresAt: string | null;
  last4Digits: string;
  cardBrand: string;
  expiryMonth: number | null;
  expiryYear: number | null;
  isDefault: boolean;
  isVerified: boolean;
  verifiedAt: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerPaymentMethodId` | `int` | `number` |
| `GatewayCustomerId` | `string` | `string` |
| `GatewayPaymentMethodId` | `string` | `string` |
| `ExpiresAt` | `DateTime?` | `string | null` |
| `Last4Digits` | `string` | `string` |
| `CardBrand` | `string` | `string` |
| `ExpiryMonth` | `int?` | `number | null` |
| `ExpiryYear` | `int?` | `number | null` |
| `IsDefault` | `bool` | `boolean` |
| `IsVerified` | `bool` | `boolean` |
| `VerifiedAt` | `DateTime?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

