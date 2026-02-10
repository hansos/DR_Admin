# Password Reset Functionality Fix

## Problem
The `reset-password.html` page was failing when sending password reset emails with:
- **Error**: `Failed to load resource: the server responded with a status of 404`
- **Endpoint called**: `https://localhost:7201/api/v1/myaccount/reset-password`
- **Root cause**: The password reset functionality was completely missing from the backend

## Changes Made

### 1. Backend API Changes (DR_Admin project)

#### Added RequestPasswordResetDto
**File**: `DR_Admin\DTOs\MyAccountDto.cs`
- Added new DTO class `RequestPasswordResetDto` with `Email` property

#### Updated IMyAccountService Interface
**File**: `DR_Admin\Services\IMyAccountService.cs`
- Added method: `Task<bool> RequestPasswordResetAsync(string email)`

#### Implemented Password Reset Service
**File**: `DR_Admin\Services\MyAccountService.cs`
- Implemented `RequestPasswordResetAsync` method that:
  - Validates the email address
  - Revokes existing password reset tokens for security
  - Generates a new password reset token (valid for 24 hours)
  - Returns success even if user not found (prevents email enumeration attacks)
  - **Note**: Email sending is logged but not yet implemented - requires integration with `EmailSenderLib`

#### Added Controller Endpoint
**File**: `DR_Admin\Controllers\MyAccountController.cs`
- Added endpoint: `POST api/v1/myaccount/request-password-reset`
- Accepts `RequestPasswordResetDto` with email
- Returns success message
- Implements security best practices (always returns same message)

### 2. Frontend Changes (DR_Admin_FrontEnd_Demo project)

#### Updated API Client
**File**: `DR_Admin_FrontEnd_Demo\wwwroot\js\api-client.ts`
- Changed `BASE_URL` from `/api` to `https://localhost:7201/api/v1` (direct backend calls)
- Updated `credentials` from `'same-origin'` to `'include'` (CORS support)
- Fixed endpoint paths:
  - `login`: `auth/login` (with `username` parameter instead of `email`)
  - `register`: `myaccount/register` (with full DTO structure)
  - `resetPassword`: `myaccount/request-password-reset` (new endpoint)

#### Compiled TypeScript
- Compiled changes to `api-client.js`

### 3. CORS Configuration
The backend `appsettings.json` already includes the frontend URL in allowed origins:
- `https://localhost:7247` ? (frontend URL)

## API Endpoint Details

### Request Password Reset
**POST** `https://localhost:7201/api/v1/myaccount/request-password-reset`

**Request Body**:
```json
{
  "email": "user@example.com"
}
```

**Response** (200 OK):
```json
{
  "message": "If the email address exists in our system, a password reset link has been sent."
}
```

**Security Features**:
- Always returns 200 OK even if email doesn't exist (prevents email enumeration)
- Revokes all existing password reset tokens when new one is requested
- Tokens expire after 24 hours
- Tokens are one-time use (revoked when password is set)

## Next Steps - Email Integration

The password reset token is generated but **emails are not yet being sent**. To complete the implementation:

### Required Changes:

1. **Add IEmailSender dependency to MyAccountService**:
   ```csharp
   private readonly IEmailSender _emailSender;
   
   public MyAccountService(ApplicationDbContext context, IConfiguration configuration, IEmailSender emailSender)
   {
       _context = context;
       _configuration = configuration;
       _emailSender = emailSender;
   }
   ```

2. **Update RequestPasswordResetAsync to send emails**:
   ```csharp
   // Build reset URL
   var resetUrl = $"{_configuration["AppSettings:BaseUrl"]}/reset-password?token={resetToken}&email={email}";
   
   // Send email
   await _emailSender.SendEmailAsync(
       email, 
       "Password Reset Request",
       $"Click here to reset your password: <a href='{resetUrl}'>{resetUrl}</a>",
       isHtml: true
   );
   ```

3. **Configure EmailSenderLib in Program.cs** (if not already done)

4. **Add BaseUrl to appsettings.json**:
   ```json
   {
     "AppSettings": {
       "BaseUrl": "https://localhost:7247"
     }
   }
   ```

## Testing

### Manual Test Steps:
1. Start DR_Admin backend (https://localhost:7201)
2. Start DR_Admin_FrontEnd_Demo (https://localhost:7247)
3. Navigate to https://localhost:7247/reset-password.html
4. Enter an email address
5. Click "Send Reset Link"
6. Check the DR_Admin logs for the generated token
7. Currently: Token will be logged but no email sent

### Expected Behavior:
- ? No more 404 errors
- ? Success message displayed
- ? Token generated and logged
- ? Email sending (pending integration)

## Files Modified

### Backend (DR_Admin)
- `Services/IMyAccountService.cs` - Added interface method
- `Services/MyAccountService.cs` - Implemented password reset logic
- `Controllers/MyAccountController.cs` - Added endpoint
- `DTOs/MyAccountDto.cs` - Added RequestPasswordResetDto

### Frontend (DR_Admin_FrontEnd_Demo)
- `wwwroot/js/api-client.ts` - Updated API endpoints
- `wwwroot/js/api-client.js` - Compiled TypeScript

## Architecture Notes

The frontend now calls the backend API **directly** (not through proxy controllers):
```
Browser (localhost:7247)
  ? CORS request
DR_Admin API (localhost:7201)
```

This matches the architecture described in:
- `DR_Admin_FrontEnd_Demo/Documentation/DIRECT_API_CONFIGURATION.md`
- `DR_Admin_FrontEnd_Demo/Documentation/CUSTOMER_USER_MANAGEMENT.md`
