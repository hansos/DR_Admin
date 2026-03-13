# TLD Pricing Refactoring - Implementation Checklist

## ? Completed (What We Just Did)

- [x] Created `RegistrarTldCostPricing` entity
- [x] Created `TldSalesPricing` entity  
- [x] Created `ResellerTldDiscount` entity
- [x] Created `RegistrarSelectionPreference` entity
- [x] Updated `RegistrarTld` with navigation properties
- [x] Updated `Tld` with navigation properties
- [x] Updated `Registrar` with navigation properties
- [x] Updated `ResellerCompany` with navigation properties
- [x] Updated `ApplicationDbContext` with new DbSets
- [x] Created `ITldPricingService` interface
- [x] Created all pricing DTOs
- [x] Created migration guide documentation
- [x] Created architecture summary
- [x] Created visual diagrams and examples

## ?? Phase 1: Database Setup (Day 1)

### Migration Tasks
- [ ] Create EF Core migration
  ```bash
  dotnet ef migrations add AddTldPricingTables
  ```
- [ ] Review generated migration file
- [ ] Add indexes manually if not auto-generated:
  - [ ] `IX_RegistrarTldCostPricing_RegistrarTldId_EffectiveFrom`
  - [ ] `IX_RegistrarTldCostPricing_EffectiveTo`
  - [ ] `IX_TldSalesPricing_TldId_EffectiveFrom`
  - [ ] `IX_TldSalesPricing_EffectiveTo`
  - [ ] `IX_ResellerTldDiscount_ResellerCompanyId_TldId`
  - [ ] `IX_ResellerTldDiscount_EffectiveTo`
  - [ ] `IX_RegistrarSelectionPreference_RegistrarId`
  - [ ] `IX_RegistrarSelectionPreference_Priority`
- [ ] Apply migration to development database
  ```bash
  dotnet ef database update
  ```
- [ ] Verify all tables and indexes created

### Configuration Tasks
- [ ] Configure entity relationships in `OnModelCreating`
- [ ] Add foreign key constraints
- [ ] Add check constraints (e.g., EffectiveFrom < EffectiveTo)
- [ ] Configure decimal precision for price fields

## ?? Phase 2: Data Migration (Day 1-2)

### Preparation
- [ ] Backup production database
- [ ] Create data migration script
- [ ] Test migration script on copy of production data
- [ ] Verify row counts match

### Migration Script
- [ ] Migrate `RegistrarTld` costs ? `RegistrarTldCostPricing`
  ```sql
  -- Copy costs from RegistrarTld to new table
  INSERT INTO RegistrarTldCostPricing (...)
  SELECT ... FROM RegistrarTld
  ```
- [ ] Migrate `RegistrarTld` prices ? `TldSalesPricing`
  ```sql
  -- One sales price per TLD (deduplicate if multiple registrars)
  INSERT INTO TldSalesPricing (...)
  SELECT DISTINCT ON (TldId) ... FROM RegistrarTld
  ```
- [ ] Verify all prices migrated correctly
- [ ] Check for missing/null values
- [ ] Run data validation queries

### Validation Queries
- [ ] Count check: `SELECT COUNT(*) FROM RegistrarTld` vs new tables
- [ ] Price check: Verify prices match between old and new tables
- [ ] Currency check: Ensure all currencies populated
- [ ] Date check: Ensure all EffectiveFrom dates set

## ?? Phase 3: Service Layer (Day 2-4)

### TldPricingService Implementation
- [ ] Create `TldPricingService.cs` class
- [ ] Implement `GetCurrentCostPricingAsync`
- [ ] Implement `GetCurrentSalesPricingAsync`
- [ ] Implement `GetCostPricingAtDateAsync`
- [ ] Implement `GetSalesPricingAtDateAsync`
- [ ] Implement `GetCostPricingHistoryAsync`
- [ ] Implement `GetSalesPricingHistoryAsync`
- [ ] Implement `CreateCostPricingAsync`
- [ ] Implement `CreateSalesPricingAsync`
- [ ] Implement `ScheduleCostPriceChangeAsync`
- [ ] Implement `ScheduleSalesPriceChangeAsync`
- [ ] Implement `GetActiveDiscountAsync`
- [ ] Implement `GetResellerDiscountsAsync`
- [ ] Implement `CreateDiscountAsync`
- [ ] Implement `DeactivateDiscountAsync`
- [ ] Implement `CalculateEffectivePriceAsync`
- [ ] Implement `SelectBestRegistrarAsync`
- [ ] Implement `CalculateMarginAsync`

### Helper Methods
- [ ] Create temporal query extension methods
- [ ] Create price calculation helpers
- [ ] Create currency conversion helpers (if needed)
- [ ] Create validation helpers

### Unit Tests
- [ ] Test temporal queries (past, current, future dates)
- [ ] Test price calculations
- [ ] Test discount applications
- [ ] Test registrar selection logic
- [ ] Test edge cases (no pricing, expired pricing, etc.)
- [ ] Test multi-currency scenarios

## ?? Phase 4: Controllers & API (Day 4-6)

### New Controller Endpoints
- [ ] `POST /api/registrar-tlds/{id}/cost-pricing` - Create cost pricing
- [ ] `GET /api/registrar-tlds/{id}/cost-pricing` - Get current cost pricing
- [ ] `GET /api/registrar-tlds/{id}/cost-pricing/history` - Get cost history
- [ ] `PUT /api/registrar-tlds/{id}/cost-pricing/schedule` - Schedule cost change
- [ ] `POST /api/tlds/{id}/sales-pricing` - Create sales pricing
- [ ] `GET /api/tlds/{id}/sales-pricing` - Get current sales pricing
- [ ] `GET /api/tlds/{id}/sales-pricing/history` - Get sales history
- [ ] `PUT /api/tlds/{id}/sales-pricing/schedule` - Schedule price change
- [ ] `POST /api/reseller-companies/{id}/tld-discounts` - Create discount
- [ ] `GET /api/reseller-companies/{id}/tld-discounts` - Get reseller discounts
- [ ] `DELETE /api/reseller-companies/{id}/tld-discounts/{discountId}` - Remove discount
- [ ] `GET /api/tlds/{id}/effective-price` - Calculate effective price
- [ ] `GET /api/tlds/{id}/margin` - Get margin analysis

### Update Existing Endpoints
- [ ] Update `RegistrarTldsController` to read from new tables
- [ ] Update DTOs to include new pricing fields
- [ ] Mark old price properties as `[Obsolete]`
- [ ] Maintain backward compatibility

### API Documentation
- [ ] Update Swagger/OpenAPI documentation
- [ ] Add XML comments to new endpoints
- [ ] Document request/response examples
- [ ] Update API changelog

### Integration Tests
- [ ] Test all new endpoints
- [ ] Test backward compatibility
- [ ] Test error scenarios
- [ ] Test authorization

## ?? Phase 5: Frontend/Client Updates (Day 7-8)

### Admin UI Updates
- [ ] Create pricing management pages
- [ ] Create cost pricing form
- [ ] Create sales pricing form
- [ ] Create discount management page
- [ ] Add price history views
- [ ] Add margin analysis dashboard
- [ ] Add registrar selection preferences

### Customer-Facing Updates
- [ ] Update domain pricing display
- [ ] Show promotional pricing
- [ ] Show discount applications
- [ ] Update cart calculations

## ?? Phase 6: Testing (Day 8-10)

### Manual Testing
- [ ] Test creating new cost pricing
- [ ] Test scheduling future price changes
- [ ] Test promotional pricing
- [ ] Test customer discounts
- [ ] Test registrar selection
- [ ] Test margin calculations
- [ ] Test multi-currency

### Automated Testing
- [ ] Run all unit tests
- [ ] Run all integration tests
- [ ] Run end-to-end tests
- [ ] Performance testing
- [ ] Load testing

### Edge Case Testing
- [ ] No pricing exists
- [ ] Pricing gaps (EffectiveTo ? EffectiveFrom gap)
- [ ] Overlapping pricing
- [ ] Expired discounts
- [ ] Negative margins
- [ ] Multi-year registrations during price change

## ?? Phase 7: Deployment (Day 11-12)

### Pre-Deployment
- [ ] Code review
- [ ] Security review
- [ ] Performance review
- [ ] Database backup
- [ ] Rollback plan prepared

### Staging Deployment
- [ ] Deploy to staging
- [ ] Run smoke tests
- [ ] Test data migration on staging
- [ ] Verify all features work

### Production Deployment
- [ ] Schedule maintenance window (if needed)
- [ ] Run database migration
- [ ] Deploy new code
- [ ] Run smoke tests
- [ ] Monitor logs for errors
- [ ] Monitor performance metrics

### Post-Deployment
- [ ] Verify all API endpoints working
- [ ] Check error logs
- [ ] Monitor database performance
- [ ] User acceptance testing
- [ ] Announce new features

## ?? Phase 8: Cleanup (Day 12+)

### Code Cleanup
- [ ] Remove old price columns from `RegistrarTld` (after verification period)
- [ ] Remove deprecated endpoints
- [ ] Remove obsolete DTOs
- [ ] Update documentation

### Database Cleanup
- [ ] Archive old data (if needed)
- [ ] Optimize indexes
- [ ] Update statistics

### Documentation
- [ ] Update user documentation
- [ ] Update admin documentation
- [ ] Update API documentation
- [ ] Create training materials

## ?? Ongoing Maintenance

### Daily
- [ ] Monitor scheduled price changes (ensure they activate correctly)
- [ ] Check for negative margins
- [ ] Monitor API errors

### Weekly
- [ ] Review margin reports
- [ ] Check for pricing gaps/overlaps
- [ ] Verify discount applications

### Monthly
- [ ] Audit price change history
- [ ] Review registrar costs vs sales prices
- [ ] Analyze customer discounts

### Quarterly
- [ ] Archive old pricing data (older than 7 years for costs, 5 years for sales)
- [ ] Review registrar selection preferences
- [ ] Optimize database indexes

## ?? Rollback Plan

If issues occur after deployment:

### Immediate Rollback (Critical Issues)
1. Revert to previous code version
2. Restore database backup
3. Verify old system functioning
4. Investigate issues

### Partial Rollback (Minor Issues)
1. Disable new pricing endpoints (feature flag)
2. Revert service layer to read from old columns
3. Fix issues
4. Re-deploy

### Data Rollback
- [ ] Backup current state
- [ ] Restore pre-migration backup
- [ ] Verify data integrity
- [ ] Re-run migration (if fixed)

## ?? Success Metrics

### Technical Metrics
- [ ] All unit tests passing (100%)
- [ ] All integration tests passing (100%)
- [ ] API response time < 200ms (95th percentile)
- [ ] Database query time < 50ms (average)
- [ ] Zero data loss
- [ ] Zero downtime deployment

### Business Metrics
- [ ] All historical prices preserved
- [ ] Scheduled price changes activate on time
- [ ] Customer discounts apply correctly
- [ ] Margin calculations accurate
- [ ] Multi-currency support working

## ?? Training Checklist

### Admin Training
- [ ] How to create cost pricing
- [ ] How to schedule price changes
- [ ] How to create promotions
- [ ] How to manage customer discounts
- [ ] How to view margin reports
- [ ] How to configure registrar selection

### Developer Training
- [ ] New entity models
- [ ] Service layer usage
- [ ] Temporal query patterns
- [ ] API endpoints
- [ ] Testing strategies

## ?? Support Plan

### During Implementation
- [ ] Daily standup meetings
- [ ] Slack/Teams channel for questions
- [ ] Weekly progress review
- [ ] Blocker resolution process

### Post-Deployment
- [ ] On-call rotation (first week)
- [ ] Bug tracking system
- [ ] User feedback collection
- [ ] Issue escalation process

---

## ?? Ready to Start?

**Recommended Start Date**: ___________  
**Target Completion Date**: ___________  
**Project Lead**: ___________  
**Team Members**: ___________

**Next Step**: Create EF Core migration and review migration file

```bash
dotnet ef migrations add AddTldPricingTables
```

Good luck! ??
