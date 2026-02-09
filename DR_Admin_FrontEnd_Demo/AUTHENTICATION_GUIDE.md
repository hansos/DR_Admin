# Authentication Guide - Frontend Demo

## Understanding the 401 Unauthorized Error

When you see `401 (Unauthorized)` error, it means you need to login first before accessing protected pages.

## Quick Fix: Login First

### Step 1: Navigate to Login Page
1. Go to: `http://localhost:7247/login.html` (or your frontend URL)
2. Enter your credentials
3. Click "Sign In"

### Step 2: Verify Login Success
After successful login:
- You should see "Login successful! Redirecting..."
- The page will redirect to the home page
- A JWT token will be stored in your session

### Step 3: Access Customer/User Pages
Now you can navigate to:
- `/customers.html` - Customer management
- `/users.html` - User management

---

## How Authentication Works

### Login Flow
```
1. User enters credentials on /login.html
2. Frontend calls: POST /api/Account/login
3. Frontend proxy calls: POST https://localhost:[YOUR_PORT]/api/v1/Auth/login
4. Backend validates credentials and returns JWT token
5. Frontend stores token in session: HttpContext.Session.SetString("AuthToken", token)
6. Token is now available for subsequent requests
```

### Protected Page Flow
```
1. User navigates to /customers.html
2. JavaScript calls: GET /api/Customers
3. Frontend proxy retrieves token: HttpContext.Session.GetString("AuthToken")
4. Proxy adds token to request header: Authorization: Bearer {token}
5. Backend validates token and returns data
```

---

## Troubleshooting Authentication Issues

### Issue 1: 401 Error on Protected Pages

**Symptoms:**
- Browser console shows: `GET /api/Customers 401 (Unauthorized)`
- Error message: "Authentication required"

**Solution:**
1. Open `/login.html` in your browser
2. Login with valid credentials
3. Navigate back to the protected page

**Test Credentials** (if using default database):
- Create a user via the backend API or seed data
- Or use the registration page: `/register.html`

---

### Issue 2: Token Not Being Stored

**Symptoms:**
- Login appears successful but still get 401 on protected pages
- Token not showing in session

**Debug Steps:**

1. **Check Login Response**
   Open browser console on login page:
   ```javascript
   // After login, check the response
   // Should contain: { token: "eyJ...", username: "...", email: "..." }
   ```

2. **Verify Token Storage**
   Check the `ApiAccountController.cs` login method:
   ```csharp
   // Should have this code:
   HttpContext.Session.SetString("AuthToken", loginResponse.Token);
   ```

3. **Check Session Configuration**
   In `Program.cs`, verify session is configured:
   ```csharp
   builder.Services.AddDistributedMemoryCache();
   builder.Services.AddSession(options => {
       options.IdleTimeout = TimeSpan.FromMinutes(30);
       options.Cookie.HttpOnly = true;
       options.Cookie.IsEssential = true;
   });
   
   // And in middleware:
   app.UseSession();
   ```

---

### Issue 3: Token Expired

**Symptoms:**
- Was working before, but now getting 401
- Session timeout

**Solution:**
- JWT tokens expire (default: 60 minutes in appsettings)
- Session expires (default: 30 minutes)
- Simply login again

---

## Manual Testing Authentication

### Test 1: Login and Check Token Storage

1. **Open Browser DevTools** (F12)
2. **Go to Application tab** ? Cookies
3. **Login** via `/login.html`
4. **Check** if `.AspNetCore.Session` cookie exists
5. **Navigate** to `/customers.html`
6. **Check Network tab** - should see `Authorization: Bearer ...` header

### Test 2: Verify Backend is Running

1. **Check backend URL** in `appsettings.json`:
   ```json
   {
     "DrAdminApi": {
       "BaseUrl": "https://localhost:[YOUR_PORT]"
     }
   }
   ```

2. **Test backend directly**:
   ```bash
   # Open browser or Postman
   POST https://localhost:[YOUR_PORT]/api/v1/Auth/login
   Content-Type: application/json
   
   {
     "username": "your-username",
     "password": "your-password"
   }
   ```

3. **Should return**:
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIs...",
     "username": "your-username",
     "email": "email@example.com",
     "roles": ["Admin"]
   }
   ```

---

## Auto-Redirect to Login

The latest code now automatically redirects to login page when:
- You try to access a protected page without authentication
- Your session expires
- You get a 401 error

**Message you'll see:**
> "Authentication required. Redirecting to login..."

Then you'll be redirected to `/login.html` after 2 seconds.

---

## Creating Test Users

If you don't have any users in the system:

### Option 1: Use Registration Page
1. Go to `/register.html`
2. Fill in the form
3. Complete email confirmation (if enabled)
4. Login with new credentials

### Option 2: Use Backend API Directly
```bash
POST https://localhost:[YOUR_PORT]/api/v1/MyAccount/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test123!",
  "confirmPassword": "Test123!",
  "customerName": "Test Customer",
  "customerEmail": "test@example.com",
  "customerPhone": "",
  "customerAddress": ""
}
```

### Option 3: Database Seed Data
Check if your `DR_Admin` project has seed data in the database initializer.

---

## Permission Errors (403 Forbidden)

If you get **403 Forbidden** instead of 401:

**This means:**
- You ARE logged in ?
- But you DON'T have the required role/permission ?

**Solutions:**

### For Customer Management Pages:
- **View**: Requires `Customer.Read` policy ? Admin or Sales role
- **Create/Edit**: Requires `Customer.Write` policy ? Admin or Sales role
- **Delete**: Requires `Customer.Delete` policy ? Admin role only

### For User Management Pages:
- **View**: Requires `User.Read` policy ? Admin or Support role
- **Create/Edit/Delete**: Requires `User.Write` and `User.Delete` ? Admin role only

**How to Fix:**
1. Login with an Admin account
2. OR assign appropriate roles to your user account via backend

---

## Session Management Best Practices

### Session Lifetime
- Default: 30 minutes of inactivity
- Adjust in `Program.cs`:
  ```csharp
  options.IdleTimeout = TimeSpan.FromMinutes(60); // 1 hour
  ```

### Token Lifetime
- Configured in backend `appsettings.json`:
  ```json
  "JwtSettings": {
    "ExpirationInMinutes": 60
  }
  ```

### Staying Logged In
- Keep browser window open
- Activity on the site extends session
- Or implement "Remember Me" functionality

---

## Common Scenarios

### Scenario 1: Fresh Start
```
1. Start backend: Run DR_Admin project
2. Start frontend: Run DR_Admin_FrontEnd_Demo project
3. Open browser: http://localhost:7247/login.html
4. Login with credentials
5. Navigate to: /customers.html or /users.html
? Should work!
```

### Scenario 2: After Restart
```
1. Close browser
2. Restart application
3. Open browser again
4. Try to access /customers.html
? Gets 401 - Need to login again
5. Go to /login.html and login
? Now works!
```

### Scenario 3: Session Expired
```
1. Login successfully
2. Leave page open for > 30 minutes
3. Try to use the page
? Gets 401 - Session expired
4. Auto-redirected to /login.html
5. Login again
? Works again!
```

---

## Quick Reference

### URLs
- Login: `/login.html`
- Register: `/register.html`
- Customers: `/customers.html`
- Users: `/users.html`

### Status Codes
- `200` - Success ?
- `401` - Not authenticated ? Login required
- `403` - Authenticated but no permission ? Need different role
- `404` - Page/resource not found
- `500` - Server error ? Check backend logs

### Browser Console Checks
```javascript
// Check if APIs are loaded
console.log(window.CustomerAPI);  // Should show object
console.log(window.UserAPI);      // Should show object
console.log(window.AuthAPI);      // Should show object
```

---

## Still Having Issues?

1. **Clear browser cache** (Ctrl+Shift+Delete)
2. **Check backend is running** (should be at https://localhost:[YOUR_PORT])
3. **Check frontend is running** (default: https://localhost:7247)
4. **Check browser console** for JavaScript errors
5. **Check Network tab** in DevTools for failed requests
6. **Verify credentials** are correct
7. **Check database** has user records
8. **Review backend logs** for authentication errors

---

## Next Steps After Login

Once successfully logged in:

1. ? Go to `/customers.html` to manage customers
2. ? Go to `/users.html` to manage users (Admin/Support only)
3. ? Create new records using the "New Customer" or "New User" buttons
4. ? Edit/Delete existing records (if you have permissions)

Enjoy using the DR Admin Frontend Demo! ??
