# ServerDto

Data transfer object representing a server

## Source

`DR_Admin/DTOs/ServerDto.cs`

## TypeScript Interface

```ts
export interface ServerDto {
  id: number;
  name: string;
  serverTypeId: number;
  serverTypeName: string;
  hostProviderId: number | null;
  hostProviderName: string | null;
  location: string | null;
  operatingSystemId: number;
  operatingSystemName: string;
  status: boolean | null;
  cpuCores: number | null;
  ramMB: number | null;
  diskSpaceGB: number | null;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `ServerTypeId` | `int` | `number` |
| `ServerTypeName` | `string` | `string` |
| `HostProviderId` | `int?` | `number | null` |
| `HostProviderName` | `string?` | `string | null` |
| `Location` | `string?` | `string | null` |
| `OperatingSystemId` | `int` | `number` |
| `OperatingSystemName` | `string` | `string` |
| `Status` | `bool?` | `boolean | null` |
| `CpuCores` | `int?` | `number | null` |
| `RamMB` | `int?` | `number | null` |
| `DiskSpaceGB` | `int?` | `number | null` |
| `Notes` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetServerById](../servers/get-get-server-by-id-api-v1-servers-id.md)
- [POST CreateServer](../servers/post-create-server-api-v1-servers.md)
- [PUT UpdateServer](../servers/put-update-server-api-v1-servers-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

