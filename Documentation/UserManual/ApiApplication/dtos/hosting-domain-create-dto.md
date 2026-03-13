# HostingDomainCreateDto

Data transfer object for HostingDomainCreateDto.

## Source

`DR_Admin/DTOs/HostingResourceDto.cs`

## TypeScript Interface

```ts
export interface HostingDomainCreateDto {
  hostingAccountId: number;
  domainName: string;
  domainType: string;
  documentRoot: string | null;
  phpEnabled: boolean;
  phpVersion: string | null;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `HostingAccountId` | `int` | `number` |
| `DomainName` | `string` | `string` |
| `DomainType` | `string` | `string` |
| `DocumentRoot` | `string?` | `string | null` |
| `PhpEnabled` | `bool` | `boolean` |
| `PhpVersion` | `string?` | `string | null` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
