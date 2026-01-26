using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing subscriptions
/// </summary>
public class SubscriptionService : ISubscriptionService
{
    private readonly ApplicationDbContext _context;
    private readonly IInvoiceService _invoiceService;
    private readonly IEmailQueueService _emailQueueService;
    private static readonly Serilog.ILogger _log = Log.ForContext<SubscriptionService>();

    public SubscriptionService(
        ApplicationDbContext context,
        IInvoiceService invoiceService,
        IEmailQueueService emailQueueService)
    {
        _context = context;
        _invoiceService = invoiceService;
        _emailQueueService = emailQueueService;
    }

    /// <summary>
    /// Retrieves all subscriptions
    /// </summary>
    public async Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync()
    {
        try
        {
            _log.Information("Fetching all subscriptions");

            var subscriptions = await _context.Set<Subscription>()
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var subscriptionDtos = subscriptions.Select(MapToDto);

            _log.Information("Successfully fetched {Count} subscriptions", subscriptions.Count);
            return subscriptionDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all subscriptions");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all subscriptions for a specific customer
    /// </summary>
    public async Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByCustomerIdAsync(int customerId)
    {
        try
        {
            _log.Information("Fetching subscriptions for customer ID: {CustomerId}", customerId);

            var subscriptions = await _context.Set<Subscription>()
                .AsNoTracking()
                .Where(s => s.CustomerId == customerId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var subscriptionDtos = subscriptions.Select(MapToDto);

            _log.Information("Successfully fetched {Count} subscriptions for customer ID: {CustomerId}",
                subscriptions.Count, customerId);
            return subscriptionDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching subscriptions for customer ID: {CustomerId}", customerId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all subscriptions with a specific status
    /// </summary>
    public async Task<IEnumerable<SubscriptionDto>> GetSubscriptionsByStatusAsync(SubscriptionStatus status)
    {
        try
        {
            _log.Information("Fetching subscriptions with status: {Status}", status);

            var subscriptions = await _context.Set<Subscription>()
                .AsNoTracking()
                .Where(s => s.Status == status)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var subscriptionDtos = subscriptions.Select(MapToDto);

            _log.Information("Successfully fetched {Count} subscriptions with status: {Status}",
                subscriptions.Count, status);
            return subscriptionDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching subscriptions with status: {Status}", status);
            throw;
        }
    }

    /// <summary>
    /// Retrieves subscriptions that are due for billing
    /// </summary>
    public async Task<IEnumerable<SubscriptionDto>> GetDueSubscriptionsAsync(DateTime? dueDate = null)
    {
        try
        {
            var checkDate = dueDate ?? DateTime.UtcNow;
            _log.Information("Fetching subscriptions due for billing on or before: {DueDate}", checkDate);

            var subscriptions = await _context.Set<Subscription>()
                .AsNoTracking()
                .Where(s => s.Status == SubscriptionStatus.Active &&
                           s.NextBillingDate <= checkDate)
                .OrderBy(s => s.NextBillingDate)
                .ToListAsync();

            var subscriptionDtos = subscriptions.Select(MapToDto);

            _log.Information("Successfully fetched {Count} due subscriptions", subscriptions.Count);
            return subscriptionDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching due subscriptions");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a subscription by its unique identifier
    /// </summary>
    public async Task<SubscriptionDto?> GetSubscriptionByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching subscription with ID: {SubscriptionId}", id);

            var subscription = await _context.Set<Subscription>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subscription == null)
            {
                _log.Warning("Subscription with ID: {SubscriptionId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched subscription with ID: {SubscriptionId}", id);
            return MapToDto(subscription);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching subscription with ID: {SubscriptionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new subscription
    /// </summary>
    public async Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto createDto)
    {
        try
        {
            _log.Information("Creating new subscription for customer ID: {CustomerId}, service ID: {ServiceId}",
                createDto.CustomerId, createDto.ServiceId);

            var startDate = createDto.StartDate ?? DateTime.UtcNow;
            var trialEndDate = createDto.TrialDays > 0
                ? startDate.AddDays(createDto.TrialDays)
                : (DateTime?)null;

            var firstBillingDate = trialEndDate ?? startDate;
            var nextBillingDate = CalculateNextBillingDate(firstBillingDate,
                createDto.BillingPeriodCount, createDto.BillingPeriodUnit);

            var subscription = new Subscription
            {
                CustomerId = createDto.CustomerId,
                ServiceId = createDto.ServiceId,
                BillingCycleId = createDto.BillingCycleId,
                CustomerPaymentMethodId = createDto.CustomerPaymentMethodId,
                Status = trialEndDate.HasValue ? SubscriptionStatus.Trialing : SubscriptionStatus.Active,
                StartDate = startDate,
                EndDate = createDto.EndDate,
                NextBillingDate = nextBillingDate,
                CurrentPeriodStart = startDate,
                CurrentPeriodEnd = nextBillingDate,
                Amount = createDto.Amount,
                CurrencyCode = createDto.CurrencyCode,
                BillingPeriodCount = createDto.BillingPeriodCount,
                BillingPeriodUnit = createDto.BillingPeriodUnit,
                TrialEndDate = trialEndDate,
                IsInTrial = trialEndDate.HasValue,
                RetryCount = 0,
                MaxRetryAttempts = createDto.MaxRetryAttempts,
                Metadata = createDto.Metadata,
                Notes = createDto.Notes,
                Quantity = createDto.Quantity,
                SendEmailNotifications = createDto.SendEmailNotifications,
                AutoRetryFailedPayments = createDto.AutoRetryFailedPayments,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Set<Subscription>().Add(subscription);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created subscription with ID: {SubscriptionId}", subscription.Id);

            // Send welcome email if notifications enabled
            if (subscription.SendEmailNotifications)
            {
                await SendSubscriptionEmailAsync(subscription.CustomerId, "SubscriptionCreated",
                    $"Your subscription has been created successfully. Next billing date: {subscription.NextBillingDate:yyyy-MM-dd}");
            }

            return MapToDto(subscription);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating subscription");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing subscription
    /// </summary>
    public async Task<SubscriptionDto?> UpdateSubscriptionAsync(int id, UpdateSubscriptionDto updateDto)
    {
        try
        {
            _log.Information("Updating subscription with ID: {SubscriptionId}", id);

            var subscription = await _context.Set<Subscription>()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subscription == null)
            {
                _log.Warning("Subscription with ID: {SubscriptionId} not found", id);
                return null;
            }

            // Update only provided fields
            if (updateDto.CustomerPaymentMethodId.HasValue)
                subscription.CustomerPaymentMethodId = updateDto.CustomerPaymentMethodId.Value;

            if (updateDto.EndDate.HasValue)
                subscription.EndDate = updateDto.EndDate.Value;

            if (updateDto.Amount.HasValue)
                subscription.Amount = updateDto.Amount.Value;

            if (!string.IsNullOrEmpty(updateDto.CurrencyCode))
                subscription.CurrencyCode = updateDto.CurrencyCode;

            if (updateDto.BillingPeriodCount.HasValue)
                subscription.BillingPeriodCount = updateDto.BillingPeriodCount.Value;

            if (updateDto.BillingPeriodUnit.HasValue)
                subscription.BillingPeriodUnit = updateDto.BillingPeriodUnit.Value;

            if (updateDto.MaxRetryAttempts.HasValue)
                subscription.MaxRetryAttempts = updateDto.MaxRetryAttempts.Value;

            if (!string.IsNullOrEmpty(updateDto.Metadata))
                subscription.Metadata = updateDto.Metadata;

            if (!string.IsNullOrEmpty(updateDto.Notes))
                subscription.Notes = updateDto.Notes;

            if (updateDto.Quantity.HasValue)
                subscription.Quantity = updateDto.Quantity.Value;

            if (updateDto.SendEmailNotifications.HasValue)
                subscription.SendEmailNotifications = updateDto.SendEmailNotifications.Value;

            if (updateDto.AutoRetryFailedPayments.HasValue)
                subscription.AutoRetryFailedPayments = updateDto.AutoRetryFailedPayments.Value;

            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated subscription with ID: {SubscriptionId}", id);
            return MapToDto(subscription);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating subscription with ID: {SubscriptionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Cancels a subscription
    /// </summary>
    public async Task<SubscriptionDto?> CancelSubscriptionAsync(int id, CancelSubscriptionDto cancelDto)
    {
        try
        {
            _log.Information("Cancelling subscription with ID: {SubscriptionId}", id);

            var subscription = await _context.Set<Subscription>()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subscription == null)
            {
                _log.Warning("Subscription with ID: {SubscriptionId} not found", id);
                return null;
            }

            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.CancelledAt = DateTime.UtcNow;
            subscription.CancellationReason = cancelDto.CancellationReason;

            if (cancelDto.CancelImmediately)
            {
                subscription.EndDate = DateTime.UtcNow;
            }
            else
            {
                // Cancel at end of current billing period
                subscription.EndDate = subscription.CurrentPeriodEnd;
            }

            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully cancelled subscription with ID: {SubscriptionId}", id);

            // Send cancellation email if notifications enabled
            if (subscription.SendEmailNotifications)
            {
                await SendSubscriptionEmailAsync(subscription.CustomerId, "SubscriptionCancelled",
                    $"Your subscription has been cancelled. End date: {subscription.EndDate:yyyy-MM-dd}");
            }

            return MapToDto(subscription);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while cancelling subscription with ID: {SubscriptionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Pauses a subscription
    /// </summary>
    public async Task<SubscriptionDto?> PauseSubscriptionAsync(int id, PauseSubscriptionDto pauseDto)
    {
        try
        {
            _log.Information("Pausing subscription with ID: {SubscriptionId}", id);

            var subscription = await _context.Set<Subscription>()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subscription == null)
            {
                _log.Warning("Subscription with ID: {SubscriptionId} not found", id);
                return null;
            }

            subscription.Status = SubscriptionStatus.Paused;
            subscription.PausedAt = DateTime.UtcNow;
            subscription.PauseReason = pauseDto.PauseReason;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully paused subscription with ID: {SubscriptionId}", id);

            // Send pause email if notifications enabled
            if (subscription.SendEmailNotifications)
            {
                await SendSubscriptionEmailAsync(subscription.CustomerId, "SubscriptionPaused",
                    "Your subscription has been paused.");
            }

            return MapToDto(subscription);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while pausing subscription with ID: {SubscriptionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Resumes a paused subscription
    /// </summary>
    public async Task<SubscriptionDto?> ResumeSubscriptionAsync(int id)
    {
        try
        {
            _log.Information("Resuming subscription with ID: {SubscriptionId}", id);

            var subscription = await _context.Set<Subscription>()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subscription == null)
            {
                _log.Warning("Subscription with ID: {SubscriptionId} not found", id);
                return null;
            }

            if (subscription.Status != SubscriptionStatus.Paused)
            {
                _log.Warning("Cannot resume subscription {SubscriptionId} with status: {Status}",
                    id, subscription.Status);
                throw new InvalidOperationException($"Cannot resume subscription with status: {subscription.Status}");
            }

            subscription.Status = SubscriptionStatus.Active;
            subscription.PausedAt = null;
            subscription.PauseReason = string.Empty;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully resumed subscription with ID: {SubscriptionId}", id);

            // Send resume email if notifications enabled
            if (subscription.SendEmailNotifications)
            {
                await SendSubscriptionEmailAsync(subscription.CustomerId, "SubscriptionResumed",
                    $"Your subscription has been resumed. Next billing date: {subscription.NextBillingDate:yyyy-MM-dd}");
            }

            return MapToDto(subscription);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while resuming subscription with ID: {SubscriptionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a subscription
    /// </summary>
    public async Task<bool> DeleteSubscriptionAsync(int id)
    {
        try
        {
            _log.Information("Deleting subscription with ID: {SubscriptionId}", id);

            var subscription = await _context.Set<Subscription>()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subscription == null)
            {
                _log.Warning("Subscription with ID: {SubscriptionId} not found", id);
                return false;
            }

            _context.Set<Subscription>().Remove(subscription);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted subscription with ID: {SubscriptionId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting subscription with ID: {SubscriptionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Processes billing for a specific subscription
    /// </summary>
    public async Task<bool> ProcessSubscriptionBillingAsync(int subscriptionId)
    {
        try
        {
            _log.Information("Processing billing for subscription ID: {SubscriptionId}", subscriptionId);

            var subscription = await _context.Set<Subscription>()
                .Include(s => s.Customer)
                .Include(s => s.Service)
                .Include(s => s.CustomerPaymentMethod)
                .FirstOrDefaultAsync(s => s.Id == subscriptionId);

            if (subscription == null)
            {
                _log.Warning("Subscription with ID: {SubscriptionId} not found", subscriptionId);
                return false;
            }

            // Check if subscription is in valid state for billing
            if (subscription.Status != SubscriptionStatus.Active &&
                subscription.Status != SubscriptionStatus.Trialing &&
                subscription.Status != SubscriptionStatus.PastDue)
            {
                _log.Warning("Cannot bill subscription {SubscriptionId} with status: {Status}",
                    subscriptionId, subscription.Status);
                return false;
            }

            // Check if in trial period
            if (subscription.IsInTrial && subscription.TrialEndDate.HasValue &&
                DateTime.UtcNow < subscription.TrialEndDate.Value)
            {
                _log.Information("Subscription {SubscriptionId} is still in trial period until {TrialEndDate}",
                    subscriptionId, subscription.TrialEndDate.Value);
                return true; // Not an error, just skip billing
            }

            // Update trial status if trial has ended
            if (subscription.IsInTrial && subscription.TrialEndDate.HasValue &&
                DateTime.UtcNow >= subscription.TrialEndDate.Value)
            {
                subscription.IsInTrial = false;
                subscription.Status = SubscriptionStatus.Active;
            }

            // Create invoice
            var invoiceDto = await CreateSubscriptionInvoiceAsync(subscription);

            if (invoiceDto == null)
            {
                _log.Error("Failed to create invoice for subscription {SubscriptionId}", subscriptionId);
                return false;
            }

            // Update subscription billing dates
            subscription.LastBillingAttempt = DateTime.UtcNow;
            subscription.CurrentPeriodStart = subscription.NextBillingDate;
            subscription.NextBillingDate = CalculateNextBillingDate(
                subscription.NextBillingDate,
                subscription.BillingPeriodCount,
                subscription.BillingPeriodUnit);
            subscription.CurrentPeriodEnd = subscription.NextBillingDate;
            subscription.LastSuccessfulBilling = DateTime.UtcNow;
            subscription.RetryCount = 0;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully processed billing for subscription {SubscriptionId}, invoice {InvoiceNumber} created",
                subscriptionId, invoiceDto.InvoiceNumber);

            // Send billing email if notifications enabled
            if (subscription.SendEmailNotifications)
            {
                await SendSubscriptionEmailAsync(subscription.CustomerId, "SubscriptionBilled",
                    $"Your subscription has been billed. Invoice: {invoiceDto.InvoiceNumber}, Amount: {invoiceDto.TotalAmount:C}");
            }

            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while processing billing for subscription ID: {SubscriptionId}", subscriptionId);

            // Update retry count
            var subscription = await _context.Set<Subscription>()
                .FirstOrDefaultAsync(s => s.Id == subscriptionId);

            if (subscription != null)
            {
                subscription.RetryCount++;
                subscription.LastBillingAttempt = DateTime.UtcNow;

                if (subscription.RetryCount >= subscription.MaxRetryAttempts)
                {
                    subscription.Status = SubscriptionStatus.PastDue;
                    _log.Warning("Subscription {SubscriptionId} marked as PastDue after {RetryCount} failed attempts",
                        subscriptionId, subscription.RetryCount);

                    // Send failure email
                    if (subscription.SendEmailNotifications)
                    {
                        await SendSubscriptionEmailAsync(subscription.CustomerId, "SubscriptionPaymentFailed",
                            "Your subscription payment has failed. Please update your payment method.");
                    }
                }

                await _context.SaveChangesAsync();
            }

            return false;
        }
    }

    // Private helper methods

    private static SubscriptionDto MapToDto(Subscription subscription)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            CustomerId = subscription.CustomerId,
            ServiceId = subscription.ServiceId,
            BillingCycleId = subscription.BillingCycleId,
            CustomerPaymentMethodId = subscription.CustomerPaymentMethodId,
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            NextBillingDate = subscription.NextBillingDate,
            CurrentPeriodStart = subscription.CurrentPeriodStart,
            CurrentPeriodEnd = subscription.CurrentPeriodEnd,
            Amount = subscription.Amount,
            CurrencyCode = subscription.CurrencyCode,
            BillingPeriodCount = subscription.BillingPeriodCount,
            BillingPeriodUnit = subscription.BillingPeriodUnit,
            TrialEndDate = subscription.TrialEndDate,
            IsInTrial = subscription.IsInTrial,
            RetryCount = subscription.RetryCount,
            MaxRetryAttempts = subscription.MaxRetryAttempts,
            LastBillingAttempt = subscription.LastBillingAttempt,
            LastSuccessfulBilling = subscription.LastSuccessfulBilling,
            CancelledAt = subscription.CancelledAt,
            CancellationReason = subscription.CancellationReason,
            PausedAt = subscription.PausedAt,
            PauseReason = subscription.PauseReason,
            Metadata = subscription.Metadata,
            Notes = subscription.Notes,
            Quantity = subscription.Quantity,
            SendEmailNotifications = subscription.SendEmailNotifications,
            AutoRetryFailedPayments = subscription.AutoRetryFailedPayments,
            CreatedAt = subscription.CreatedAt,
            UpdatedAt = subscription.UpdatedAt
        };
    }

    private static DateTime CalculateNextBillingDate(DateTime fromDate, int periodCount, SubscriptionPeriodUnit periodUnit)
    {
        return periodUnit switch
        {
            SubscriptionPeriodUnit.Days => fromDate.AddDays(periodCount),
            SubscriptionPeriodUnit.Months => fromDate.AddMonths(periodCount),
            SubscriptionPeriodUnit.Years => fromDate.AddYears(periodCount),
            _ => fromDate.AddMonths(periodCount)
        };
    }

    private async Task<InvoiceDto?> CreateSubscriptionInvoiceAsync(Subscription subscription)
    {
        try
        {
            var invoiceNumber = $"SUB-{subscription.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var dueDate = DateTime.UtcNow.AddDays(7); // 7 days payment term

            var createInvoiceDto = new CreateInvoiceDto
            {
                InvoiceNumber = invoiceNumber,
                CustomerId = subscription.CustomerId,
                Status = InvoiceStatus.Issued,
                IssueDate = DateTime.UtcNow,
                DueDate = dueDate,
                SubTotal = subscription.Amount * subscription.Quantity,
                TaxAmount = 0, // Tax calculation would go here
                TotalAmount = subscription.Amount * subscription.Quantity,
                CurrencyCode = subscription.CurrencyCode,
                CustomerName = subscription.Customer?.Name ?? string.Empty,
                CustomerAddress = subscription.Customer?.Address ?? string.Empty,
                Notes = $"Subscription billing for {subscription.Service?.Name ?? "Service"}",
                InternalComment = $"Auto-generated from subscription ID: {subscription.Id}"
            };

            return await _invoiceService.CreateInvoiceAsync(createInvoiceDto);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Failed to create invoice for subscription {SubscriptionId}", subscription.Id);
            return null;
        }
    }

    private async Task SendSubscriptionEmailAsync(int customerId, string subject, string body)
    {
        try
        {
            var customer = await _context.Set<Customer>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null || string.IsNullOrEmpty(customer.Email))
            {
                _log.Warning("Cannot send email for customer {CustomerId}: customer not found or no email", customerId);
                return;
            }

            var emailDto = new QueueEmailDto
            {
                To = customer.Email,
                Subject = subject,
                BodyText = body
            };

            await _emailQueueService.QueueEmailAsync(emailDto);
            _log.Information("Email queued for customer {CustomerId}: {Subject}", customerId, subject);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Failed to send email for customer {CustomerId}", customerId);
        }
    }
}
