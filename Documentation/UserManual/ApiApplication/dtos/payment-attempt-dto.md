# PaymentAttemptDto

Data transfer object representing a payment attempt

## Source

`DR_Admin/DTOs/PaymentProcessingDtos.cs`

## TypeScript Interface

```ts
export interface PaymentAttemptDto {
  id: number;
  invoiceId: number;
  paymentTransactionId: number | null;
  customerPaymentMethodId: number;
  attemptedAmount: number;
  currency: string;
  status: PaymentAttemptStatus;
  gatewayResponse: string;
  gatewayTransactionId: string;
  errorCode: string;
  errorMessage: string;
  retryCount: number;
  nextRetryAt: string | null;
  requiresAuthentication: boolean;
  authenticationUrl: string;
  authenticationStatus: AuthenticationStatus;
  ipAddress: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `InvoiceId` | `int` | `number` |
| `PaymentTransactionId` | `int?` | `number | null` |
| `CustomerPaymentMethodId` | `int` | `number` |
| `AttemptedAmount` | `decimal` | `number` |
| `Currency` | `string` | `string` |
| `Status` | `PaymentAttemptStatus` | `PaymentAttemptStatus` |
| `GatewayResponse` | `string` | `string` |
| `GatewayTransactionId` | `string` | `string` |
| `ErrorCode` | `string` | `string` |
| `ErrorMessage` | `string` | `string` |
| `RetryCount` | `int` | `number` |
| `NextRetryAt` | `DateTime?` | `string | null` |
| `RequiresAuthentication` | `bool` | `boolean` |
| `AuthenticationUrl` | `string` | `string` |
| `AuthenticationStatus` | `AuthenticationStatus` | `AuthenticationStatus` |
| `IpAddress` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
