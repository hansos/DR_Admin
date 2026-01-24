using PaymentGatewayLib.Models;

namespace PaymentGatewayLib.Implementations
{
    /// <summary>
    /// Base class for payment gateway implementations providing common functionality
    /// </summary>
    public abstract class BasePaymentGateway
    {
        /// <summary>
        /// Validates a payment request before processing
        /// </summary>
        /// <param name="request">Payment request to validate</param>
        /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
        /// <exception cref="ArgumentException">Thrown when request contains invalid data</exception>
        protected virtual void ValidatePaymentRequest(PaymentRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(request));

            if (string.IsNullOrWhiteSpace(request.Currency))
                throw new ArgumentException("Currency is required", nameof(request));

            if (string.IsNullOrWhiteSpace(request.PaymentMethodToken))
                throw new ArgumentException("Payment method token is required", nameof(request));
        }

        /// <summary>
        /// Validates a refund request before processing
        /// </summary>
        /// <param name="request">Refund request to validate</param>
        /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
        /// <exception cref="ArgumentException">Thrown when request contains invalid data</exception>
        protected virtual void ValidateRefundRequest(RefundRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.TransactionId))
                throw new ArgumentException("Transaction ID is required", nameof(request));

            if (request.Amount.HasValue && request.Amount.Value <= 0)
                throw new ArgumentException("Refund amount must be greater than zero", nameof(request));
        }

        /// <summary>
        /// Validates a payment intent request before processing
        /// </summary>
        /// <param name="request">Payment intent request to validate</param>
        /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
        /// <exception cref="ArgumentException">Thrown when request contains invalid data</exception>
        protected virtual void ValidatePaymentIntentRequest(PaymentIntentRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(request));

            if (string.IsNullOrWhiteSpace(request.Currency))
                throw new ArgumentException("Currency is required", nameof(request));
        }

        /// <summary>
        /// Validates a customer profile request before processing
        /// </summary>
        /// <param name="request">Customer profile request to validate</param>
        /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
        /// <exception cref="ArgumentException">Thrown when request contains invalid data</exception>
        protected virtual void ValidateCustomerProfileRequest(CustomerProfileRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required", nameof(request));

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name is required", nameof(request));
        }

        /// <summary>
        /// Creates an error payment result
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="errorCode">Error code</param>
        /// <returns>Payment result indicating failure</returns>
        protected PaymentResult CreateErrorResult(string errorMessage, string errorCode = "")
        {
            return new PaymentResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                ProcessedAt = DateTime.UtcNow
            };
        }
    }
}
