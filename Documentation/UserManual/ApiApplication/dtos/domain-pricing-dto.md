# DomainPricingDto

DTO for getting domain pricing

## Source

`DR_Admin/DTOs/DomainRegistrationDto.cs`

## TypeScript Interface

```ts
export interface DomainPricingDto {
  tld: string;
  registrarId: number;
  registrarName: string;
  registrationPrice: number;
  renewalPrice: number;
  transferPrice: number;
  currency: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Tld` | `string` | `string` |
| `RegistrarId` | `int` | `number` |
| `RegistrarName` | `string` | `string` |
| `RegistrationPrice` | `decimal` | `number` |
| `RenewalPrice` | `decimal` | `number` |
| `TransferPrice` | `decimal` | `number` |
| `Currency` | `string` | `string` |

## Used By Endpoints

- [GET GetDomainPricing](../registered-domains/get-get-domain-pricing-api-v1-registered-domains-pricing-tld.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

