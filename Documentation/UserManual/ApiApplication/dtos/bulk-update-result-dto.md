# BulkUpdateResultDto

Data transfer object representing the result of a bulk update operation

## Source

`DR_Admin/DTOs/RegistrarTldDto.cs`

## TypeScript Interface

```ts
export interface BulkUpdateResultDto {
  updatedCount: number;
  message: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `UpdatedCount` | `int` | `number` |
| `Message` | `string` | `string` |

## Used By Endpoints

- [PUT BulkUpdateAllRegistrarTldStatus](../registrar-tlds/put-bulk-update-all-registrar-tld-status-api-v1-registrar-tlds-bulk-update-status.md)
- [PUT BulkUpdateRegistrarTldStatusByTld](../registrar-tlds/put-bulk-update-registrar-tld-status-by-tld-api-v1-registrar-tlds-bulk-update-status-by-tld.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

