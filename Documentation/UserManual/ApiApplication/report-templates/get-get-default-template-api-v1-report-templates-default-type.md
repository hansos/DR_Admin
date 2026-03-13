# GET GetDefaultTemplate

Retrieves the default template for a specific type

## Endpoint

```
GET /api/v1/report-templates/default/{type}
```

## Authorization

Requires authentication. Policy: **ReportTemplate.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `type` | Route | `ReportTemplateType` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ReportTemplateDto](../dtos/report-template-dto.md) |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




