# HostingDomainDto

Data transfer object for HostingDomainDto.

## Source

`DR_Admin/DTOs/HostingAccountDto.cs`

## TypeScript Interface

```ts
export interface HostingDomainDto {
  id: number;
  hostingAccountId: number;
  domainName: string;
  domainType: string;
  documentRoot: string | null;
  sslEnabled: boolean;
  sslExpirationDate: string | null;
  sslIssuer: string | null;
  phpEnabled: boolean;
  phpVersion: string | null;
  externalDomainId: string | null;
  lastSyncedAt: string | null;
  syncStatus: string | null;
  createdAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `HostingAccountId` | `int` | `number` |
| `DomainName` | `string` | `string` |
| `DomainType` | `string` | `string` |
| `DocumentRoot` | `string?` | `string | null` |
| `SslEnabled` | `bool` | `boolean` |
| `SslExpirationDate` | `DateTime?` | `string | null` |
| `SslIssuer` | `string?` | `string | null` |
| `PhpEnabled` | `bool` | `boolean` |
| `PhpVersion` | `string?` | `string | null` |
| `ExternalDomainId` | `string?` | `string | null` |
| `LastSyncedAt` | `DateTime?` | `string | null` |
| `SyncStatus` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetDomain](../hosting-domains/get-get-domain-api-v1-hosting-accounts-hostingaccountid-domains-id.md)
- [POST CreateDomain](../hosting-domains/post-create-domain-api-v1-hosting-accounts-hostingaccountid-domains.md)
- [PUT UpdateDomain](../hosting-domains/put-update-domain-api-v1-hosting-accounts-hostingaccountid-domains-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

