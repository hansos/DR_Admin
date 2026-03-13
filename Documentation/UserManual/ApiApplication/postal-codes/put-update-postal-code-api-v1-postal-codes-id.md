# PUT UpdatePostalCode

Update an existing postal code

## Endpoint

```
PUT /api/v1/postal-codes/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdatePostalCodeDto](../dtos/update-postal-code-dto.md)` |

[Back to API Manual index](../index.md)



