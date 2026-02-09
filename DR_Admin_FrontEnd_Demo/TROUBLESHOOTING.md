# Troubleshooting Guide - Frontend Demo

## Common Issues and Solutions

### Issue 1: CSS File Not Found (404 Error)

**Error**: `site.css:1 Failed to load resource: the server responded with a status of 404 ()`

**Cause**: The `site.css` file was missing from `wwwroot/css/` directory.

**Solution**: 
- The file has been recreated at `DR_Admin_FrontEnd_Demo/wwwroot/css/site.css`
- If you see this error again:
  1. Stop the application
  2. Verify the file exists: `DR_Admin_FrontEnd_Demo/wwwroot/css/site.css`
  3. Clear browser cache (Ctrl+Shift+Delete)
  4. Restart the application

---

### Issue 2: UserAPI is Not Defined

**Error**: `Uncaught ReferenceError: UserAPI is not defined at api-client.js:138:22`

**Cause**: The `api-client.js` file was missing the `UserAPI` object definition.

**Solution**: 
- Updated `api-client.js` to include complete `UserAPI` and `CustomerAPI` definitions
- The file now exports both APIs properly

**Verification**:
```javascript
// Open browser console on any page that loads api-client.js
console.log(window.UserAPI);    // Should show the UserAPI object
console.log(window.CustomerAPI); // Should show the CustomerAPI object
```

---

### Issue 3: Wrong Customer API Endpoint

**Error**: `api/Customers/list:1 Failed to load resource: the server responded with a status of 400 ()`

**Cause**: Old code was calling `/api/Customers/list` instead of `/api/Customers`

**Solution**:
- Updated `CustomerAPI.getCustomers()` to use correct endpoint: `/api/Customers`
- Added pagination support: `/api/Customers?pageNumber=1&pageSize=10`

**Before**:
```javascript
async getCustomers() {
    return apiRequest(`${BASE_URL}/Customers/list`, {
        method: 'GET',
    });
}
```

**After**:
```javascript
async getCustomers(pageNumber, pageSize) {
    let url = `${BASE_URL}/Customers`;
    if (pageNumber && pageSize) {
        url += `?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    }
    return apiRequest(url, { method: 'GET' });
}
```

---

## How to Apply Fixes While Debugging

Since the app is currently running:

### Option 1: Hot Reload (Recommended)
1. Save all modified files
2. The application should automatically hot reload
3. Refresh the browser page (F5)

### Option 2: Full Restart
1. Stop debugging (Shift+F5)
2. Clear browser cache
3. Rebuild solution (Ctrl+Shift+B)
4. Start debugging again (F5)

---

## Verification Steps

After applying fixes, verify everything works:

### 1. Check CSS is Loading
1. Open browser DevTools (F12)
2. Go to Network tab
3. Refresh page (F5)
4. Look for `site.css` - should show status 200 (not 404)

### 2. Check JavaScript APIs
Open browser console and run:
```javascript
// Should all return objects, not undefined
console.log(window.AuthAPI);
console.log(window.CustomerAPI);
console.log(window.UserAPI);
console.log(window.DomainAPI);
console.log(window.OrderAPI);
```

### 3. Test Customer List Page
1. Navigate to `/customers.html`
2. Open browser console (F12)
3. Should see no JavaScript errors
4. If authenticated, customer list should load
5. If not authenticated, should see appropriate error message

### 4. Test User List Page
1. Navigate to `/users.html`
2. Should see no JavaScript errors
3. If authenticated with proper role, users should load

---

## Important Notes

### Authentication Required
Most pages require:
- Valid login session
- JWT token stored in session
- Appropriate role permissions

If you see 401/403 errors:
1. Go to `/login.html`
2. Log in with valid credentials
3. Navigate back to the page

### Browser Cache
If changes don't appear:
1. Hard refresh: Ctrl+F5 (Windows) or Cmd+Shift+R (Mac)
2. Clear cache: Ctrl+Shift+Delete
3. Try incognito/private window

### Session Storage
JWT tokens are stored in server-side session:
- Login stores the token
- Token is automatically added to API requests
- Closing browser may clear session (depends on configuration)

---

## Quick Reference: All Endpoints

### Authentication
- `POST /api/Account/login` - Login
- `POST /api/Account/register` - Register
- `POST /api/Account/reset-password` - Reset password

### Customers
- `GET /api/Customers` - Get all (supports pagination)
- `GET /api/Customers/{id}` - Get by ID
- `POST /api/Customers` - Create
- `PUT /api/Customers/{id}` - Update
- `DELETE /api/Customers/{id}` - Delete

### Users
- `GET /api/Users` - Get all (supports pagination)
- `GET /api/Users/{id}` - Get by ID
- `POST /api/Users` - Create
- `PUT /api/Users/{id}` - Update
- `DELETE /api/Users/{id}` - Delete

---

## Still Having Issues?

1. Check browser console for JavaScript errors
2. Check Network tab for failed requests
3. Verify backend API is running (default: https://localhost:[YOUR_PORT])
4. Check appsettings.json for correct backend URL
5. Ensure all files are saved
6. Try clean rebuild: Build > Clean Solution, then Build > Rebuild Solution
