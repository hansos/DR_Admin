# UpdateTldDto

Data transfer object for updating an existing TLD

## Source

`DR_Admin/DTOs/TldDto.cs`

## TypeScript Interface

```ts
export interface UpdateTldDto {
  extension: string;
  description: string;
  isActive: boolean;
  defaultRegistrationYears: number | null;
  maxRegistrationYears: number | null;
  requiresPrivacy: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Extension` | `string` | `string` |
| `Description` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `DefaultRegistrationYears` | `int?` | `number | null` |
| `MaxRegistrationYears` | `int?` | `number | null` |
| `RequiresPrivacy` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

## Used By Endpoints

- [PUT UpdateTld](../tlds/put-update-tld-api-v1-tlds-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

