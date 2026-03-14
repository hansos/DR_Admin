# BulkUpdateRegistrarTldStatusByTldDto

Data transfer object for bulk updating the active status of specific registrar-TLD offerings by TLD extensions

## Source

`DR_Admin/DTOs/RegistrarTldDto.cs`

## TypeScript Interface

```ts
export interface BulkUpdateRegistrarTldStatusByTldDto {
  registrarId: number | null;
  tldExtensions: string;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `RegistrarId` | `int?` | `number | null` |
| `TldExtensions` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |

## Used By Endpoints

- [PUT BulkUpdateRegistrarTldStatusByTld](../registrar-tlds/put-bulk-update-registrar-tld-status-by-tld-api-v1-registrar-tlds-bulk-update-status-by-tld.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

