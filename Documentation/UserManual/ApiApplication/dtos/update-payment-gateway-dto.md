# UpdatePaymentGatewayDto

Data transfer object for updating an existing payment gateway

## Source

`DR_Admin/DTOs/PaymentGatewayDto.cs`

## TypeScript Interface

```ts
export interface UpdatePaymentGatewayDto {
  name: string;
  providerCode: string;
  paymentInstrument: string;
  paymentInstrumentId: number | null;
  isActive: boolean;
  isDefault: boolean;
  apiKey: string | null;
  apiSecret: string | null;
  configurationJson: string;
  useSandbox: boolean;
  webhookUrl: string;
  webhookSecret: string | null;
  displayOrder: number;
  description: string;
  logoUrl: string;
  supportedCurrencies: string;
  feePercentage: number;
  fixedFee: number;
  notes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `ProviderCode` | `string` | `string` |
| `PaymentInstrument` | `string` | `string` |
| `PaymentInstrumentId` | `int?` | `number | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `ApiKey` | `string?` | `string | null` |
| `ApiSecret` | `string?` | `string | null` |
| `ConfigurationJson` | `string` | `string` |
| `UseSandbox` | `bool` | `boolean` |
| `WebhookUrl` | `string` | `string` |
| `WebhookSecret` | `string?` | `string | null` |
| `DisplayOrder` | `int` | `number` |
| `Description` | `string` | `string` |
| `LogoUrl` | `string` | `string` |
| `SupportedCurrencies` | `string` | `string` |
| `FeePercentage` | `decimal` | `number` |
| `FixedFee` | `decimal` | `number` |
| `Notes` | `string` | `string` |

## Used By Endpoints

- [PUT UpdatePaymentGateway](../payment-gateways/put-update-payment-gateway-api-v1-payment-gateways-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

