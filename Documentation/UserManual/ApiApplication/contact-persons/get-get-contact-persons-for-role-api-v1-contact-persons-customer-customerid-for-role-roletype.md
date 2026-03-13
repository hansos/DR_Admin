# GET GetContactPersonsForRole

Retrieves contact persons for a customer categorized by role preference and usage.     Returns a three-tiered list:     1. Preferred - Contact persons marked as default for the specified role     2. Frequently Used - Contact persons used 3+ times for the specified role     3. Available - All other contact persons

## Endpoint

```
GET /api/v1/contact-persons/customer/{customerId}/for-role/{roleType}
```

## Authorization

Requires authentication. Policy: **Customer.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |
| `roleType` | Route | `ContactRoleType` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [CategorizedContactPersonListResponse](../dtos/categorized-contact-person-list-response.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




