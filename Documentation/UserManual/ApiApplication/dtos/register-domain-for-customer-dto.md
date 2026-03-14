# RegisterDomainForCustomerDto

DTO for sales/admin registering domain for a specific customer

## Source

`DR_Admin/DTOs/DomainRegistrationDto.cs`

## TypeScript Interface

```ts
export interface RegisterDomainForCustomerDto {
  customerId: number;
  domainName: string;
  registrarId: number;
  years: number;
  autoRenew: boolean;
  privacyProtection: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerId` | `int` | `number` |
| `DomainName` | `string` | `string` |
| `RegistrarId` | `int` | `number` |
| `Years` | `int` | `number` |
| `AutoRenew` | `bool` | `boolean` |
| `PrivacyProtection` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

## Used By Endpoints

- [POST RegisterDomainForCustomer](../registered-domains/post-register-domain-for-customer-api-v1-registered-domains-register-for-customer.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

