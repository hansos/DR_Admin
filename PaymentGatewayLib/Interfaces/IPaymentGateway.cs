using PaymentGatewayLib.Models;

namespace PaymentGatewayLib.Interfaces
{
    /// <summary>
    /// Defines the contract for payment gateway providers
    /// </summary>
    public interface IPaymentGateway
    {
        /// <summary>
        /// Processes a payment transaction
        /// </summary>
        /// <param name="request">Payment request containing transaction details</param>
        /// <returns>Payment result with transaction status and details</returns>
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);

        /// <summary>
        /// Refunds a previous payment transaction
        /// </summary>
        /// <param name="request">Refund request containing transaction details</param>
        /// <returns>Refund result with transaction status and details</returns>
        Task<RefundResult> RefundPaymentAsync(RefundRequest request);

        /// <summary>
        /// Retrieves the status of a payment transaction
        /// </summary>
        /// <param name="transactionId">Unique identifier of the transaction</param>
        /// <returns>Transaction status information</returns>
        Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId);

        /// <summary>
        /// Creates a payment intent for later processing
        /// </summary>
        /// <param name="request">Payment intent request containing transaction details</param>
        /// <returns>Payment intent result with client secret and intent ID</returns>
        Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request);

        /// <summary>
        /// Captures an authorized payment
        /// </summary>
        /// <param name="authorizationId">Authorization identifier from previous authorization</param>
        /// <param name="amount">Amount to capture (optional, defaults to full amount)</param>
        /// <returns>Capture result with transaction status and details</returns>
        Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null);

        /// <summary>
        /// Voids an authorized payment before capture
        /// </summary>
        /// <param name="authorizationId">Authorization identifier to void</param>
        /// <returns>Void result with transaction status</returns>
        Task<VoidResult> VoidPaymentAsync(string authorizationId);

        /// <summary>
        /// Creates a customer profile in the payment gateway
        /// </summary>
        /// <param name="request">Customer profile request</param>
        /// <returns>Customer profile result with customer ID</returns>
        Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request);

        /// <summary>
        /// Retrieves list of transactions for a specific period
        /// </summary>
        /// <param name="startDate">Start date for transaction search</param>
        /// <param name="endDate">End date for transaction search</param>
        /// <returns>List of transactions within the specified period</returns>
        Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate);
    }
}
