# DnsTroubleshootTestResultDto

Represents a single DNS troubleshoot test result.

## Source

`DR_Admin/DTOs/DnsTroubleshootDto.cs`

## TypeScript Interface

```ts
export interface DnsTroubleshootTestResultDto {
  key: string;
  name: string;
  severity: string;
  passed: boolean;
  message: string;
  details: string | null;
  fixUrl: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Key` | `string` | `string` |
| `Name` | `string` | `string` |
| `Severity` | `string` | `string` |
| `Passed` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `Details` | `string?` | `string | null` |
| `FixUrl` | `string?` | `string | null` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

