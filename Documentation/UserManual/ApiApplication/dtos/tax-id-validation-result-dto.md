# TaxIdValidationResultDto

Data transfer object representing tax ID validation results

## Source

`DR_Admin/DTOs/TaxIdValidationResultDto.cs`

## TypeScript Interface

```ts
export interface TaxIdValidationResultDto {
  isValid: boolean;
  validationDate: string;
  validationService: string;
  companyName: string | null;
  registeredAddress: string | null;
  rawResponse: string;
  errorMessage: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `IsValid` | `bool` | `boolean` |
| `ValidationDate` | `DateTime` | `string` |
| `ValidationService` | `string` | `string` |
| `CompanyName` | `string?` | `string | null` |
| `RegisteredAddress` | `string?` | `string | null` |
| `RawResponse` | `string` | `string` |
| `ErrorMessage` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
