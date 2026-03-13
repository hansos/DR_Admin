# CreatePaymentGatewayDto

Data transfer object for creating a new payment gateway

## Source

`DR_Admin/DTOs/PaymentGatewayDto.cs`

## TypeScript Interface

```ts
export interface CreatePaymentGatewayDto {
  name: string;
  providerCode: string;
  paymentInstrument: string;
  paymentInstrumentId: number | null;
  isActive: boolean;
  isDefault: boolean;
  apiKey: string;
  apiSecret: string;
  configurationJson: string;
  useSandbox: boolean;
  webhookUrl: string;
  webhookSecret: string;
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
| `ApiKey` | `string` | `string` |
| `ApiSecret` | `string` | `string` |
| `ConfigurationJson` | `string` | `string` |
| `UseSandbox` | `bool` | `boolean` |
| `WebhookUrl` | `string` | `string` |
| `WebhookSecret` | `string` | `string` |
| `DisplayOrder` | `int` | `number` |
| `Description` | `string` | `string` |
| `LogoUrl` | `string` | `string` |
| `SupportedCurrencies` | `string` | `string` |
| `FeePercentage` | `decimal` | `number` |
| `FixedFee` | `decimal` | `number` |
| `Notes` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
