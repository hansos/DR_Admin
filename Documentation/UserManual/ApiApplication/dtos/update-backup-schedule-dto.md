# UpdateBackupScheduleDto

Data transfer object for UpdateBackupScheduleDto.

## Source

`DR_Admin/DTOs/BackupScheduleDto.cs`

## TypeScript Interface

```ts
export interface UpdateBackupScheduleDto {
  databaseName: string;
  frequency: string;
  lastBackupDate: string | null;
  nextBackupDate: string | null;
  status: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `DatabaseName` | `string` | `string` |
| `Frequency` | `string` | `string` |
| `LastBackupDate` | `DateTime?` | `string | null` |
| `NextBackupDate` | `DateTime?` | `string | null` |
| `Status` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
