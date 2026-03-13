# UpdateServerIpAddressDto

Data transfer object for updating an existing server IP address

## Source

`DR_Admin/DTOs/ServerIpAddressDto.cs`

## TypeScript Interface

```ts
export interface UpdateServerIpAddressDto {
  ipAddress: string;
  ipVersion: string;
  isPrimary: boolean;
  status: string;
  assignedTo: string | null;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `IpAddress` | `string` | `string` |
| `IpVersion` | `string` | `string` |
| `IsPrimary` | `bool` | `boolean` |
| `Status` | `string` | `string` |
| `AssignedTo` | `string?` | `string | null` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
