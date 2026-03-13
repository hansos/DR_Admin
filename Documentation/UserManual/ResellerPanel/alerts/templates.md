# Templates

Create and manage notification and email templates used by alerts and automation.

## How to Access

Navigate to **Alerts & Automation > Templates** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Template Name | Descriptive name. |
| Type | Email, SMS, or In-App. |
| Subject | Email subject line (for email templates). |
| Last Modified | Date the template was last updated. |
| Actions | Edit, Duplicate, Preview, Delete. |

## Creating a Template

1. Click **Add Template**.
2. Enter a template name and select the type.
3. Compose the content using the rich text editor.
4. Use **placeholders** for dynamic data:

| Placeholder | Description |
|-------------|-------------|
| `{CustomerName}` | Customer's full name. |
| `{DomainName}` | Domain name. |
| `{ExpiryDate}` | Domain expiry date. |
| `{InvoiceNumber}` | Invoice identifier. |
| `{AmountDue}` | Outstanding amount. |

5. Click **Save**.

## Previewing

Click **Preview** to see how the template renders with sample data.

## Related Pages

- [Automated Actions](automated-actions.md)
- [Communication](../customers/communication.md)

[Back to Reseller Manual index](../index.md)
