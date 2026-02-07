# DR Admin Frontend Demo - Quick Start Guide

## ? What You Have

A complete **pure HTML/TypeScript/CSS** frontend demo - **NO Razor, NO Blazor, NO frameworks**.

## ?? Run It Now

```bash
cd DR_Admin_FrontEnd_Demo
dotnet run
```

Then open: `https://localhost:5001`

## ?? Available Pages

- **/** - Homepage
- **/login.html** - Login (use any email/password in demo mode)
- **/register.html** - Registration
- **/reset-password.html** - Password reset
- **/domains.html** - Domain management
- **/hosting.html** - Hosting services
- **/customers.html** - Customer management  
- **/orders.html** - Order history

## ?? Key Features

- ? Pure HTML pages (no templating)
- ? TypeScript compiled to JavaScript
- ? Bootstrap 5.3 styling
- ? Responsive navigation
- ? Simulated API responses
- ? Client-side authentication (demo)
- ? All workflows functional

## ?? Development

**Compile TypeScript:**
```bash
tsc
```

**Watch mode (auto-compile):**
```bash
tsc --watch
```

**Build project:**
```bash
dotnet build
```

## ?? File Structure

```
wwwroot/
??? *.html          # Pure HTML pages
??? css/site.css    # Custom styles
??? js/
    ??? *.ts        # TypeScript source
    ??? *.js        # Compiled JavaScript

Controllers/         # Minimal API endpoints (C#)
Program.cs          # ASP.NET Core setup
```

## ?? Connect to Real API

1. Update `Controllers/*Controller.cs` - replace simulated responses
2. Configure API URL in `appsettings.json`
3. Update `wwwroot/js/api-client.ts` - set BASE_URL
4. Add authentication headers
5. Test all endpoints

See **README.md** for detailed integration guide.

## ?? Quick Tips

- **Add new page?** Create HTML in `wwwroot/`, TS in `wwwroot/js/`, run `tsc`
- **Customize styles?** Edit `wwwroot/css/site.css`
- **Change API endpoints?** Edit `wwwroot/js/api-client.ts`
- **Modify navigation?** Update nav bar in each HTML file

## ?? Full Documentation

See **README.md** for complete guide, architecture, and integration instructions.

---

**That's it! You're ready to explore the demo!** ??
