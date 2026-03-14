# TestDebugRuntimeInfoDto

Provides debug-only runtime details used by the reseller debug help page.

## Source

`DR_Admin/DTOs/TestDebugRuntimeInfoDto.cs`

## TypeScript Interface

```ts
export interface TestDebugRuntimeInfoDto {
  databaseConnectionDescription: string;
  simulatorRegistrarDatabasePath: string;
  userJsonImportFilePath: string;
  adminJsonImportFilePath: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `DatabaseConnectionDescription` | `string` | `string` |
| `SimulatorRegistrarDatabasePath` | `string` | `string` |
| `UserJsonImportFilePath` | `string` | `string` |
| `AdminJsonImportFilePath` | `string` | `string` |

## Used By Endpoints

- [GET GetDebugRuntimeInfo](../test/get-get-debug-runtime-info-api-v1-test-debug-runtime-info.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

