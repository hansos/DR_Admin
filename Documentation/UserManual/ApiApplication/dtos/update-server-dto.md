# UpdateServerDto

Data transfer object for updating an existing server

## Source

`DR_Admin/DTOs/ServerDto.cs`

## TypeScript Interface

```ts
export interface UpdateServerDto {
  name: string;
  serverTypeId: number;
  hostProviderId: number | null;
  location: string | null;
  operatingSystemId: number;
  status: boolean | null;
  cpuCores: number | null;
  ramMB: number | null;
  diskSpaceGB: number | null;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `ServerTypeId` | `int` | `number` |
| `HostProviderId` | `int?` | `number | null` |
| `Location` | `string?` | `string | null` |
| `OperatingSystemId` | `int` | `number` |
| `Status` | `bool?` | `boolean | null` |
| `CpuCores` | `int?` | `number | null` |
| `RamMB` | `int?` | `number | null` |
| `DiskSpaceGB` | `int?` | `number | null` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
