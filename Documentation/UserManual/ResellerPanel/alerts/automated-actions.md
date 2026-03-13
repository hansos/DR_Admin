# Automated Actions

Set up automated workflows triggered by events such as auto-renew, expiration reminders, and follow-ups.

## How to Access

Navigate to **Alerts & Automation > Automated Actions** from the side menu.

## Page Layout

| Column | Description |
|--------|-------------|
| Action Name | Descriptive name for the automation. |
| Trigger | Event that starts the action (e.g., Domain Expiring in 30 days). |
| Action | What happens (e.g., Send Email, Auto-Renew, Create Invoice). |
| Status | Active or Disabled. |
| Last Run | Date the action was last executed. |
| Actions | Edit, Enable/Disable, Delete, View Log. |

## Creating an Automated Action

1. Click **Add Automation**.
2. **Select Trigger** â€” Choose the event (e.g., "Domain expiring in X days", "Invoice overdue by X days").
3. **Define Conditions** â€” Optionally filter (e.g., only for specific customers or TLDs).
4. **Choose Action** â€” Select what should happen (send email, auto-renew, etc.).
5. **Select Template** â€” If sending a notification, choose a [Template](templates.md).
6. Click **Save**.

## Example Automations

| Trigger | Action |
|---------|--------|
| Domain expiring in 30 days | Send renewal reminder email to customer. |
| Invoice overdue by 7 days | Send payment reminder email. |
| Domain expiring in 1 day (auto-renew on) | Automatically renew the domain. |
| Transfer completed | Send confirmation email to customer. |

## Related Pages

- [Notifications](notifications.md)
- [Templates](templates.md)

[Back to Reseller Manual index](../index.md)
