# BulkUpdateRegistrarTldStatusDto

Data transfer object for bulk updating the active status of all registrar-TLD offerings

## Source

`DR_Admin/DTOs/RegistrarTldDto.cs`

## TypeScript Interface

```ts
export interface BulkUpdateRegistrarTldStatusDto {
  registrarId: number | null;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `RegistrarId` | `int?` | `number | null` |
| `IsActive` | `bool` | `boolean` |

## Used By Endpoints

- [PUT BulkUpdateAllRegistrarTldStatus](../registrar-tlds/put-bulk-update-all-registrar-tld-status-api-v1-registrar-tlds-bulk-update-status.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

