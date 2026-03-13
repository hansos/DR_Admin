# POST InitializeCustomer

Initializes the user panel with the first customer user, company and primary contact person.

## Endpoint

```
POST /api/v1/initialization/initialize-customer
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `UserPanelInitializationRequestDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `UserPanelInitializationResponseDto` |
| 400 | Bad Request | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
