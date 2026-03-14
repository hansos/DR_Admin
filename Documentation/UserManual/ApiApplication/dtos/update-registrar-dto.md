# UpdateRegistrarDto

Data transfer object for updating an existing registrar

## Source

`DR_Admin/DTOs/RegistrarDto.cs`

## TypeScript Interface

```ts
export interface UpdateRegistrarDto {
  name: string;
  code: string;
  isActive: boolean;
  contactEmail: string | null;
  contactPhone: string | null;
  website: string | null;
  notes: string | null;
  isDefault: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `Code` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `ContactEmail` | `string?` | `string | null` |
| `ContactPhone` | `string?` | `string | null` |
| `Website` | `string?` | `string | null` |
| `Notes` | `string?` | `string | null` |
| `IsDefault` | `bool` | `boolean` |

## Used By Endpoints

- [PUT UpdateRegistrar](../registrars/put-update-registrar-api-v1-registrars-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

