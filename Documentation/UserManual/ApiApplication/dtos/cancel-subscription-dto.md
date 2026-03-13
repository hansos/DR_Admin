# CancelSubscriptionDto

Data transfer object for cancelling a subscription

## Source

`DR_Admin/DTOs/SubscriptionDto.cs`

## TypeScript Interface

```ts
export interface CancelSubscriptionDto {
  cancellationReason: string;
  cancelImmediately: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CancellationReason` | `string` | `string` |
| `CancelImmediately` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
