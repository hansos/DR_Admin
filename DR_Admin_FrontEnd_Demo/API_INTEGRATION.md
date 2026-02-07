# DR Admin API Integration Guide

## ? Configuration Complete!

Your frontend is now configured to connect to the **DR_Admin backend API** at `https://localhost:7201`.

## ?? What Was Updated

### 1. **appsettings.json**
```json
{
  "DrAdminApi": {
    "BaseUrl": "https://localhost:7201",
    "Timeout": 30
  }
}
```

### 2. **Program.cs**
- Added `HttpClient` service for API calls
- Added CORS configuration
- Configured base URL from settings

### 3. **ApiAccountController.cs** (Example)
- Proxies requests to the real DR_Admin API
- Handles authentication endpoints
- Includes error handling and logging

### 4. **api-client.ts**
- Updated to call through frontend proxy (`/api/*`)
- Maintains same interface for TypeScript code
- Already compiled to JavaScript

## ?? Architecture

```
Browser
  ? (calls /api/Account/login)
Frontend (localhost:5001)
  ? (proxies to https://localhost:7201/api/auth/login)
DR_Admin API (localhost:7201)
```

## ?? Next Steps to Complete Integration

### 1. **Map DR_Admin API Endpoints**

You need to know the actual endpoint paths in your DR_Admin API. Update the controller methods:

**In `ApiAccountController.cs`:**
```csharp
// Find the correct endpoint in your DR_Admin API
var response = await client.PostAsJsonAsync("/api/auth/login", request);
```

Common DR_Admin endpoints (update these based on your actual API):
- Login: `/api/auth/login` or `/api/account/signin`
- Register: `/api/auth/register` or `/api/account/signup`
- Reset Password: `/api/auth/reset-password`
- Domains: `/api/domains/search`, `/api/domains/list`
- Hosting: `/api/hosting/plans`, `/api/hosting/services`
- Customers: `/api/customers/list`
- Orders: `/api/orders/list`

### 2. **Create Proxy Controllers for Other Endpoints**

Create similar controllers for:
- `ApiDomainsController.cs`
- `ApiHostingController.cs`
- `ApiCustomersController.cs`
- `ApiOrdersController.cs`

**Template:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ApiDomainsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiDomainsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetDomains()
    {
        var client = _httpClientFactory.CreateClient("DrAdminApi");
        var response = await client.GetAsync("/api/domains/list");
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }
        
        return StatusCode((int)response.StatusCode);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchDomain([FromQuery] string domain)
    {
        var client = _httpClientFactory.CreateClient("DrAdminApi");
        var response = await client.GetAsync($"/api/domains/search?domain={domain}");
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }
        
        return StatusCode((int)response.StatusCode);
    }
}
```

### 3. **Handle Authentication Tokens**

If DR_Admin API returns JWT tokens:

**In ApiAccountController.cs:**
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    var client = _httpClientFactory.CreateClient("DrAdminApi");
    var response = await client.PostAsJsonAsync("/api/auth/login", request);

    if (response.IsSuccessStatusCode)
    {
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        
        // Store token in session or return to client
        if (!string.IsNullOrEmpty(result?.Token))
        {
            HttpContext.Session.SetString("AuthToken", result.Token);
        }

        return Ok(result);
    }

    return StatusCode((int)response.StatusCode);
}
```

**Add token to subsequent requests:**
```csharp
[HttpGet("list")]
public async Task<IActionResult> GetDomains()
{
    var client = _httpClientFactory.CreateClient("DrAdminApi");
    
    // Add auth token from session
    var token = HttpContext.Session.GetString("AuthToken");
    if (!string.IsNullOrEmpty(token))
    {
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
    
    var response = await client.GetAsync("/api/domains/list");
    // ...
}
```

### 4. **Configure CORS in DR_Admin API**

Your DR_Admin backend needs to allow requests from `https://localhost:5001`.

**In DR_Admin `Program.cs` or `Startup.cs`:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ...

app.UseCors();
```

### 5. **Test the Integration**

1. **Start DR_Admin API:**
   ```bash
   cd DR_Admin
   dotnet run
   ```
   Should run on `https://localhost:7201`

2. **Start Frontend:**
   ```bash
   cd DR_Admin_FrontEnd_Demo
   dotnet run
   ```
   Should run on `https://localhost:5001`

3. **Test Login:**
   - Go to `https://localhost:5001/login.html`
   - Try logging in with valid credentials
   - Check browser DevTools Console for any errors
   - Check Network tab to see API calls

### 6. **Troubleshooting**

**CORS Errors?**
- Ensure CORS is configured in both frontend and backend
- Check allowed origins match exactly

**404 Errors?**
- Verify endpoint paths in DR_Admin API
- Use browser DevTools to see actual URLs being called
- Check DR_Admin API documentation

**Authentication Issues?**
- Verify token is being stored and sent
- Check token format (Bearer, etc.)
- Ensure token isn't expired

**Connection Refused?**
- Ensure DR_Admin API is running on port 7201
- Check firewall settings
- Try `https` vs `http`

## ?? Resources

- **Frontend Code:** `DR_Admin_FrontEnd_Demo/`
- **Backend API:** `DR_Admin/`
- **API Endpoints:** Check `DR_Admin/Controllers/` folder
- **Frontend API Calls:** `wwwroot/js/api-client.ts`

## ?? Quick Checklist

- [x] Updated `appsettings.json` with API URL
- [x] Added `HttpClient` in `Program.cs`
- [x] Created example `ApiAccountController.cs`
- [x] Updated `api-client.ts`
- [ ] Map all DR_Admin API endpoints
- [ ] Create proxy controllers for all features
- [ ] Implement JWT token handling
- [ ] Configure CORS in DR_Admin backend
- [ ] Test all pages end-to-end
- [ ] Remove any remaining demo/simulated responses

## ?? You're Ready!

Your frontend is now configured to talk to the real DR_Admin API. Just need to:
1. Create the remaining proxy controllers
2. Test with the actual API running
3. Handle authentication properly

**Good luck! ??**
