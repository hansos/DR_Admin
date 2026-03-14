# DomainNameAlternativesResponseDto

Response data transfer object containing generated alternative domain names.

## Source

`DR_Admin/DTOs/DomainNameAlternativesDto.cs`

## TypeScript Interface

```ts
export interface DomainNameAlternativesResponseDto {
  inputDomainName: string;
  count: number;
  suggestions: string[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `InputDomainName` | `string` | `string` |
| `Count` | `int` | `number` |
| `Suggestions` | `List<string>` | `string[]` |

## Used By Endpoints

- [GET GetAlternativeDomainNames](../domain-manager/get-get-alternative-domain-names-api-v1-domain-manager-domain-name-domainname-alternatives.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

