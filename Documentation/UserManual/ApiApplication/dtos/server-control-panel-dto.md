# ServerControlPanelDto

Data transfer object representing a server control panel installation

## Source

`DR_Admin/DTOs/ServerControlPanelDto.cs`

## TypeScript Interface

```ts
export interface ServerControlPanelDto {
  id: number;
  serverId: number;
  controlPanelTypeId: number;
  apiUrl: string;
  port: number;
  useHttps: boolean;
  username: string | null;
  status: string;
  lastConnectionTest: string | null;
  isConnectionHealthy: boolean | null;
  lastError: string | null;
  notes: string | null;
  ipAddressId: number | null;
  ipAddressValue: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `ServerId` | `int` | `number` |
| `ControlPanelTypeId` | `int` | `number` |
| `ApiUrl` | `string` | `string` |
| `Port` | `int` | `number` |
| `UseHttps` | `bool` | `boolean` |
| `Username` | `string?` | `string | null` |
| `Status` | `string` | `string` |
| `LastConnectionTest` | `DateTime?` | `string | null` |
| `IsConnectionHealthy` | `bool?` | `boolean | null` |
| `LastError` | `string?` | `string | null` |
| `Notes` | `string?` | `string | null` |
| `IpAddressId` | `int?` | `number | null` |
| `IpAddressValue` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
