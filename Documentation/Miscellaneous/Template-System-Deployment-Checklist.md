# Email Template System - Deployment Checklist

## Pre-Deployment

### 1. Review Template Content
- [ ] Review all template files for correct branding
- [ ] Update company logo URLs in master templates
- [ ] Update contact information in footers
- [ ] Verify all URLs (customer portal, support, etc.)
- [ ] Check for spelling/grammar errors
- [ ] Test all placeholders are correctly named

### 2. Customize Master Templates

#### HTML Email Master (`Templates/Layouts/email.html.master.txt`)
- [ ] Update company name
- [ ] Update logo URL
- [ ] Update brand colors
- [ ] Update footer information
- [ ] Add social media links (if needed)
- [ ] Verify responsive design CSS

#### Plain Text Email Master (`Templates/Layouts/email.text.master.txt`)
- [ ] Update company name
- [ ] Update contact information
- [ ] Update copyright year

#### SMS Master (`Templates/Layouts/sms.master.txt`)
- [ ] Update company name suffix
- [ ] Verify character count (keep under 160)

### 3. Update Configuration

#### appsettings.json (Recommended)
```json
{
  "EmailTemplates": {
    "BasePath": "Templates",
    "CacheDurationMinutes": 10,
    "CustomerPortalUrl": "https://portal.yourcompany.com",
    "SupportUrl": "https://support.yourcompany.com",
    "CompanyName": "Your Company Name"
  }
}
```

#### Environment-Specific Settings
- [ ] Development: `appsettings.Development.json`
- [ ] Staging: `appsettings.Staging.json`
- [ ] Production: `appsettings.Production.json`

### 4. Update Event Handlers with Configuration

Instead of hardcoded URLs:
```csharp
// Before
CustomerPortalUrl = "https://portal.example.com/domains"

// After - inject IConfiguration
var portalUrl = _configuration["EmailTemplates:CustomerPortalUrl"];
CustomerPortalUrl = $"{portalUrl}/domains"
```

### 5. Testing

#### Unit Tests
- [ ] Run all template system tests
- [ ] Verify placeholder replacement
- [ ] Test master template injection
- [ ] Test caching behavior

#### Integration Tests
- [ ] Test DomainRegistered email sending
- [ ] Test DomainExpired email sending
- [ ] Verify emails arrive with correct formatting
- [ ] Test HTML and plain text versions
- [ ] Verify all links are clickable

#### Manual Testing
- [ ] Send test emails to personal inbox
- [ ] Check rendering in multiple email clients:
  - [ ] Gmail
  - [ ] Outlook
  - [ ] Apple Mail
  - [ ] Mobile email apps
- [ ] Verify responsive design on mobile
- [ ] Check spam score (tools: mail-tester.com)

### 6. Template File Permissions

Production server:
```bash
# Make templates directory read-only
chmod 444 Templates/**/*.txt

# Or specific files
chmod 444 Templates/Layouts/*.txt
chmod 444 Templates/DomainRegistered/*.txt
chmod 444 Templates/DomainExpired/*.txt
```

### 7. Backup Current Production Code

Before deploying:
- [ ] Tag current production version in Git
- [ ] Create backup of production database
- [ ] Document rollback procedure

---

## Deployment Steps

### 1. Deploy Code Changes

```bash
# 1. Commit all changes
git add .
git commit -m "Implement email template system"

# 2. Push to repository
git push origin master

# 3. Deploy to production server
# (Your deployment method - CI/CD, manual, etc.)
```

### 2. Deploy Template Files

Ensure Templates folder is deployed:
```
Templates/
??? Layouts/
?   ??? email.html.master.txt
?   ??? email.text.master.txt
?   ??? sms.master.txt
??? DomainRegistered/
?   ??? email.html.txt
?   ??? email.text.txt
?   ??? sms.txt
??? DomainExpired/
    ??? email.html.txt
    ??? email.text.txt
    ??? sms.txt
```

**Important**: Verify templates are deployed to correct path relative to application root!

### 3. Verify NuGet Packages

Ensure new package is restored:
```bash
dotnet restore
```

Required package:
- `Microsoft.Extensions.Caching.Memory` (v10.0.1 or compatible)

### 4. Restart Application

```bash
# Stop application
sudo systemctl stop your-app-service

# Start application
sudo systemctl start your-app-service

# Verify it started
sudo systemctl status your-app-service
```

### 5. Smoke Tests

Immediately after deployment:
- [ ] Application starts without errors
- [ ] Check application logs for template loading
- [ ] Trigger a test domain registration event
- [ ] Verify email is sent and formatted correctly
- [ ] Check for any error logs

---

## Post-Deployment

### 1. Monitor Logs

Watch for these potential issues:
```bash
# Template not found errors
grep "Template not found" /var/log/your-app/*.log

# Template rendering errors
grep "Error rendering message" /var/log/your-app/*.log

# FileNotFoundException
grep "FileNotFoundException" /var/log/your-app/*.log
```

### 2. Performance Monitoring

- [ ] Monitor memory usage (templates cached)
- [ ] Check template cache hit rate in logs
- [ ] Monitor email queue processing time
- [ ] Verify no performance degradation

### 3. User Acceptance Testing

- [ ] Register a real test domain
- [ ] Verify email received by customer
- [ ] Get feedback on email design
- [ ] Test renewal/expiration scenarios

---

## Rollback Procedure

If issues occur:

### Quick Rollback (Code only)
```bash
# 1. Revert to previous Git tag
git checkout <previous-tag>

# 2. Redeploy
# (Your deployment method)

# 3. Restart application
sudo systemctl restart your-app-service
```

### Database Rollback
Not required - no database schema changes

---

## Common Issues & Solutions

### Issue: Template Not Found
**Symptoms**: 
```
FileNotFoundException: Template not found: Templates/...
```

**Solutions**:
1. Verify Templates folder deployed to server
2. Check file paths (case-sensitive on Linux!)
3. Verify application working directory
4. Check file permissions (readable by application)

**Quick Fix**:
```bash
# On server, verify files exist
ls -la Templates/Layouts/
ls -la Templates/DomainRegistered/
```

### Issue: Placeholders Not Replaced
**Symptoms**: Email contains `{{DomainName}}` instead of actual domain

**Solutions**:
1. Verify model property names match placeholders exactly
2. Check for typos in template files
3. Verify data is passed to model correctly

**Quick Fix**:
Enable debug logging in event handlers to see model data

### Issue: Email Formatting Broken
**Symptoms**: HTML not rendering, looks like plain text

**Solutions**:
1. Verify `isHtml = true` when sending
2. Check master template has valid HTML structure
3. Verify no extra characters in template files (BOM, etc.)

**Quick Fix**:
Re-save template files with UTF-8 encoding (no BOM)

### Issue: High Memory Usage
**Symptoms**: Memory usage increases after deployment

**Solutions**:
1. Reduce cache duration from 10 to 5 minutes
2. Implement cache size limits
3. Monitor for template cache leaks

**Quick Fix**:
Restart application to clear cache

---

## Maintenance

### Regular Tasks

#### Weekly
- [ ] Review email delivery logs
- [ ] Check for bounced emails
- [ ] Monitor spam reports

#### Monthly
- [ ] Review and update template content
- [ ] Update copyright year (if needed)
- [ ] Analyze email open/click rates
- [ ] Update templates based on feedback

#### Quarterly
- [ ] Security review of template files
- [ ] Performance optimization review
- [ ] A/B test new email designs

### Template Updates

When updating templates:
1. Test changes in development environment
2. Preview rendered emails
3. Deploy during low-traffic period
4. Monitor for issues after deployment
5. Cache will auto-refresh in 10 minutes

**Note**: Template changes don't require application restart!

---

## Security Checklist

- [ ] Template files are read-only in production
- [ ] No sensitive information in templates (API keys, passwords, etc.)
- [ ] User input properly sanitized before passing to templates
- [ ] Email content doesn't execute scripts
- [ ] Template directory not web-accessible
- [ ] Verify SPF/DKIM/DMARC records for email domain

---

## Documentation

Ensure team has access to:
- [ ] `Templates/README.md` - Template system documentation
- [ ] `Documentation/Email-Template-System-QuickStart.md` - Quick start guide
- [ ] `Documentation/Template-System-Implementation-Summary.md` - Implementation details
- [ ] This deployment checklist

---

## Success Criteria

Deployment is successful when:
- ? Application starts without errors
- ? Domain registration emails send correctly
- ? Domain expiration emails send correctly
- ? Emails render properly in major email clients
- ? No increase in error logs
- ? No performance degradation
- ? Customer feedback is positive

---

## Support Contacts

If issues arise:
- Developer: [Your contact]
- DevOps: [DevOps contact]
- Email Infrastructure: [Email team contact]

---

## Notes

Add deployment-specific notes here:
- Deployment date: _______________
- Deployed by: _______________
- Production URL: _______________
- Any issues encountered: _______________
- Rollback performed: [ ] Yes [ ] No
