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

## Used By Endpoints

- [POST ApplyCustomerCredit](../payments/post-apply-customer-credit-api-v1-payments-apply-credit.md)
- [POST ConfirmAuthentication](../payments/post-confirm-authentication-api-v1-payments-confirm-authentication-paymentattemptid.md)
- [POST ProcessInvoicePayment](../payments/post-process-invoice-payment-api-v1-payments-process.md)
- [POST ProcessPartialPayment](../payments/post-process-partial-payment-api-v1-payments-partial.md)
- [POST RetryFailedPayment](../payments/post-retry-failed-payment-api-v1-payments-retry-paymentattemptid.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

