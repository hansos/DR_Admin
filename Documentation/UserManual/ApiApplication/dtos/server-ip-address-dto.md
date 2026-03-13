# ServerIpAddressDto

Data transfer object representing a server IP address

## Source

`DR_Admin/DTOs/ServerIpAddressDto.cs`

## TypeScript Interface

```ts
export interface ServerIpAddressDto {
  id: number;
  serverId: number;
  ipAddress: string;
  ipVersion: string;
  isPrimary: boolean;
  status: string;
  assignedTo: string | null;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `ServerId` | `int` | `number` |
| `IpAddress` | `string` | `string` |
| `IpVersion` | `string` | `string` |
| `IsPrimary` | `bool` | `boolean` |
| `Status` | `string` | `string` |
| `AssignedTo` | `string?` | `string | null` |
| `Notes` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
