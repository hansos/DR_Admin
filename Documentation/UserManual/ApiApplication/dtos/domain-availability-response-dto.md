# DomainAvailabilityResponseDto

Response DTO for domain availability check

## Source

`DR_Admin/DTOs/DomainRegistrationDto.cs`

## TypeScript Interface

```ts
export interface DomainAvailabilityResponseDto {
  domainName: string;
  isAvailable: boolean;
  message: string;
  price: number | null;
  currency: string | null;
  isPremium: boolean;
  suggestedAlternatives: string[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `DomainName` | `string` | `string` |
| `IsAvailable` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `Price` | `decimal?` | `number | null` |
| `Currency` | `string?` | `string | null` |
| `IsPremium` | `bool` | `boolean` |
| `SuggestedAlternatives` | `List<string>` | `string[]` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
