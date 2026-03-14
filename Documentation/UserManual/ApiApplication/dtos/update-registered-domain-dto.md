# UpdateRegisteredDomainDto

Represents the payload for updating a registered domain.

## Source

`DR_Admin/DTOs/RegisteredDomainDto.cs`

## TypeScript Interface

```ts
export interface UpdateRegisteredDomainDto {
  customerId: number;
  serviceId: number | null;
  name: string;
  providerId: number;
  status: string;
  registrationStatus: DomainRegistrationStatus;
  registrationDate: string | null;
  registrationAttemptCount: number;
  lastRegistrationAttemptUtc: string | null;
  nextRegistrationAttemptUtc: string | null;
  registrationError: string | null;
  expirationDate: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerId` | `int` | `number` |
| `ServiceId` | `int?` | `number | null` |
| `Name` | `string` | `string` |
| `ProviderId` | `int` | `number` |
| `Status` | `string` | `string` |
| `RegistrationStatus` | `DomainRegistrationStatus` | `DomainRegistrationStatus` |
| `RegistrationDate` | `DateTime?` | `string | null` |
| `RegistrationAttemptCount` | `int` | `number` |
| `LastRegistrationAttemptUtc` | `DateTime?` | `string | null` |
| `NextRegistrationAttemptUtc` | `DateTime?` | `string | null` |
| `RegistrationError` | `string?` | `string | null` |
| `ExpirationDate` | `DateTime?` | `string | null` |

## Used By Endpoints

- [PUT UpdateDomain](../registered-domains/put-update-domain-api-v1-registered-domains-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

