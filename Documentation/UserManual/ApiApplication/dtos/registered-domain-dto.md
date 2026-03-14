# RegisteredDomainDto

Represents a registered domain.

## Source

`DR_Admin/DTOs/RegisteredDomainDto.cs`

## TypeScript Interface

```ts
export interface RegisteredDomainDto {
  id: number;
  customerId: number;
  serviceId: number | null;
  name: string;
  providerId: number;
  status: string;
  registrationStatus: DomainRegistrationStatus;
  registrationDate: string | null;
  expirationDate: string | null;
  registrationAttemptCount: number;
  lastRegistrationAttemptUtc: string | null;
  nextRegistrationAttemptUtc: string | null;
  registrationError: string | null;
  createdAt: string;
  updatedAt: string;
  customer: CustomerDto | null;
  registrar: RegistrarDto | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `ServiceId` | `int?` | `number | null` |
| `Name` | `string` | `string` |
| `ProviderId` | `int` | `number` |
| `Status` | `string` | `string` |
| `RegistrationStatus` | `DomainRegistrationStatus` | `DomainRegistrationStatus` |
| `RegistrationDate` | `DateTime?` | `string | null` |
| `ExpirationDate` | `DateTime?` | `string | null` |
| `RegistrationAttemptCount` | `int` | `number` |
| `LastRegistrationAttemptUtc` | `DateTime?` | `string | null` |
| `NextRegistrationAttemptUtc` | `DateTime?` | `string | null` |
| `RegistrationError` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `Customer` | `CustomerDto?` | `CustomerDto | null` |
| `Registrar` | `RegistrarDto?` | `RegistrarDto | null` |

## Used By Endpoints

- [GET GetAllDomains](../registered-domains/get-get-all-domains-api-v1-registered-domains.md)
- [GET GetDomainById](../registered-domains/get-get-domain-by-id-api-v1-registered-domains-id.md)
- [GET GetDomainByName](../registered-domains/get-get-domain-by-name-api-v1-registered-domains-name-name.md)
- [POST CreateDomain](../registered-domains/post-create-domain-api-v1-registered-domains.md)
- [PUT UpdateDomain](../registered-domains/put-update-domain-api-v1-registered-domains-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

