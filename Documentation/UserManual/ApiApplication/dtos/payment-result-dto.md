# PaymentResultDto

Result from payment processing

## Source

`DR_Admin/DTOs/PaymentProcessingDtos.cs`

## TypeScript Interface

```ts
export interface PaymentResultDto {
  isSuccess: boolean;
  requiresAuthentication: boolean;
  authenticationUrl: string;
  paymentAttemptId: number | null;
  paymentTransactionId: number | null;
  errorCode: string;
  errorMessage: string;
  transactionId: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `IsSuccess` | `bool` | `boolean` |
| `RequiresAuthentication` | `bool` | `boolean` |
| `AuthenticationUrl` | `string` | `string` |
| `PaymentAttemptId` | `int?` | `number | null` |
| `PaymentTransactionId` | `int?` | `number | null` |
| `ErrorCode` | `string` | `string` |
| `ErrorMessage` | `string` | `string` |
| `TransactionId` | `string` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
