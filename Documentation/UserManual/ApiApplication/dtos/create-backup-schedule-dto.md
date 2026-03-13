# CreateBackupScheduleDto

Data transfer object for CreateBackupScheduleDto.

## Source

`DR_Admin/DTOs/BackupScheduleDto.cs`

## TypeScript Interface

```ts
export interface CreateBackupScheduleDto {
  databaseName: string;
  frequency: string;
  nextBackupDate: string | null;
  status: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `DatabaseName` | `string` | `string` |
| `Frequency` | `string` | `string` |
| `NextBackupDate` | `DateTime?` | `string | null` |
| `Status` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
