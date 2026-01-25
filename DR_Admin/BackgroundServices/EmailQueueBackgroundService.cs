using System.Threading.Channels;
using EmailSenderLib.Interfaces;
using ISPAdmin.Services;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ISPAdmin.BackgroundServices;

/// <summary>
/// Background service that processes the email queue using hybrid approach:
/// - In-memory Channel for immediate notification
/// - Database polling as fallback
/// - Respects provider throttling limits
/// </summary>
public class EmailQueueBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<int> _emailNotificationChannel;
    private static readonly Serilog.ILogger _log = Log.ForContext<EmailQueueBackgroundService>();
    
    // Throttling configuration
    private readonly int _maxEmailsPerMinute = 60; // Default, can be overridden from provider
    private readonly TimeSpan _pollInterval = TimeSpan.FromMinutes(2);
    private readonly int _batchSize = 10;
    
    private DateTime _lastPollTime = DateTime.MinValue;
    private int _emailsSentThisMinute = 0;
    private DateTime _currentMinuteStart = DateTime.UtcNow;

    public EmailQueueBackgroundService(
        IServiceProvider serviceProvider, 
        Channel<int> emailNotificationChannel)
    {
        _serviceProvider = serviceProvider;
        _emailNotificationChannel = emailNotificationChannel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.Information("Email Queue Background Service starting");

        // Populate channel with existing pending emails on startup
        await PopulateChannelOnStartup(stoppingToken);

        // Run two tasks in parallel: channel listener and periodic poller
        var channelTask = ProcessChannelNotificationsAsync(stoppingToken);
        var pollerTask = PeriodicPollerAsync(stoppingToken);

        await Task.WhenAll(channelTask, pollerTask);
    }

    /// <summary>
    /// Populates the channel with existing pending emails from database on startup
    /// </summary>
    private async Task PopulateChannelOnStartup(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var emailQueueService = scope.ServiceProvider.GetRequiredService<IEmailQueueService>();

            var pendingEmailIds = await emailQueueService.GetPendingEmailIdsAsync(100);
            
            foreach (var emailId in pendingEmailIds)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;
                    
                await _emailNotificationChannel.Writer.WriteAsync(emailId, stoppingToken);
            }

            _log.Information("Populated channel with {Count} pending emails on startup", pendingEmailIds.Count());
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error populating channel on startup");
        }
    }

    /// <summary>
    /// Listens to the in-memory channel for immediate email processing
    /// </summary>
    private async Task ProcessChannelNotificationsAsync(CancellationToken stoppingToken)
    {
        _log.Information("Channel notification processor started");

        await foreach (var emailId in _emailNotificationChannel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                // Respect throttling
                await WaitForThrottleIfNeeded(stoppingToken);

                await ProcessEmailAsync(emailId, stoppingToken);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error processing email {EmailId} from channel", emailId);
            }
        }
    }

    /// <summary>
    /// Periodic poller as fallback to catch emails that might have been missed
    /// </summary>
    private async Task PeriodicPollerAsync(CancellationToken stoppingToken)
    {
        _log.Information("Periodic poller started with interval: {Interval}", _pollInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_pollInterval, stoppingToken);

                _log.Information("Running periodic poll for pending emails");

                using var scope = _serviceProvider.CreateScope();
                var emailQueueService = scope.ServiceProvider.GetRequiredService<IEmailQueueService>();

                var pendingEmailIds = await emailQueueService.GetPendingEmailIdsAsync(_batchSize);

                foreach (var emailId in pendingEmailIds)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    // Add to channel for processing (deduplication handled by channel)
                    await _emailNotificationChannel.Writer.WriteAsync(emailId, stoppingToken);
                }

                _log.Information("Periodic poll added {Count} emails to channel", pendingEmailIds.Count());
            }
            catch (TaskCanceledException)
            {
                // Expected during shutdown
                break;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in periodic poller");
            }
        }
    }

    /// <summary>
    /// Processes a single email from the queue
    /// </summary>
    private async Task ProcessEmailAsync(int emailId, CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var emailQueueService = scope.ServiceProvider.GetRequiredService<IEmailQueueService>();

        try
        {
            // Mark as in-progress (atomic operation to prevent duplicate processing)
            var marked = await emailQueueService.MarkEmailInProgressAsync(emailId);
            if (!marked)
            {
                _log.Debug("Email {EmailId} already being processed or in invalid state", emailId);
                return;
            }

            var email = await emailQueueService.GetEmailByIdAsync(emailId);
            if (email == null)
            {
                _log.Warning("Email {EmailId} not found", emailId);
                return;
            }

            _log.Information("Processing email {EmailId} to {To}", emailId, email.To);

            // Get email sender (can be provider-specific based on email.Provider)
            var emailSender = GetEmailSender(scope, email.Provider);

            // Parse attachments if any
            var attachments = !string.IsNullOrEmpty(email.Attachments)
                ? email.Attachments.Split(';').ToList()
                : new List<string>();

            // Determine body and format to send
            string body;
            bool isHtml;
            
            if (!string.IsNullOrEmpty(email.BodyHtml))
            {
                body = email.BodyHtml;
                isHtml = true;
            }
            else if (!string.IsNullOrEmpty(email.BodyText))
            {
                body = email.BodyText;
                isHtml = false;
            }
            else
            {
                throw new InvalidOperationException($"Email {emailId} has no body content");
            }

            // Send the email
            if (attachments.Any())
            {
                await emailSender.SendEmailAsync(email.To, email.Subject, body, attachments, isHtml);
            }
            else
            {
                await emailSender.SendEmailAsync(email.To, email.Subject, body, isHtml);
            }

            // Mark as sent
            var messageId = Guid.NewGuid().ToString(); // Or get from provider response
            await emailQueueService.MarkEmailSentAsync(emailId, messageId);

            // Update throttle counter
            _emailsSentThisMinute++;

            _log.Information("Successfully sent email {EmailId}", emailId);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error sending email {EmailId}", emailId);

            // Mark as failed (will schedule retry if applicable)
            await emailQueueService.MarkEmailFailedAsync(emailId, ex.Message);
        }
    }

    /// <summary>
    /// Waits if throttle limit is reached
    /// </summary>
    private async Task WaitForThrottleIfNeeded(CancellationToken stoppingToken)
    {
        var now = DateTime.UtcNow;

        // Reset counter if a new minute has started
        if ((now - _currentMinuteStart).TotalMinutes >= 1)
        {
            _currentMinuteStart = now;
            _emailsSentThisMinute = 0;
        }

        // If limit reached, wait until the next minute
        if (_emailsSentThisMinute >= _maxEmailsPerMinute)
        {
            var waitTime = TimeSpan.FromMinutes(1) - (now - _currentMinuteStart);
            if (waitTime > TimeSpan.Zero)
            {
                _log.Information("Throttle limit reached ({Count}/{Max}), waiting {WaitTime}", 
                    _emailsSentThisMinute, _maxEmailsPerMinute, waitTime);
                
                await Task.Delay(waitTime, stoppingToken);
                
                _currentMinuteStart = DateTime.UtcNow;
                _emailsSentThisMinute = 0;
            }
        }
    }

    /// <summary>
    /// Gets the appropriate email sender based on provider
    /// </summary>
    private IEmailSender GetEmailSender(IServiceScope scope, string? provider)
    {
        // For now, return default email sender
        // TODO: Implement provider-specific logic to select appropriate IEmailSender
        // e.g., if provider == "SendGrid", return SendGridEmailSender
        
        var emailSender = scope.ServiceProvider.GetService<IEmailSender>();
        
        if (emailSender == null)
        {
            throw new InvalidOperationException("No IEmailSender implementation registered");
        }

        return emailSender;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _log.Information("Email Queue Background Service stopping");
        
        // Complete the channel to stop accepting new items
        _emailNotificationChannel.Writer.Complete();
        
        await base.StopAsync(stoppingToken);
    }
}
