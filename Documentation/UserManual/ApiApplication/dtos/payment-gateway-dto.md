# PaymentGatewayDto

Data transfer object representing a payment gateway

## Source

`DR_Admin/DTOs/PaymentGatewayDto.cs`

## TypeScript Interface

```ts
export interface PaymentGatewayDto {
  id: number;
  name: string;
  providerCode: string;
  paymentInstrument: string;
  paymentInstrumentId: number | null;
  isActive: boolean;
  isDefault: boolean;
  apiKey: string;
  useSandbox: boolean;
  webhookUrl: string;
  displayOrder: number;
  description: string;
  logoUrl: string;
  supportedCurrencies: string;
  feePercentage: number;
  fixedFee: number;
  notes: string;
  createdAt: string;
  updatedAt: string;
  deletedAt: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `ProviderCode` | `string` | `string` |
| `PaymentInstrument` | `string` | `string` |
| `PaymentInstrumentId` | `int?` | `number | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `ApiKey` | `string` | `string` |
| `UseSandbox` | `bool` | `boolean` |
| `WebhookUrl` | `string` | `string` |
| `DisplayOrder` | `int` | `number` |
| `Description` | `string` | `string` |
| `LogoUrl` | `string` | `string` |
| `SupportedCurrencies` | `string` | `string` |
| `FeePercentage` | `decimal` | `number` |
| `FixedFee` | `decimal` | `number` |
| `Notes` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `DeletedAt` | `DateTime?` | `string | null` |

## Used By Endpoints

- [GET GetDefaultPaymentGateway](../payment-gateways/get-get-default-payment-gateway-api-v1-payment-gateways-default.md)
- [GET GetPaymentGatewayById](../payment-gateways/get-get-payment-gateway-by-id-api-v1-payment-gateways-id.md)
- [GET GetPaymentGatewayByProvider](../payment-gateways/get-get-payment-gateway-by-provider-api-v1-payment-gateways-provider-providercode.md)
- [POST CreatePaymentGateway](../payment-gateways/post-create-payment-gateway-api-v1-payment-gateways.md)
- [PUT UpdatePaymentGateway](../payment-gateways/put-update-payment-gateway-api-v1-payment-gateways-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

