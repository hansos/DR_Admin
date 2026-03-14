# UpdateSalesAgentDto

Data transfer object for updating an existing sales agent

## Source

`DR_Admin/DTOs/SalesAgentDto.cs`

## TypeScript Interface

```ts
export interface UpdateSalesAgentDto {
  resellerCompanyId: number | null;
  firstName: string;
  lastName: string;
  email: string | null;
  phone: string | null;
  mobilePhone: string | null;
  isActive: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ResellerCompanyId` | `int?` | `number | null` |
| `FirstName` | `string` | `string` |
| `LastName` | `string` | `string` |
| `Email` | `string?` | `string | null` |
| `Phone` | `string?` | `string | null` |
| `MobilePhone` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

## Used By Endpoints

- [PUT UpdateSalesAgent](../sales-agents/put-update-sales-agent-api-v1-sales-agents-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

