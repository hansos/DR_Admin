# POST CreatePostalCode

Create a new postal code

## Endpoint

```
POST /api/v1/postal-codes
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreatePostalCodeDto](../dtos/create-postal-code-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [PostalCodeDto](../dtos/postal-code-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




