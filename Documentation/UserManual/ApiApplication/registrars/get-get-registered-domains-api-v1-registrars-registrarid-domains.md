# GET GetRegisteredDomains

Gets all domains registered with a specific registrar

## Endpoint

```
GET /api/v1/registrars/{registrarId}/domains
```

## Authorization

Requires authentication. Policy: **Registrar.Read**.

## Parameters

| Name          | Source | Type  |
| ------------- | ------ | ----- |
| `registrarId` | Route  | `int` |

## Responses

| Code | Description           | Body                      |
| ---- | --------------------- | ------------------------- |
| 200  | OK                    | `RegisteredDomainsResult` |
| 400  | Bad Request           | -                         |
| 401  | Unauthorized          | -                         |
| 500  | Internal Server Error | -                         |

[Back to API Manual index](../index.md)
