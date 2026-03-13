# GET GetTemplateById

Manages report templates for FastReport and other reporting engines

## Endpoint

```
GET /api/v1/report-templates/{id}
```

## Authorization

Requires authentication. Policy: **ReportTemplate.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[ReportTemplateDto](../dtos/report-template-dto.md)` |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



