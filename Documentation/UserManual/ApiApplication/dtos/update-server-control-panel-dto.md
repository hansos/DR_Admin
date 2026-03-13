# UpdateServerControlPanelDto

Data transfer object for updating an existing server control panel

## Source

`DR_Admin/DTOs/ServerControlPanelDto.cs`

## TypeScript Interface

```ts
export interface UpdateServerControlPanelDto {
  apiUrl: string;
  port: number;
  useHttps: boolean;
  apiToken: string | null;
  apiKey: string | null;
  username: string | null;
  password: string | null;
  additionalSettings: string | null;
  status: string;
  ipAddressId: number | null;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ApiUrl` | `string` | `string` |
| `Port` | `int` | `number` |
| `UseHttps` | `bool` | `boolean` |
| `ApiToken` | `string?` | `string | null` |
| `ApiKey` | `string?` | `string | null` |
| `Username` | `string?` | `string | null` |
| `Password` | `string?` | `string | null` |
| `AdditionalSettings` | `string?` | `string | null` |
| `Status` | `string` | `string` |
| `IpAddressId` | `int?` | `number | null` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
