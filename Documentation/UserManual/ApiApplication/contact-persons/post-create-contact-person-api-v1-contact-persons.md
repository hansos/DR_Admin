# POST CreateContactPerson

Creates a new contact person in the system

## Endpoint

```
POST /api/v1/contact-persons
```

## Authorization

Requires authentication. Policy: **Customer.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateContactPersonDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `ContactPersonDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
