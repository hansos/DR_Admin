# RegisterDomainDto

DTO for customer self-service domain registration

## Source

`DR_Admin/DTOs/DomainRegistrationDto.cs`

## TypeScript Interface

```ts
export interface RegisterDomainDto {
  domainName: string;
  years: number;
  autoRenew: boolean;
  privacyProtection: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `DomainName` | `string` | `string` |
| `Years` | `int` | `number` |
| `AutoRenew` | `bool` | `boolean` |
| `PrivacyProtection` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

## Used By Endpoints

- [POST RegisterDomain](../registered-domains/post-register-domain-api-v1-registered-domains-register.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

