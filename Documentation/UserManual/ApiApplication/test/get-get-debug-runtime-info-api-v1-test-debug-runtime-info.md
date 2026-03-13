# GET GetDebugRuntimeInfo

Returns debug-only runtime details used by the reseller debug help page.

## Endpoint

```
GET /api/v1/test/debug-runtime-info
```

## Authorization

Requires authentication. Policy: **Policy = "Admin.Only"**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [TestDebugRuntimeInfoDto](../dtos/test-debug-runtime-info-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)




