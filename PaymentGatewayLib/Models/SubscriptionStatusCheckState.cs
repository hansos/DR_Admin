namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Normalized subscription status returned from payment gateways.
    /// </summary>
    public enum SubscriptionStatusCheckState
    {
        Unknown = 0,
        Active = 1,
        Trialing = 2,
        PastDue = 3,
        Cancelled = 4,
        Incomplete = 5
    }
}
