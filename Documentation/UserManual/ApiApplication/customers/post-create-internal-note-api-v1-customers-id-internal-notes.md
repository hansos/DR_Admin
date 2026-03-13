# POST CreateInternalNote

Manages customer information including creation, retrieval, updates, and deletion

## Endpoint

```
POST /api/v1/customers/{id}/internal-notes
```

## Authorization

Requires authentication. Policy: **Customer.Write**.

## Parameters

| Name        | Source | Type                            |
| ----------- | ------ | ------------------------------- |
| `id`        | Route  | `int`                           |
| `createDto` | Body   | `CreateCustomerInternalNoteDto` |

## Responses

| Code | Description           | Body                      |
| ---- | --------------------- | ------------------------- |
| 201  | Created               | `CustomerInternalNoteDto` |
| 400  | Bad Request           | -                         |
| 401  | Unauthorized          | -                         |
| 403  | Forbidden             | -                         |
| 404  | Not Found             | -                         |
| 500  | Internal Server Error | -                         |

[Back to API Manual index](../index.md)
