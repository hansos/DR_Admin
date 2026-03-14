# CreateServiceDto

Data transfer object for creating a new service

## Source

`DR_Admin/DTOs/ServiceDto.cs`

## TypeScript Interface

```ts
export interface CreateServiceDto {
  name: string;
  description: string;
  serviceTypeId: number;
  billingCycleId: number | null;
  price: number | null;
  resellerCompanyId: number | null;
  salesAgentId: number | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `ServiceTypeId` | `int` | `number` |
| `BillingCycleId` | `int?` | `number | null` |
| `Price` | `decimal?` | `number | null` |
| `ResellerCompanyId` | `int?` | `number | null` |
| `SalesAgentId` | `int?` | `number | null` |

## Used By Endpoints

- [POST CreateService](../services/post-create-service-api-v1-services.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

