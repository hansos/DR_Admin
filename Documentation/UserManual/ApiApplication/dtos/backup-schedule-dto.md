# BackupScheduleDto

Data transfer object for BackupScheduleDto.

## Source

`DR_Admin/DTOs/BackupScheduleDto.cs`

## TypeScript Interface

```ts
export interface BackupScheduleDto {
  id: number;
  databaseName: string;
  frequency: string;
  lastBackupDate: string | null;
  nextBackupDate: string | null;
  status: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `DatabaseName` | `string` | `string` |
| `Frequency` | `string` | `string` |
| `LastBackupDate` | `DateTime?` | `string | null` |
| `NextBackupDate` | `DateTime?` | `string | null` |
| `Status` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
