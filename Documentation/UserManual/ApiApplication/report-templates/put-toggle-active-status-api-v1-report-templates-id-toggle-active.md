# PUT ToggleActiveStatus

Toggles the active status of a report template

## Endpoint

```
PUT /api/v1/report-templates/{id}/toggle-active
```

## Authorization

Requires authentication. Policy: **ReportTemplate.Update**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
