# Email Notification Endpoints - Verification List

## Summary
This document lists all API endpoints and event handlers that should send email notifications. Use this list to verify which ones need to be migrated to the template system.

---

## ✅ Already Using Templates (Migrated)

### 1. Domain Workflow Events
- **DomainRegisteredEventHandler** ✅
  - Event: Domain successfully registered
  - Email: Welcome email with domain details
  - Template: `Templates/DomainRegistered/`
  - Status: **Using MessagingTemplateLib**

- **DomainExpiredEventHandler** ✅
  - Event: Domain has expired
  - Email: Urgent expiration notification
  - Template: `Templates/DomainExpired/`
  - Status: **Using MessagingTemplateLib**

---

## ⚠️ Using Hardcoded Email Bodies (Need Migration)

### 2. Account Management (`MyAccountController.cs` + `MyAccountService.cs`)

#### **POST /api/v1/myaccount/register**
- **Purpose**: Register new account
- **Email Type**: Email confirmation/verification
- **Current Implementation**: Hardcoded in `MyAccountService.QueueEmailConfirmationAsync()`
- **Location**: `MyAccountService.cs` lines 423-449
- **Hardcoded Body**:
```html
<html>
<body>
    <h2>Confirm Your Email Address</h2>
    <p>Thank you for registering! Please confirm your email address by clicking the link below:</p>
    <p><a href="{confirmationUrl}">Confirm Email</a></p>
    <p>This link will expire in 3 days.</p>
    <p>If you did not request this, please ignore this email.</p>
</body>
</html>
```
- **Placeholders Needed**: `ConfirmationUrl`, `UserName`, `ExpirationDays`
- **Priority**: HIGH

#### **POST /api/v1/myaccount/request-password-reset**
- **Purpose**: Request password reset
- **Email Type**: Password reset link
- **Current Implementation**: Hardcoded in `MyAccountService.QueuePasswordResetEmailAsync()`
- **Location**: `MyAccountService.cs` lines 451-477
- **Hardcoded Body**:
```html
<html>
<body>
    <h2>Password Reset Request</h2>
    <p>We received a request to reset your password. Click the link below to reset it:</p>
    <p><a href="{resetUrl}">Reset Password</a></p>
    <p>This link will expire in 24 hours.</p>
    <p>If you did not request this, please ignore this email and your password will remain unchanged.</p>
</body>
</html>
```
- **Placeholders Needed**: `ResetUrl`, `UserName`, `ExpirationHours`
- **Priority**: HIGH

#### **PATCH /api/v1/myaccount/email**
- **Purpose**: Update email address (requires re-confirmation)
- **Email Type**: Email re-confirmation for new address
- **Current Implementation**: Reuses `QueueEmailConfirmationAsync()` (hardcoded)
- **Location**: `MyAccountService.cs` line 263
- **Priority**: HIGH (same as registration)

---

### 3. Order Workflow Events

#### **OrderActivatedEventHandler**
- **Event**: Order successfully activated
- **Email Type**: Service activation confirmation
- **Current Implementation**: Hardcoded in `BuildActivationEmailBody()`
- **Location**: `OrderActivatedEventHandler.cs` lines 60-79
- **Hardcoded Body**:
```
Hello,

Your service order {OrderNumber} has been successfully activated!

Activation Details:
- Order Number: {OrderNumber}
- Activated Date: {ActivatedAt:yyyy-MM-dd HH:mm:ss} UTC

You can now start using your service. Please visit your customer portal for more details.

Thank you for your business!

Best regards,
Your Service Team
```
- **Placeholders Needed**: `OrderNumber`, `ActivatedAt`, `ServiceName`, `CustomerPortalUrl`
- **Priority**: HIGH

---

### 4. Payment & Invoice Notifications (`PaymentNotificationService.cs`)

#### **SendInvoiceCreatedNotificationAsync()**
- **Purpose**: Notify customer of new invoice
- **Email Type**: Invoice created notification
- **Current Implementation**: TODO placeholder (not implemented)
- **Location**: `PaymentNotificationService.cs` lines 24-43
- **Status**: ⚠️ **NOT IMPLEMENTED** - needs both template creation and logic
- **Placeholders Needed**: `InvoiceNumber`, `InvoiceDate`, `DueDate`, `TotalAmount`, `InvoiceUrl`, `CustomerName`
- **Priority**: HIGH

#### **SendInvoicePaymentDueReminderAsync()**
- **Purpose**: Remind customer of upcoming payment due date
- **Email Type**: Payment due reminder
- **Current Implementation**: TODO placeholder (not implemented)
- **Location**: `PaymentNotificationService.cs` lines 45-56
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `InvoiceNumber`, `DueDate`, `TotalAmount`, `PaymentUrl`, `DaysUntilDue`
- **Priority**: MEDIUM

#### **SendInvoiceOverdueNotificationAsync()**
- **Purpose**: Notify customer of overdue invoice
- **Email Type**: Overdue payment notification
- **Current Implementation**: TODO placeholder (not implemented)
- **Location**: `PaymentNotificationService.cs` lines 58-69
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `InvoiceNumber`, `DueDate`, `TotalAmount`, `DaysOverdue`, `PaymentUrl`, `LateFee`
- **Priority**: HIGH

#### **SendPaymentReceivedConfirmationAsync()**
- **Purpose**: Confirm payment received
- **Email Type**: Payment confirmation/receipt
- **Current Implementation**: TODO placeholder (not implemented)
- **Location**: `PaymentNotificationService.cs` lines 71-88
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `InvoiceNumber`, `PaymentDate`, `AmountPaid`, `PaymentMethod`, `ReceiptUrl`
- **Priority**: HIGH

#### **SendPaymentFailedNotificationAsync()**
- **Purpose**: Notify customer of failed payment
- **Email Type**: Payment failure notification
- **Current Implementation**: TODO placeholder (not implemented)
- **Location**: `PaymentNotificationService.cs` lines 90-100+
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `InvoiceNumber`, `FailureReason`, `RetryUrl`, `SupportUrl`
- **Priority**: MEDIUM

---

### 5. Subscription Notifications

#### **Subscription Expiration Warning**
- **Purpose**: Warn customer of upcoming subscription expiration
- **Email Type**: Subscription expiration warning
- **Current Implementation**: Not found (may need to be created)
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `SubscriptionName`, `ExpirationDate`, `RenewUrl`, `DaysUntilExpiration`
- **Priority**: MEDIUM

#### **Subscription Canceled**
- **Purpose**: Confirm subscription cancellation
- **Email Type**: Cancellation confirmation
- **Current Implementation**: Not found (may need to be created)
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `SubscriptionName`, `CancellationDate`, `EndOfServiceDate`
- **Priority**: MEDIUM

---

### 6. Hosting Account Events

#### **Hosting Account Created**
- **Purpose**: Notify customer of new hosting account
- **Email Type**: Hosting account welcome email
- **Current Implementation**: Not found in code (may be in TODO)
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `HostingAccountName`, `ControlPanelUrl`, `Username`, `ServerName`, `SetupInstructions`
- **Priority**: MEDIUM

#### **Hosting Account Suspended**
- **Purpose**: Notify customer of account suspension
- **Email Type**: Suspension notification
- **Current Implementation**: Not found (may need to be created)
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `HostingAccountName`, `SuspensionReason`, `ReactivationUrl`, `SupportUrl`
- **Priority**: MEDIUM

---

### 7. Customer Service Notifications

#### **Welcome Email (First Customer)**
- **Purpose**: Welcome new customer to the platform
- **Email Type**: Welcome/onboarding email
- **Current Implementation**: Not found
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `CustomerName`, `CustomerPortalUrl`, `SupportUrl`, `GettingStartedGuide`
- **Priority**: LOW

#### **Customer Support Ticket Response**
- **Purpose**: Notify customer of support response
- **Email Type**: Support ticket update
- **Current Implementation**: Not found
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `TicketNumber`, `ResponseText`, `TicketUrl`, `AgentName`
- **Priority**: LOW (if support system exists)

---

### 8. Quote Notifications

#### **Quote Created**
- **Purpose**: Send quote to customer
- **Email Type**: Quote/estimate email
- **Current Implementation**: Not found
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `QuoteNumber`, `QuoteDate`, `ExpirationDate`, `TotalAmount`, `QuoteUrl`, `Items`
- **Priority**: MEDIUM

#### **Quote Accepted**
- **Purpose**: Confirm quote acceptance
- **Email Type**: Quote acceptance confirmation
- **Current Implementation**: Not found
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `QuoteNumber`, `OrderNumber`, `NextSteps`
- **Priority**: MEDIUM

---

### 9. Refund Notifications

#### **Refund Issued**
- **Purpose**: Confirm refund processed
- **Email Type**: Refund confirmation
- **Current Implementation**: Not found
- **Status**: ⚠️ **NOT IMPLEMENTED**
- **Placeholders Needed**: `RefundAmount`, `RefundDate`, `OriginalInvoiceNumber`, `RefundMethod`, `ProcessingTime`
- **Priority**: MEDIUM

---

## 📊 Summary Statistics

| Status | Count |
|--------|-------|
| ✅ Using Templates | 2 |
| ⚠️ Hardcoded (Need Migration) | 3 |
| ⚠️ Not Implemented (Need Creation) | 14 |
| **Total Email Types** | **19** |

---

## 🎯 Migration Priority

### High Priority (Implement First)
1. ✅ **Email Confirmation** (Registration) - Hardcoded ⚠️
2. ✅ **Password Reset** - Hardcoded ⚠️
3. ✅ **Order Activated** - Hardcoded ⚠️
4. ⚠️ **Invoice Created** - Not implemented
5. ⚠️ **Payment Received Confirmation** - Not implemented
6. ⚠️ **Invoice Overdue** - Not implemented

### Medium Priority
7. ⚠️ **Payment Due Reminder** - Not implemented
8. ⚠️ **Payment Failed** - Not implemented
9. ⚠️ **Quote Created** - Not implemented
10. ⚠️ **Quote Accepted** - Not implemented
11. ⚠️ **Refund Issued** - Not implemented
12. ⚠️ **Subscription Expiration** - Not implemented
13. ⚠️ **Hosting Account Created** - Not implemented
14. ⚠️ **Hosting Account Suspended** - Not implemented

### Low Priority
15. ⚠️ **Welcome Email** - Not implemented
16. ⚠️ **Subscription Canceled** - Not implemented
17. ⚠️ **Support Ticket Response** - Not implemented

---

## 🔍 Files to Modify

### Immediate Action Required:
1. `DR_Admin/Services/MyAccountService.cs`
   - Migrate `QueueEmailConfirmationAsync()` (lines 423-449)
   - Migrate `QueuePasswordResetEmailAsync()` (lines 451-477)

2. `DR_Admin/Workflow/Domain/EventHandlers/OrderActivatedEventHandler.cs`
   - Migrate `BuildActivationEmailBody()` (lines 60-79)

3. `DR_Admin/Services/PaymentNotificationService.cs`
   - Implement all TODO methods with templates

### Create New Templates For:
1. `Templates/EmailConfirmation/` (email.html.txt, email.text.txt, sms.txt)
2. `Templates/PasswordReset/` (email.html.txt, email.text.txt, sms.txt)
3. `Templates/OrderActivated/` (email.html.txt, email.text.txt, sms.txt)
4. `Templates/InvoiceCreated/` (email.html.txt, email.text.txt, sms.txt)
5. `Templates/PaymentReceived/` (email.html.txt, email.text.txt, sms.txt)
6. `Templates/InvoiceOverdue/` (email.html.txt, email.text.txt, sms.txt)
7. ... (additional templates as needed)

---

## ✅ Verification Checklist

Use this checklist to track your migration progress:

- [x] ✅ DomainRegistered - Migrated
- [x] ✅ DomainExpired - Migrated
- [x] ✅ EmailConfirmation - **MIGRATED** (2026-01-15)
- [x] ✅ PasswordReset - **MIGRATED** (2026-01-15)
- [x] ✅ OrderActivated - **MIGRATED** (2026-01-15)
- [ ] ⚠️ InvoiceCreated - Needs implementation
- [ ] ⚠️ PaymentReceived - Needs implementation
- [ ] ⚠️ InvoiceOverdue - Needs implementation
- [ ] ⚠️ PaymentDueReminder - Needs implementation
- [ ] ⚠️ PaymentFailed - Needs implementation
- [ ] ⚠️ QuoteCreated - Needs implementation
- [ ] ⚠️ QuoteAccepted - Needs implementation
- [ ] ⚠️ RefundIssued - Needs implementation
- [ ] ⚠️ SubscriptionExpiring - Needs implementation
- [ ] ⚠️ SubscriptionCanceled - Needs implementation
- [ ] ⚠️ HostingAccountCreated - Needs implementation
- [ ] ⚠️ HostingAccountSuspended - Needs implementation
- [ ] ⚠️ WelcomeEmail - Needs implementation
- [ ] ⚠️ SupportTicketResponse - Needs implementation (if needed)

---

## 📝 Notes

1. **Email vs SMS**: Most templates should have both email (HTML + text) and SMS versions
2. **Localization**: Consider creating language-specific template folders in the future (e.g., `Templates/en/`, `Templates/no/`)
3. **Testing**: Create test endpoints or unit tests for each email type
4. **Configuration**: Move hardcoded URLs (BaseUrl, CustomerPortalUrl) to appsettings.json
5. **Logging**: All email sending should be logged with customer ID, email type, and timestamp

---

## 🚀 Next Steps

1. **Review this list** and verify all email types are identified
2. **Prioritize** which emails to migrate first (start with High Priority)
3. **Create templates** for high-priority emails
4. **Create model classes** in `MessagingTemplateLib/Models/`
5. **Update services** to use `MessagingService` instead of hardcoded strings
6. **Test** each email type thoroughly
7. **Deploy** incrementally, starting with high-priority emails

---

**Last Updated**: 2026-01-15
**Status**: Ready for review and implementation
