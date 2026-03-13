# ImportRegistrarTldsDto

Data transfer object for importing TLDs for a registrar from form data

## Source

`DR_Admin/DTOs/TldDto.cs`

## TypeScript Interface

```ts
export interface ImportRegistrarTldsDto {
  content: string;
  defaultRegistrationCost: number | null;
  defaultRegistrationPrice: number | null;
  defaultRenewalCost: number | null;
  defaultRenewalPrice: number | null;
  defaultTransferCost: number | null;
  defaultTransferPrice: number | null;
  isAvailable: boolean;
  activateNewTlds: boolean;
  currency: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Content` | `string` | `string` |
| `DefaultRegistrationCost` | `decimal?` | `number | null` |
| `DefaultRegistrationPrice` | `decimal?` | `number | null` |
| `DefaultRenewalCost` | `decimal?` | `number | null` |
| `DefaultRenewalPrice` | `decimal?` | `number | null` |
| `DefaultTransferCost` | `decimal?` | `number | null` |
| `DefaultTransferPrice` | `decimal?` | `number | null` |
| `IsAvailable` | `bool` | `boolean` |
| `ActivateNewTlds` | `bool` | `boolean` |
| `Currency` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
