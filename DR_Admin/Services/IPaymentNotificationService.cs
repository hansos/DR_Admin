namespace ISPAdmin.Services;

/// <summary>
/// Service interface for payment-related notifications
/// </summary>
public interface IPaymentNotificationService
{
    // Invoice notifications
    Task SendInvoiceCreatedNotificationAsync(int invoiceId);
    Task SendInvoicePaymentDueReminderAsync(int invoiceId);
    Task SendInvoiceOverdueNotificationAsync(int invoiceId);
    Task SendPaymentReceivedConfirmationAsync(int invoiceId);

    // Payment attempt notifications
    Task SendPaymentFailedNotificationAsync(int paymentAttemptId);
    Task SendPaymentAuthenticationRequiredAsync(int paymentAttemptId);
    
    // Refund notifications
    Task SendRefundProcessedNotificationAsync(int refundId);

    // Subscription notifications
    Task SendSubscriptionPaymentSuccessAsync(int subscriptionId);
    Task SendSubscriptionPaymentFailedAsync(int subscriptionId);
    Task SendPaymentMethodExpiringNotificationAsync(int paymentMethodId);
}
