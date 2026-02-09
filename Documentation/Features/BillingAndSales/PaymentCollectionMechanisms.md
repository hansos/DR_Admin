# Customer Payment Collection and Receipt Mechanisms

## Overview

This document describes the complete mechanisms for collecting and receiving payments from customers in the DR_Admin ISP Administration System. The system supports multiple payment methods, automatic collection, partial payments, and full traceability of all payment attempts.

---

## 1. Payment Flow - Main Process

### 1.1 High-Level Flow

```
Invoice Created -> Sent to Customer -> Customer Pays -> Payment Received -> Invoice Marked Paid
```

### 1.2 Detailed Flow

```
+------------------------------------------------------------------+
| 1. INVOICE GENERATION                                            |
+------------------------------------------------------------------+
| * System creates invoice from order/subscription                 |
| * Calculates total amount incl. VAT and fees                     |
| * Invoice status set to "Issued"                                 |
| * Due date calculated based on payment terms                     |
+------------------------------------------------------------------+
                            |
                            v
+------------------------------------------------------------------+
| 2. CUSTOMER NOTIFICATION                                         |
+------------------------------------------------------------------+
| * Email with invoice sent to customer                            |
| * PDF attachment generated (if configured)                       |
| * Payment link included                                          |
| * Reminders scheduled for due date                               |
+------------------------------------------------------------------+
                            |
                            v
+------------------------------------------------------------------+
| 3. CUSTOMER SELECTS PAYMENT METHOD                               |
+------------------------------------------------------------------+
| A. Saved payment card (with token)                               |
| B. New payment card                                              |
| C. Customer credit (prepaid balance)                             |
| D. Bank transfer                                                 |
| E. Other payment gateways (PayPal, etc.)                         |
+------------------------------------------------------------------+
                            |
                            v
+------------------------------------------------------------------+
| 4. PAYMENT ATTEMPT CREATED                                       |
+------------------------------------------------------------------+
| * PaymentAttempt record created                                  |
| * Status: "Processing"                                           |
| * IP address and User Agent logged (security)                    |
| * Amount, currency, and payment method recorded                  |
+------------------------------------------------------------------+
                            |
                            v
+------------------------------------------------------------------+
| 5. PAYMENT GATEWAY PROCESSING                                    |
+------------------------------------------------------------------+
| * PaymentMethodToken retrieved                                   |
| * Gateway Adapter called (Stripe/PayPal/other)                   |
| * Transaction sent to payment gateway                            |
|                                                                  |
| POSSIBLE RESULTS:                                                |
| +-- Success -> Go to step 6A                                     |
| +-- Requires Authentication (3D Secure) -> Go to step 6B        |
| +-- Failed -> Go to step 6C                                      |
+------------------------------------------------------------------+
                            |
                            v
+------------------------------------------------------------------+
| 6A. PAYMENT SUCCESSFUL                                          |
+------------------------------------------------------------------+
| * PaymentTransaction created                                     |
| * Status: "Completed"                                            |
| * Gateway transaction ID stored                                  |
| * PaymentAttempt updated to "Succeeded"                          |
| * InvoicePayment record created                                  |
| * Invoice status -> "Paid"                                       |
| * PaidAt timestamp set                                           |
| * Receipt sent to customer                                       |
| * Order activated (if relevant)                                  |
+------------------------------------------------------------------+

+------------------------------------------------------------------+
| 6B. REQUIRES AUTHENTICATION (3D Secure / SCA)                    |
+------------------------------------------------------------------+
| * PaymentAttempt status -> "RequiresAuthentication"             |
| * AuthenticationUrl generated                                    |
| * Customer redirected to bank's authentication page             |
| * Customer completes authentication (BankID, SMS code, etc.)    |
| * Callback received from gateway                                 |
| * Payment confirmed and completed -> Go to 6A                    |
+------------------------------------------------------------------+

+------------------------------------------------------------------+
| 6C. PAYMENT FAILED                                               |
+------------------------------------------------------------------+
| * PaymentAttempt status -> "Failed"                              |
| * Error message and error code stored                            |
| * RetryCount incremented                                         |
| * NextRetryAt scheduled (if automatic retry)                     |
| * Customer notified of failure                                   |
| * Admin notified (if critical)                                   |
|                                                                  |
| POSSIBLE FAILURE REASONS:                                        |
| * Insufficient funds                                             |
| * Expired card                                                   |
| * Declined by issuer                                             |
| * Technical error                                                |
| * Suspected fraud                                                |
+------------------------------------------------------------------+
```

---

## 2. Payment Methods

### 2.1 Payment Cards (Stripe, PayPal, etc.)

**First-time use:**
```
1. Customer provides card information
2. Frontend sends data to gateway (directly, not via server)
3. Gateway returns token
4. Token stored in PaymentMethodToken table (encrypted)
5. Card details stored (last 4 digits, brand, expiry date)
6. Token used for future payments
```

**Recurring payments:**
```
1. System retrieves existing PaymentMethodToken
2. Gateway called with token (no card numbers sent)
3. Payment processed
```

**Implementation:**
- Entity: `PaymentMethodToken`
- Fields: `GatewayCustomerId`, `GatewayPaymentMethodId`, `EncryptedToken`
- Security: Token encrypted before storage
- Expiry: `ExpiresAt` date monitored

### 2.2 Customer Credit (Prepaid)

**How it works:**
```
1. Customer has prepaid balance (CustomerCredit)
2. On billing: System checks available credit
3. If sufficient credit: Automatic application
4. CreditTransaction created (Type: "Deduction")
5. CustomerCredit.Balance reduced
6. Invoice marked as paid
```

**Credit types:**
- **Deposit**: Customer deposits money
- **Deduction**: Credit used on invoice
- **Refund**: Refund added to credit
- **Adjustment**: Manual adjustment by admin
- **Promotional**: Promotional credit

**API Endpoint:**
```http
POST /api/v1/payments/apply-credit
{
  "invoiceId": 123,
  "amount": 50.00,
  "notes": "Applying account credit"
}
```

### 2.3 Bank Transfer

**Manual process:**
```
1. Invoice sent with bank account details
2. Customer performs bank transfer
3. Payment received on bank account
4. Admin verifies payment
5. PaymentTransaction created manually
6. Invoice marked as paid
```

**Future automation:**
- Bank API integration
- Automatic reconciliation
- OCR/KID number matching

### 2.4 Partial Payments

**Support for partial payment:**
```
Invoice total amount: 1000 EUR

Payment 1: 400 EUR -> InvoicePayment #1
Payment 2: 300 EUR -> InvoicePayment #2
Payment 3: 300 EUR -> InvoicePayment #3 -> Invoice fully paid
```

**Implementation:**
- Entity: `InvoicePayment` (links payment to invoice)
- Tracking: `InvoiceBalance` updated with each payment
- Status: `IsFullPayment` flag when final payment

**API Endpoint:**
```http
POST /api/v1/payments/partial
{
  "invoiceId": 123,
  "amount": 400.00,
  "customerPaymentMethodId": 5
}
```

---

## 3. Payment Attempts

### 3.1 Traceability

**Why PaymentAttempt?**
- Full audit trail of all payment attempts
- Debugging payment issues
- Analysis of failure patterns
- Fraud detection
- Reporting

**What is stored:**
```csharp
public class PaymentAttempt
{
    public int InvoiceId { get; set; }
    public int CustomerPaymentMethodId { get; set; }
    public decimal AttemptedAmount { get; set; }
    public PaymentAttemptStatus Status { get; set; }
    
    // Gateway response
    public string GatewayResponse { get; set; }
    public string GatewayTransactionId { get; set; }
    
    // Error handling
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTime? NextRetryAt { get; set; }
    
    // 3D Secure
    public bool RequiresAuthentication { get; set; }
    public string AuthenticationUrl { get; set; }
    public AuthenticationStatus AuthenticationStatus { get; set; }
    
    // Security
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
}
```

### 3.2 Status Flow

```
Pending -> Processing -> Succeeded
                    |
                    v
                RequiresAuthentication -> Succeeded
                    |
                    v
                Failed -> (Retry) -> Processing
                    |
                    v
                Cancelled
```

### 3.3 Retry Logic

**Automatic retry:**
```
Attempt 1: Immediate
Attempt 2: +1 hour
Attempt 3: +6 hours
Attempt 4: +24 hours
Attempt 5: Manual intervention
```

**Retry triggers:**
- Technical errors (gateway timeout)
- Temporarily unavailable card
- Network errors

**No retry:**
- Insufficient funds
- Expired card
- Suspected fraud
- Declined by cardholder

---

## 4. 3D Secure / SCA (Strong Customer Authentication)

### 4.1 What is 3D Secure?

**PSD2 requirements:**
- EU regulation requires strong customer authentication
- Two-factor authentication for online payments
- Reduces fraud

**Implementation in DR_Admin:**
```
1. Customer initiates payment
2. Gateway identifies need for authentication
3. PaymentAttempt status -> RequiresAuthentication
4. AuthenticationUrl generated
5. Customer redirected to bank
6. Authentication (BankID, SMS code, app)
7. Callback to our webhook
8. Payment confirmed and completed
```

### 4.2 Authentication Flow

```
POST /api/v1/payments/process
{
  "invoiceId": 123,
  "customerPaymentMethodId": 5,
  "returnUrl": "https://isp.com/payment-complete"
}

RESPONSE (requires authentication):
{
  "isSuccess": false,
  "requiresAuthentication": true,
  "authenticationUrl": "https://stripe.com/3ds/...",
  "paymentAttemptId": 78
}

Frontend redirects customer to authenticationUrl
Customer authenticates
Gateway redirects back to returnUrl
Frontend calls:

POST /api/v1/payments/confirm-authentication/78

RESPONSE (successful):
{
  "isSuccess": true,
  "paymentTransactionId": 456,
  "transactionId": "TXN-20240209120000"
}
```

---

## 5. Gateway Abstraction Layer

### 5.1 Why Gateway Abstraction?

**Problems without abstraction:**
- Vendor lock-in
- Difficult to switch gateways
- Duplicated code
- Difficult testing

**Solution:**
```csharp
public interface IPaymentGatewayAdapter
{
    string GatewayName { get; }
    
    Task<GatewayChargeResult> ChargeAsync(ChargeRequest request);
    Task<GatewayPaymentIntentResult> CreatePaymentIntentAsync(...);
    Task<GatewayChargeResult> ConfirmPaymentIntentAsync(string intentId);
    Task<GatewayPaymentMethodResult> SavePaymentMethodAsync(...);
    Task<GatewayRefundResult> RefundAsync(RefundRequest request);
    Task<bool> VerifyWebhookSignatureAsync(string payload, string signature);
    Task<GatewayWebhookEvent> ParseWebhookAsync(string payload);
}
```

### 5.2 Gateway Adapters

**Planned implementations:**

**StripePaymentGatewayAdapter:**
```csharp
public class StripePaymentGatewayAdapter : IPaymentGatewayAdapter
{
    private readonly StripeClient _stripeClient;
    
    public string GatewayName => "Stripe";
    
    public async Task<GatewayChargeResult> ChargeAsync(ChargeRequest request)
    {
        var options = new ChargeCreateOptions
        {
            Amount = (long)(request.Amount * 100), // Cents
            Currency = request.Currency.ToLower(),
            Source = request.PaymentMethodToken,
            Description = request.Description,
            Metadata = request.Metadata
        };
        
        var charge = await _stripeClient.Charges.CreateAsync(options);
        
        return new GatewayChargeResult
        {
            IsSuccess = charge.Status == "succeeded",
            TransactionId = charge.Id,
            Status = charge.Status,
            // ...
        };
    }
}
```

**PayPalPaymentGatewayAdapter:**
```csharp
public class PayPalPaymentGatewayAdapter : IPaymentGatewayAdapter
{
    private readonly PayPalHttpClient _paypalClient;
    
    public string GatewayName => "PayPal";
    
    // PayPal-specific implementation
}
```

### 5.3 Gateway Selection

**Automatic selection:**
```csharp
// Based on CustomerPaymentMethod.PaymentGatewayId
var gateway = await _context.PaymentGateways
    .FirstOrDefaultAsync(g => g.Id == paymentMethod.PaymentGatewayId);

var adapter = _gatewayFactory.GetAdapter(gateway.Name);
var result = await adapter.ChargeAsync(request);
```

---

## 6. Webhook Handling

### 6.1 What are Webhooks?

**Asynchronous communication:**
- Gateway sends events to our server
- Payment status updates
- 3D Secure completion
- Refunds
- Chargebacks

### 6.2 Webhook Endpoint

```http
POST /api/v1/payments/webhook/{gatewayName}
Headers:
  Stripe-Signature: xxx (signature for verification)
Body: {webhook payload from gateway}
```

### 6.3 Webhook Processing

```csharp
public async Task<bool> HandlePaymentWebhookAsync(
    string gatewayName, 
    string payload, 
    string signature)
{
    // 1. Verify signature (security)
    var adapter = _gatewayFactory.GetAdapter(gatewayName);
    var isValid = await adapter.VerifyWebhookSignatureAsync(payload, signature);
    
    if (!isValid)
    {
        _log.Warning("Invalid webhook signature from {Gateway}", gatewayName);
        return false;
    }
    
    // 2. Parse webhook event
    var webhookEvent = await adapter.ParseWebhookAsync(payload);
    
    // 3. Handle event based on type
    switch (webhookEvent.EventType)
    {
        case "payment_intent.succeeded":
            await HandlePaymentSuccessAsync(webhookEvent);
            break;
            
        case "payment_intent.payment_failed":
            await HandlePaymentFailedAsync(webhookEvent);
            break;
            
        case "charge.refunded":
            await HandleRefundAsync(webhookEvent);
            break;
            
        // ...
    }
    
    return true;
}
```

### 6.4 Webhook Security

**Verification:**
- Signature validation (HMAC)
- IP whitelist
- HTTPS required
- Idempotency (duplicate handling)

---

## 7. Payment Notifications

### 7.1 Notification Types

    **Invoice notifications:**
     [x] Invoice created
     [x] Payment reminder (before due)
     [x] Overdue invoice
     [x] Payment received (receipt)

    **Payment notifications:**
     [x] Payment failed
     [x] Authentication required
     [x] Refund processed

    **Subscription notifications:**
     [x] Subscription payment successful
     [x] Subscription payment failed
     [x] Payment method expiring soon

### 7.2 Email Templates

**Example: Receipt**
```
Subject: Payment Received - Invoice #INV-2024-001

Dear [Customer Name],

Thank you for your payment!

Invoice: #INV-2024-001
Amount: 1,250.00 EUR
Payment Date: February 9, 2024
Payment Method: Visa ****1234

[Download Receipt (PDF)]

Best regards,
ISP Customer Service
```

### 7.3 Service Implementation

```csharp
public async Task SendPaymentReceivedConfirmationAsync(int invoiceId)
{
    var invoice = await _context.Invoices
        .Include(i => i.Customer)
        .Include(i => i.InvoiceLines)
        .FirstOrDefaultAsync(i => i.Id == invoiceId);
    
    var emailDto = new QueueEmailDto
    {
        ToEmail = invoice.Customer.Email,
        ToName = invoice.Customer.Name,
        Subject = $"Payment Received - Invoice #{invoice.InvoiceNumber}",
        TemplateType = "PaymentReceipt",
        TemplateData = new Dictionary<string, object>
        {
            { "CustomerName", invoice.Customer.Name },
            { "InvoiceNumber", invoice.InvoiceNumber },
            { "Amount", invoice.TotalAmount },
            { "Currency", invoice.CurrencyCode },
            { "PaymentDate", DateTime.UtcNow }
        }
    };
    
    await _emailQueueService.QueueEmailAsync(emailDto);
}
```

---

## 8. Security

### 8.1 PCI DSS Compliance

**Principle: Never store sensitive card data**

**What we DO NOT store:**
- [NO] Full card number
- [NO] CVV/CVC code
- [NO] PIN code
- [NO] Magnetic stripe data

**What we store:**
- [YES] Token from gateway (encrypted)
- [YES] Last 4 digits
- [YES] Card brand (Visa, Mastercard)
- [YES] Expiry date

### 8.2 Token Security

**Encryption:**
```csharp
public class PaymentMethodToken
{
    // Encrypted before storage in database
    public string EncryptedToken { get; set; }
    
    // Gateway-specific IDs
    public string GatewayCustomerId { get; set; }
    public string GatewayPaymentMethodId { get; set; }
    
    // Display only
    public string Last4Digits { get; set; }
    public string CardBrand { get; set; }
}
```

**Best practices:**
- AES-256 encryption
- Unique encryption key per installation
- Key rotation policy
- Access logging

### 8.3 Fraud Detection

**Indicators we track:**

**PaymentAttempt tracking:**
```csharp
public string IpAddress { get; set; }
public string UserAgent { get; set; }
```

**Red flags:**
- Multiple failed attempts from same IP
- Many different cards from same IP
- High amounts from new customer
- Geographic mismatch (IP vs. card country)
- Velocity checking (too many transactions quickly)

**Future extension:**
```csharp
public interface IFraudDetectionService
{
    Task<FraudRiskScore> EvaluatePaymentAsync(PaymentAttempt attempt);
    Task<bool> IsBlacklistedAsync(string ipAddress);
    Task BlockSuspiciousPaymentAsync(int attemptId);
}
```

### 8.4 HTTPS and API Security

**Requirements:**
- [x] All payment endpoints require HTTPS
- [x] Authentication (JWT tokens)
- [x] Authorization (role-based)
- [x] Rate limiting
- [x] Input validation

---

## 9. Reporting and Analytics

### 9.1 Payment Analytics

**Key metrics:**
```sql
-- Payment success rate
SELECT 
    COUNT(CASE WHEN Status = 'Succeeded' THEN 1 END) * 100.0 / COUNT(*) as SuccessRate
FROM PaymentAttempts
WHERE CreatedAt >= DATEADD(day, -30, GETDATE())

-- Average payment processing time
SELECT 
    AVG(DATEDIFF(minute, pa.CreatedAt, pt.ProcessedAt)) as AvgMinutes
FROM PaymentAttempts pa
JOIN PaymentTransactions pt ON pa.PaymentTransactionId = pt.Id
WHERE pa.Status = 'Succeeded'

-- Top failure reasons
SELECT 
    ErrorCode,
    COUNT(*) as ErrorCount,
    AVG(RetryCount) as AvgRetries
FROM PaymentAttempts
WHERE Status = 'Failed'
GROUP BY ErrorCode
ORDER BY ErrorCount DESC
```

### 9.2 Dashboards

**Suggested widgets:**
- Total revenue (daily/weekly/monthly)
- Pending payments
- Failed payments
- Average transaction value
- Payment method distribution
- Gateway performance comparison

---

## 10. Error Handling and Recovery

### 10.1 Error Scenarios

**Scenario 1: Gateway Timeout**
```
Problem: Gateway does not respond within timeout
Action:
1. PaymentAttempt status -> Failed
2. ErrorCode: "GATEWAY_TIMEOUT"
3. Automatic retry scheduled (+1 hour)
4. Customer not notified (retry first)
```

**Scenario 2: Insufficient Funds**
```
Problem: Customer's card has insufficient funds
Action:
1. PaymentAttempt status -> Failed
2. ErrorCode: "INSUFFICIENT_FUNDS"
3. NO automatic retry
4. Customer notified immediately
5. Suggest alternative payment method
```

**Scenario 3: Expired Card**
```
Problem: Payment method expired
Action:
1. PaymentAttempt status -> Failed
2. ErrorCode: "EXPIRED_CARD"
3. NO retry
4. Customer notified to update payment method
5. Link to payment method page sent
```

**Scenario 4: 3D Secure Abandoned**
```
Problem: Customer abandons authentication
Action:
1. PaymentAttempt status -> Cancelled
2. AuthenticationStatus -> Failed
3. Invoice remains unpaid
4. Reminder sent later
```

### 10.2 Recovery Procedures

**Manual Intervention:**
```
Admin Dashboard -> Payment Attempts -> Filter: Failed
-> Select attempt -> Actions:
  - Retry payment
  - Mark as paid (if paid outside system)
  - Contact customer
  - Cancel invoice
```

**API for Retry:**
```http
POST /api/v1/payments/retry/78
Authorization: Bearer {admin-token}

RESPONSE:
{
  "isSuccess": true,
  "paymentAttemptId": 79,
  "status": "Processing"
}
```

---

## 11. Testing

### 11.1 Test Cards (Stripe Sandbox)

```
Successful payment:
4242 4242 4242 4242 | CVV: 123 | Exp: 12/25

Requires 3D Secure:
4000 0027 6000 3184 | CVV: 123 | Exp: 12/25

Declined (Insufficient Funds):
4000 0000 0000 9995 | CVV: 123 | Exp: 12/25

Expired card:
4000 0000 0000 0069 | CVV: 123 | Exp: 12/25
```

### 11.2 Test Scenarios

**Test 1: Normal Payment**
```
1. Create invoice
2. Select test card
3. Confirm payment
4. Verify PaymentAttempt.Status = Succeeded
5. Verify Invoice.Status = Paid
6. Verify PaymentTransaction created
```

**Test 2: 3D Secure Flow**
```
1. Create invoice
2. Select 3DS card
3. Verify redirect to authentication
4. Simulate authentication
5. Verify callback
6. Verify payment completed
```

**Test 3: Failed Payment + Retry**
```
1. Create invoice
2. Select "insufficient funds" card
3. Verify error recorded
4. Manual retry
5. Use valid card
6. Verify successful
```

---

## 12. Future Enhancements

### 12.1 Phase 2: Gateway Implementations

**Priority 1:**
- [ ] Stripe Adapter (international cards)
- [ ] PayPal Adapter (e-wallet)
- [ ] Vipps Adapter (Norwegian mobile payment)

**Priority 2:**
- [ ] Klarna Adapter (invoice/installments)
- [ ] Nets Easy Adapter (Nordic cards)

### 12.2 Phase 3: Automation

**Recurring Billing Background Service:**
```csharp
public class RecurringPaymentProcessor : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Get due subscriptions
            var dueSubscriptions = await GetDueSubscriptionsAsync();
            
            foreach (var subscription in dueSubscriptions)
            {
                await ProcessSubscriptionPaymentAsync(subscription);
            }
            
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
```

### 12.3 Phase 4: Advanced Features

**Payment Plans (Installment Plans):**
```csharp
public class PaymentPlan
{
    public int InvoiceId { get; set; }
    public int Installments { get; set; }
    public decimal InstallmentAmount { get; set; }
    public PaymentFrequency Frequency { get; set; } // Weekly, Monthly
    public DateTime FirstPaymentDate { get; set; }
}
```

**Smart Retry Logic:**
```csharp
// Adaptive retry based on error type
switch (errorCode)
{
    case "INSUFFICIENT_FUNDS":
        // Retry in 7 days (payday)
        nextRetry = GetNextPayday();
        break;
        
    case "EXPIRED_CARD":
        // Send reminder, don't retry
        notifyOnly = true;
        break;
        
    case "GATEWAY_TIMEOUT":
        // Aggressive retry (1h, 6h, 24h)
        nextRetry = CalculateExponentialBackoff();
        break;
}
```

**Receipt Generator:**
```csharp
public interface IReceiptGeneratorService
{
    Task<byte[]> GenerateInvoicePdfAsync(int invoiceId);
    Task<byte[]> GeneratePaymentReceiptPdfAsync(int transactionId);
    Task<string> GenerateInvoiceHtmlAsync(int invoiceId);
}
```

---

## 13. Summary - Data Flow

### 13.1 Entities Involved

```
Invoice
  +-- InvoiceLines
  +-- Customer
  |    +-- CustomerCredit
  |    +-- CustomerPaymentMethod
  |         +-- PaymentMethodToken
  +-- PaymentAttempts
  |    +-- Status tracking
  |    +-- Retry logic
  |    +-- 3D Secure handling
  +-- PaymentTransactions (successful payments)
  +-- InvoicePayments (payments applied to invoice)
```

### 13.2 Services Involved

```
PaymentProcessingService
  +-- Orchestrates entire payment flow
  +-- Creates PaymentAttempt
  +-- Calls Gateway Adapter
  +-- Creates PaymentTransaction
  +-- Updates Invoice status
  +-- Triggers notifications

PaymentNotificationService
  +-- Invoice created
  +-- Payment due reminder
  +-- Payment received
  +-- Payment failed
  +-- Card expiring

Gateway Adapters (Stripe, PayPal, etc.)
  +-- Charge operations
  +-- 3D Secure handling
  +-- Token management
  +-- Webhook processing
```

### 13.3 API Endpoints

```
POST   /api/v1/payments/process                      # Process payment
POST   /api/v1/payments/apply-credit                 # Apply credit
POST   /api/v1/payments/partial                      # Partial payment
POST   /api/v1/payments/retry/{attemptId}            # Retry failed
POST   /api/v1/payments/confirm-authentication/{id}  # Confirm 3DS
GET    /api/v1/payments/attempts/invoice/{id}        # Get attempts
GET    /api/v1/payments/attempts/{id}                # Get attempt
POST   /api/v1/payments/webhook/{gatewayName}        # Gateway webhook
```

---

## 14. Conclusion

The DR_Admin ISP Administration System now has a robust and scalable infrastructure for collecting and receiving payments from customers:

**Key Features:**
- **Complete audit trail** - All payment attempts tracked
- **Multiple payment methods** - Cards, credit, bank transfers
- **3D Secure ready** - PSD2 compliance
- **Gateway agnostic** - Easy to add new gateways
- **Automatic retry** - Intelligent error handling
- **Security** - PCI DSS principles, token-based
- **Scalability** - Handles high volume
- **Observability** - Full logging and reporting

**Current Status:** Phase 1 Complete - Ready for gateway implementations

---

**Documented:** February 9, 2024  
**Version:** 1.0  
**Author:** DR_Admin Development Team
