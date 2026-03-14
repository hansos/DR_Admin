# ServiceDto

Data transfer object representing a service offered to customers

## Source

`DR_Admin/DTOs/ServiceDto.cs`

## TypeScript Interface

```ts
export interface ServiceDto {
  id: number;
  name: string;
  description: string;
  serviceTypeId: number;
  billingCycleId: number | null;
  price: number | null;
  resellerCompanyId: number | null;
  salesAgentId: number | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `Description` | `string` | `string` |
| `ServiceTypeId` | `int` | `number` |
| `BillingCycleId` | `int?` | `number | null` |
| `Price` | `decimal?` | `number | null` |
| `ResellerCompanyId` | `int?` | `number | null` |
| `SalesAgentId` | `int?` | `number | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetServiceById](../services/get-get-service-by-id-api-v1-services-id.md)
- [POST CreateService](../services/post-create-service-api-v1-services.md)
- [PUT UpdateService](../services/put-update-service-api-v1-services-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

