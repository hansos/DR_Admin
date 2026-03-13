# POST InitializeAdmin

Initializes the system with the first admin user (only works if no users exist)

## Endpoint

```
POST /api/v1/initialization/initialize-admin
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | [InitializationRequestDto](../dtos/initialization-request-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [InitializationResponseDto](../dtos/initialization-response-dto.md) |
| 400 | Bad Request | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




