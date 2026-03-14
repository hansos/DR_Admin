# DnsTroubleshootReportDto

Represents the troubleshooting report for DNS checks on a domain.

## Source

`DR_Admin/DTOs/DnsTroubleshootDto.cs`

## TypeScript Interface

```ts
export interface DnsTroubleshootReportDto {
  domainId: number;
  domainName: string;
  generatedAtUtc: string;
  tests: DnsTroubleshootTestResultDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `DomainId` | `int` | `number` |
| `DomainName` | `string` | `string` |
| `GeneratedAtUtc` | `DateTime` | `string` |
| `Tests` | `List<DnsTroubleshootTestResultDto>` | `DnsTroubleshootTestResultDto[]` |

## Used By Endpoints

- [GET RunForDomain](../dns-troubleshoot/get-run-for-domain-api-v1-dns-troubleshoot-domain-domainid-int.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

