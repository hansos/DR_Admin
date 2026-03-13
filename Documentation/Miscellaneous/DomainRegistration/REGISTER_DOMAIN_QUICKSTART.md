# Domain Registration Page - Quick Start Guide

## QUICK START

### 1. Prerequisites
Before using the domain registration page, ensure:

- [X] DR_Admin API is running on https://localhost:7201
- [X] Database is configured and migrations applied
- [X] At least one registrar exists in the database
- [X] TLD pricing is populated in RegistrarTld table
- [X] You have a valid customer account with credentials

### 2. Configuration

#### Backend (appsettings.json)
```json
{
  "DomainRegistration": {
    "AllowCustomerRegistration": true,
    "DefaultRegistrarId": 1,
    "MaxRegistrationYears": 10,
    "MinRegistrationYears": 1,
    "DefaultRegistrationYears": 1,
    "RequireApprovalForCustomers": false
  }
}
```

#### Database Setup
```sql
-- Ensure a default registrar exists
INSERT INTO Registrars (Name, IsActive, CreatedAt)
VALUES ('Default Registrar', 1, GETDATE());

-- Add TLD pricing (example)
INSERT INTO RegistrarTlds (RegistrarId, Tld, RegistrationPrice, RenewalPrice, TransferPrice, IsAvailable)
VALUES 
  (1, 'com', 12.99, 12.99, 12.99, 1),
  (1, 'net', 14.99, 14.99, 14.99, 1),
  (1, 'org', 12.99, 12.99, 12.99, 1);
```

### 3. Access the Page

#### Option A: From Dashboard
1. Start DR_Admin_Web project
2. Navigate to https://localhost:7XXX/index.html
3. Login with customer credentials
4. Click "Register Domain" card

#### Option B: Direct URL
1. Navigate directly to https://localhost:7XXX/register-domain.html
2. You'll be redirected to login if not authenticated
3. After login, you'll be redirected back to the registration page

### 4. Register Your First Domain

#### Step-by-Step
1. Enter Domain Name
   - Type: example.com
   - Click: "Check Availability"

2. Check Result
   - [SUCCESS] Available: Green box appears
   - [ERROR] Not Available: Red box with alternatives

3. Configure Registration
   - Click: "Proceed to Registration"
   - Select: Registration period (years)
   - Toggle: Auto-renewal (recommended)
   - Toggle: Privacy protection (optional)
   - Add: Notes (optional)

4. Review & Submit
   - Review: Estimated total price
   - Click: "Register Domain"

5. Confirmation
   - View: Order number and invoice amount
   - Option: Register another domain
   - Option: Return to dashboard

### 5. Verify Registration

#### Check in Database
```sql
-- View the registered domain
SELECT * FROM RegisteredDomains 
WHERE DomainName = 'example.com';

-- View the order
SELECT * FROM Orders 
WHERE Id = (SELECT OrderId FROM RegisteredDomains WHERE DomainName = 'example.com');

-- View the invoice
SELECT * FROM Invoices 
WHERE OrderId = (SELECT OrderId FROM RegisteredDomains WHERE DomainName = 'example.com');
```

#### Check via API (Swagger)
1. Navigate to https://localhost:7201/swagger
2. Authenticate with JWT token
3. Try: GET /api/v1/RegisteredDomains
4. Look for your domain in the list

## TESTING SCENARIOS

### Test 1: Successful Registration
```
Input: example.com
Expected: Domain registered, order created, invoice generated
```

### Test 2: Duplicate Domain
```
Input: example.com (already registered)
Expected: "Not available" message
```

### Test 3: Invalid Domain
```
Input: invalid..domain
Expected: Validation error
```

### Test 4: Multi-Year Registration
```
Input: example.net, 3 years
Expected: Price = base price x 3
```

### Test 5: Privacy Protection
```
Input: example.org, 1 year, privacy enabled
Expected: Price = base price + $10
```

## TROUBLESHOOTING

### Issue: "Customer domain registration is currently disabled"
```
Solution: Update appsettings.json
"AllowCustomerRegistration": true
```

### Issue: "Customer ID not found in authentication token"
```
Solution: Ensure JWT contains CustomerId claim
Check token in jwt.io
Verify ClaimsIdentity in backend includes CustomerId
```

### Issue: "Pricing not found for TLD"
```sql
-- Solution: Add pricing for the TLD
INSERT INTO RegistrarTlds (RegistrarId, Tld, RegistrationPrice, RenewalPrice, TransferPrice, IsAvailable)
VALUES (1, 'com', 12.99, 12.99, 12.99, 1);
```

### Issue: "Registrar with ID X not found or inactive"
```sql
-- Solution: Check registrar exists and is active
UPDATE Registrars SET IsActive = 1 WHERE Id = 1;
```

### Issue: Page shows "Not authenticated"
```
Solution: 
1. Clear localStorage
2. Login again
3. Check token expiration
```

## MOBILE TESTING

### Chrome DevTools
1. Press F12
2. Click "Toggle device toolbar" (Ctrl+Shift+M)
3. Select device: iPhone 12 Pro
4. Test responsiveness

### Actual Device
1. Get local IP: ipconfig (Windows) or ifconfig (Mac/Linux)
2. Update API URL if needed
3. Access: https://[YOUR-IP]:7XXX/register-domain.html
4. Accept SSL certificate warning

## CUSTOMIZATION

### Change Colors
Edit register-domain.html:
```css
/* Primary gradient */
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

/* Success color */
background-color: #28a745;

/* Info color */
background-color: #2196F3;
```

### Change API URL
Edit domainRegistrationClient.js:
```javascript
const API_BASE_URL = 'https://your-api-url.com';
```

### Adjust Privacy Protection Price
Edit register-domain.html (line ~468):
```javascript
total += 10 * years; // Change 10 to your price
```

## MONITORING

### Check Browser Console
```javascript
// Open console (F12)
// Look for errors or API call logs
console.log('Domain registration:', result);
```

### Check Network Tab
1. Open DevTools (F12)
2. Go to "Network" tab
3. Filter: "Fetch/XHR"
4. Watch API calls in real-time

### Check API Logs
```
# In DR_Admin API output window
# Look for lines like:
[INF] API: RegisterDomain called for domain example.com by customer 123
```

## SECURITY NOTES

### Token Storage
- Tokens stored in localStorage
- Not accessible to other domains
- Cleared on logout

### HTTPS Required
- API must use HTTPS in production
- Mixed content (HTTP/HTTPS) blocked by browsers

### CORS Configuration
```csharp
// In DR_Admin Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWeb", policy =>
    {
        policy.WithOrigins("https://localhost:7XXX")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

## NEXT STEPS

After successful first registration:

1. Set up payment processing
   - Integrate PaymentGatewayLib
   - Configure payment methods
   - Test invoice payment flow

2. Configure email notifications
   - Use EmailSenderLib
   - Send registration confirmation
   - Send payment reminders

3. Set up domain renewal
   - Create renewal page
   - Implement auto-renewal workflow
   - Send renewal reminders

4. Add admin features
   - Domain management interface
   - Bulk operations
   - Approval workflow (if needed)

## SUPPORT RESOURCES

- Documentation: REGISTER_DOMAIN_PAGE.md
- API Docs: https://localhost:7201/swagger
- Flow Diagram: REGISTER_DOMAIN_FLOW_DIAGRAM.md
- Implementation: DOMAIN_REGISTRATION_IMPLEMENTATION.md

## CHECKLIST

Before going to production:

- [ ] Configure production API URL
- [ ] Set up SSL certificates
- [ ] Configure CORS for production domain
- [ ] Test with real customer accounts
- [ ] Verify payment integration
- [ ] Test email notifications
- [ ] Set up monitoring and logging
- [ ] Perform security audit
- [ ] Test on multiple browsers
- [ ] Test on mobile devices
- [ ] Create user documentation
- [ ] Train support staff

---

Happy Domain Registering!
