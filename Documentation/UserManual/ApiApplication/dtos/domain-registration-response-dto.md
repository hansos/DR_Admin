# DomainRegistrationResponseDto

Response DTO for domain registration request

## Source

`DR_Admin/DTOs/DomainRegistrationDto.cs`

## TypeScript Interface

```ts
export interface DomainRegistrationResponseDto {
  success: boolean;
  message: string;
  orderId: number | null;
  orderNumber: string | null;
  invoiceId: number | null;
  totalAmount: number | null;
  correlationId: string | null;
  requiresApproval: boolean;
  approvalStatus: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `OrderId` | `int?` | `number | null` |
| `OrderNumber` | `string?` | `string | null` |
| `InvoiceId` | `int?` | `number | null` |
| `TotalAmount` | `decimal?` | `number | null` |
| `CorrelationId` | `string?` | `string | null` |
| `RequiresApproval` | `bool` | `boolean` |
| `ApprovalStatus` | `string?` | `string | null` |

## Used By Endpoints

- [POST RegisterDomain](../registered-domains/post-register-domain-api-v1-registered-domains-register.md)
- [POST RegisterDomainForCustomer](../registered-domains/post-register-domain-for-customer-api-v1-registered-domains-register-for-customer.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

