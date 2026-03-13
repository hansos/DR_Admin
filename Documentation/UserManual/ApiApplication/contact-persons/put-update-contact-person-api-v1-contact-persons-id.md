# PUT UpdateContactPerson

Updates an existing contact person's information

## Endpoint

```
PUT /api/v1/contact-persons/{id}
```

## Authorization

Requires authentication. Policy: **Customer.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdateContactPersonDto](../dtos/update-contact-person-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[ContactPersonDto](../dtos/contact-person-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



